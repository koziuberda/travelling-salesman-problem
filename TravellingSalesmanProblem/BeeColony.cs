using System;
using System.Collections.Generic;

namespace TravellingSalesmanProblem
{
    public class BeeColony
    {
        private static Random _random;
        
        private readonly CitiesData _citiesData;

        private int _totalNumberBees;
        private int _numberActive;
        private int _numberInactive;
        private int _numberScout;
        
        private int _maxNumberCycles;
        private int _maxNumberVisits;
        
        private const double ProbPersuasion = 0.90;
        private const double ProbMistake = 0.01; 

        private Bee[] _bees;
        private Bee _bestBee;
        private int[] _indexesOfInactiveBees;

        public BeeColony(CitiesData citiesData, int numberActive, int numberInactive,
            int numberScout, int maxNumberCycles, int maxNumberVisits)
        {
            _citiesData = citiesData;
            _numberActive = numberActive;
            _numberInactive = numberInactive;
            _numberScout = numberScout;
            _maxNumberCycles = maxNumberCycles;
            _maxNumberVisits = maxNumberVisits;

            _random = new Random();
            _totalNumberBees = numberActive + numberInactive + numberScout;
            _bees = new Bee[_totalNumberBees];

            List<int> initialSolution = GetRandomSolution();
            _bestBee = new Bee(initialSolution, Bee.BeeStatus.Inactive, Fitness(initialSolution), 0);
            
            _indexesOfInactiveBees = new int[numberInactive];

            for (int i = 0; i < _totalNumberBees; i++)
            {
                Bee.BeeStatus status;
                if (i < numberInactive)
                {
                    status = Bee.BeeStatus.Inactive;
                    _indexesOfInactiveBees[i] = i;
                }
                else if (i < numberInactive + numberScout)
                {
                    status = Bee.BeeStatus.Scout;
                }
                else
                    status = Bee.BeeStatus.Active;

                List<int> beeWay = GetRandomSolution();
                int beeQuality = Fitness(beeWay);
                int numberOfVisits = 0;

                _bees[i] = new Bee(beeWay, status, beeQuality, numberOfVisits);

                if (_bees[i].Value < _bestBee.Value)
                {
                    _bestBee.LocationOfFlowerPatch = _bees[i].LocationOfFlowerPatch; 
                    _bestBee.Value = _bees[i].Value;
                }
            }
        }

        public List<int> Solve(out int distance)
        {
            for (int cycle = 0; cycle < _maxNumberCycles; cycle++)
            {
                for (int i = 0; i < _totalNumberBees; i++)
                {
                    if (_bees[i].Status == Bee.BeeStatus.Active)
                    {
                        ProcessActiveBee(i);
                    }
                    else if(_bees[i].Status == Bee.BeeStatus.Scout)
                    {
                        ProcessScoutBee(i);
                    }
                }
            }

            distance = _bestBee.Value;

            var bestSolution = _bestBee.LocationOfFlowerPatch;
            
            // Return to the start point
            bestSolution.Add(bestSolution[0]);
            
            return bestSolution;
        }

