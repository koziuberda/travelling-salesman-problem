using System.Collections.Generic;

namespace TravellingSalesmanProblem
{
    public class Bee
    {
        public BeeStatus Status { get; set; }
        public int Value { get; set; }
        public int NumberOfVisit { get; set; }
        private List<int> _traversalOrder;

        public Bee(List<int> traversalOrder, BeeStatus status, int value, int numberOfVisit)
        {
            _traversalOrder = traversalOrder;
            Status = status;
            Value = value;
            NumberOfVisit = numberOfVisit;
        }

        public List<int> LocationOfFlowerPatch()
        {
            return new List<int>(_traversalOrder);
        }
        
        public enum BeeStatus
        {
            Active,
            Inactive,
            Scout
        }
    }
}