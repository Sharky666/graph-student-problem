using System;
using System.Collections.Generic;

namespace studentProblem
{
    class Program
    {
        static void Main(string[] args)
        {
            NodeService nodeService = NodeService.Instance;
            List<Node> nodes = nodeService.initializeNodes(20, 4);
            CommunitiesCalculationClient calculationClient = new CommunitiesCalculationClient(new NormalizeCommunities());
            RoomsStruct roomsStruct = calculationClient.calculate(nodes, 4);
            foreach (Room room in roomsStruct.rooms)
            {
                Console.WriteLine($" {room.nodes.Count}");
            }
            Console.WriteLine($"legnth of unassigned nodes: {roomsStruct.unassignedNodes.Length}");
        }
    }
}
