namespace TravellingSalesmanProblem
{
    public class CitiesData
    {
        private readonly int[,] _weightMatrix;

        public CitiesData(int[,] weightMatrix)
        {
            _weightMatrix = weightMatrix;
        }

        public int[,] WeightMatrix => (int[,]) _weightMatrix.Clone();
        public int GetDimension => _weightMatrix.GetLength(0);
        public int GetWeight(int i, int j) => _weightMatrix[i, j];
    }
}