using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace studentProblem
{
    public sealed class NodeService
    {
        NodeService()
        {
        }
        private static List<Node> nodes = new List<Node>();
        private static readonly object padlock = new object();
        private static NodeService instance = null;
        public static NodeService Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new NodeService();
                    }
                    return instance;
                }
            }
        }
        public List<Node> initializeNodes(int nodesAmount, int interestAmount)
        {
            if (interestAmount >= nodesAmount)
            {
                //TODO: throw an error, interestAmount can't be larger to equal to the nodesAmount
            }
            nodes.Clear();
            for (int i = 0; i < nodesAmount; i++)
            {
                nodes.Add(new Node());
            }
            assignConnections(interestAmount);
            return nodes;
        }

        private void assignConnections(int interestAmount)
        {
            foreach (Node currentNode in nodes)
            {
                List<Node> currentUnassignedNodes = new List<Node>(nodes);
                currentUnassignedNodes.Remove(currentNode);
                for (int i = 0; i < interestAmount; i++)
                {
                    Node currentRandomNode;
                    currentRandomNode = getRandomNode(currentUnassignedNodes);
                    currentNode.AddInterestToNode(currentRandomNode);
                    currentUnassignedNodes.Remove(currentRandomNode);
                }
            }
        }

        private Node getRandomNode(List<Node> nodeList)
        {
            Random random = new Random();
            return nodeList[random.Next(nodeList.Count)];
        }
    }
}
