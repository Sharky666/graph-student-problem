using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace studentProblem
{
    public class Node
    {
        private List<Node> interestedIn = new List<Node>();

        public void AddInterestToNode(Node node)
        {
            interestedIn.Add(node);
        }
        public List<Node> InterestedIn { get => interestedIn; }
    }
}