        private void ProcessActiveBee(int i)
        {
            List<int> neighbour = GetNeighbourSolution(_bees[i].LocationOfFlowerPatch);
            int neighbourValue = Fitness(neighbour);
            double probability = _random.NextDouble();

            bool beeGotAnotherSolution = false;
            bool shouldLeave = false;

            // Better solution was found
            if (neighbourValue < _bees[i].Value)
            {
                // The bee is wrong. Doesn't change solution
                if (probability < ProbMistake)
                {
                    ++_bees[i].NumberOfVisit;
                    if (_bees[i].NumberOfVisit > _maxNumberVisits)
                    {
                        shouldLeave = true;
                    }
                }
                // The bee is right. It takes a different solution
                else
                {
                    _bees[i] = new Bee(neighbour, Bee.BeeStatus.Active,neighbourValue, 0);
                    beeGotAnotherSolution = true;
                }
            }
            // Didn't find any better solution
            else
            {
                // Mistakenly get worse solution
                if (probability < ProbMistake)
                {
                    _bees[i] = new Bee(neighbour, Bee.BeeStatus.Active, neighbourValue, 0);
                    beeGotAnotherSolution = true;
                }
                // Keeps the solution unchanged
                else
                {
                    ++_bees[i].NumberOfVisit;
                    if (_bees[i].NumberOfVisit > _maxNumberVisits)
                    {
                        shouldLeave = true;
                    }
                }
            }

            if (shouldLeave)
            {
                _bees[i].Status = Bee.BeeStatus.Inactive;
                _bees[i].NumberOfVisit = 0;
                int nextBeeIndex = _random.Next(_numberInactive);
                _bees[_indexesOfInactiveBees[nextBeeIndex]].Status = Bee.BeeStatus.Active;
                _indexesOfInactiveBees[nextBeeIndex] = i;
            }
            else if (beeGotAnotherSolution)
            {
                if (_bees[i].Value < _bestBee.Value)
                {
                    _bestBee.LocationOfFlowerPatch = _bees[i].LocationOfFlowerPatch; 
                    _bestBee.Value = _bees[i].Value;
                }
                DoWaggleDance(i);
            }

            return;
        }

        private void ProcessScoutBee(int i)
        {
            List<int> randomSolution = GetRandomSolution();
            int randomSolutionValue = Fitness(randomSolution);

            if (randomSolutionValue < _bees[i].Value)
            {
                _bees[i].LocationOfFlowerPatch = randomSolution;
                _bees[i].Value = randomSolutionValue;

                if (_bees[i].Value < _bestBee.Value)
                {
                    _bestBee.LocationOfFlowerPatch = _bees[i].LocationOfFlowerPatch;
                    _bestBee.Value = _bees[i].Value;
                }
                DoWaggleDance(i);
            }
        }

        private void DoWaggleDance(int dancingBeeIdx)
        {
            for (int i = 0; i < _numberInactive; i++)
            {
                int inactive = _indexesOfInactiveBees[i];
                if (_bees[dancingBeeIdx].Value < _bees[inactive].Value)
                {
                    double prob = _random.NextDouble();
                    if (prob < ProbPersuasion)
                    {
                        _bees[inactive].LocationOfFlowerPatch = _bees[dancingBeeIdx].LocationOfFlowerPatch;
                        _bees[inactive].Value = _bees[dancingBeeIdx].Value;
                    }
                }
            }
        }

        private List<int> GetRandomSolution()
        {
            var solution = new List<int>();

            for (int i = 0; i < _citiesData.Dimension; i++)
            {
                solution.Add(i);
            }

            // Fisher-Yates shuffle algorithm
            for (int i = 0; i < solution.Count; i++)
            {
                int rand = _random.Next(i, solution.Count);
                (solution[i], solution[rand]) = (solution[rand], solution[i]);
            }

            return solution;
        }

        private List<int> GetNeighbourSolution(List<int> solution)
        {
            var neighbour = new List<int>(solution);

            int firstIndex = _random.Next(0, neighbour.Count);
            int secondIndex;
            if (firstIndex == neighbour.Count-1)
            {
                secondIndex = 0;
            }
            else
            {
                secondIndex = firstIndex + 1;
            }

            (neighbour[firstIndex], neighbour[secondIndex]) = (neighbour[secondIndex], neighbour[firstIndex]);

            return neighbour;
        }

        private int Fitness(List<int> solution)
        {
            var costOfPath = 0;
            
            for (int i = 0; i < solution.Count - 1; i++)
            {
                costOfPath += _citiesData.GetWeight(solution[i], solution[i + 1]);
            }

            // Goes back to the start city
            costOfPath += _citiesData.GetWeight(solution[solution.Count-1], solution[0]);

            return costOfPath;
        }
    }
}