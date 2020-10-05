using System;
using System.Collections.Generic;
using System.Text;

namespace studentProblem
{
    class CommunitiesCalculationClient
    {
        public CommunitiesCalculationInterface calculcationStrategy { get; set; }
        public CommunitiesCalculationClient(CommunitiesCalculationInterface strategy)
        {
            calculcationStrategy = strategy;
        }

        public RoomsStruct calculate(List<Node> nodes, int nodesPerRoom)
        {
            return calculcationStrategy.Calculate(nodes, nodesPerRoom);
        }

    }
}
