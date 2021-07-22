using System;
using System.Collections.Generic;
using System.IO;

namespace CSharpPlayground
{
    class Program
    {
        static void Main(string[] args)
        {
            List<int[]> edges = GetGraphFromFile(@"..\..\..\Graph.txt"); //GetGraphFromConsole();

            if (edges.Count == 0)
            {
                Console.WriteLine("No valid edges were inputted. No graph generated. Exitting.");
                return;
            }

            PathFinder pathFinder = new PathFinder(edges);

            pathFinder.PrintGraph();
            Console.WriteLine("\n");
            int[] path = pathFinder.FindPath(1, 5);

            foreach (int node in path)
                Console.WriteLine(node);
        }

        static List<int[]> GetGraphFromConsole()
        {
            Console.WriteLine("Please enter edge information in the format: <start node index> <end node index> <edge cost>. The edges do not need to be symmetric. Please enter 'end' when finished.");

            string input = Console.ReadLine();
            List<int[]> edges = new List<int[]>();

            while (!input.Equals("end"))
            {
                string[] edgeInfo = input.Split();

                if (edgeInfo.Length != 3)
                {
                    Console.WriteLine("Invalid argument number. Please enter edge information in the format: <start node index> <end node index> <edge cost>.");
                    input = Console.ReadLine();
                    continue;
                }

                int[] edge = new int[3];

                for (int i = 0; i < 3; i++)
                {
                    bool parse = int.TryParse(edgeInfo[i], out edge[i]);

                    if (!parse)
                    {
                        Console.WriteLine("Arguments would not be converted to Integers. Please enter edge information in the format: <start node index> <end node index> <edge cost>.");
                        input = Console.ReadLine();
                        continue;
                    }
                }

                edges.Add(edge);

                input = Console.ReadLine();
            }

            return edges;
        }

        static List<int[]> GetGraphFromFile(string path)
        {
            string[] inputs = System.IO.File.ReadAllLines(path);
            List<int[]> edges = new List<int[]>();

            foreach (string input in inputs)
            {
                string[] edgeInfo = input.Split();

                if (edgeInfo.Length != 3)
                {
                    Console.WriteLine("Invalid argument number. Please enter edge information in the format: <start node index> <end node index> <edge cost>.");
                    continue;
                }

                int[] edge = new int[3];

                for (int i = 0; i < 3; i++)
                {
                    bool parse = int.TryParse(edgeInfo[i], out edge[i]);

                    if (!parse)
                    {
                        Console.WriteLine("Arguments would not be converted to Integers. Please enter edge information in the format: <start node index> <end node index> <edge cost>.");
                        continue;
                    }
                }

                edges.Add(edge);
            }

            return edges;
        }
    }

    class PathFinder
    {
        Dictionary<int, Node> stateSpace = new Dictionary<int, Node>();
        public PathFinder(List<int[]> edges) //edges has 3 elements: from, to, and cost in order
        {
            foreach (int[] edge in edges)
            {
                Node startNode;
                if (!stateSpace.ContainsKey(edge[0]))
                {
                    startNode = new Node(edge[0]);
                    stateSpace.Add(edge[0], startNode);
                }
                else
                    startNode = stateSpace[edge[0]];

                Node endNode;
                if (!stateSpace.ContainsKey(edge[1]))
                {
                    endNode = new Node(edge[1]);
                    stateSpace.Add(edge[1], endNode);
                }
                else
                    endNode = stateSpace[edge[1]];

                Tuple<Node, int> newEdge = new Tuple<Node, int>(endNode, edge[2]);
                startNode.AddEdge(newEdge);
            }
        }

        public void PrintGraph()
        {
            foreach (KeyValuePair<int, Node> state in stateSpace)
            {
                foreach (Tuple<Node, int> edge in state.Value.GetEdges())
                {
                    Console.WriteLine("{0} {1} {2}", state.Key, edge.Item1.GetValue(), edge.Item2);
                }
            }
        }

        public int[] FindPath(int startNode, int endNode)
        {
            List<Node> solvedNodes = new List<Node>();
            stateSpace[startNode].cost = 0;
            solvedNodes.Add(stateSpace[startNode]);
            
            while (solvedNodes.Count > 0)
            {
                int minCost = int.MaxValue;
                Node curr = solvedNodes[0];
                foreach (Node next in solvedNodes)
                {
                    if (next.GetValue() < minCost)
                    {
                        minCost = next.GetValue();
                        curr = next;
                    }
                }

                solvedNodes.Remove(curr);

                foreach (Tuple<Node, int> child in curr.GetEdges())
                {
                    if (child.Item1.cost == int.MaxValue)
                        solvedNodes.Add(child.Item1);

                    if (curr.cost + child.Item2 < child.Item1.cost)
                    {
                        child.Item1.cost = curr.cost + child.Item2;
                        child.Item1.parent = curr;
                    }
                }
            }

            Stack<Node> nodePath = new Stack<Node>();
            Node temp = stateSpace[endNode];
            while (temp != null)
            {
                nodePath.Push(temp);
                temp = temp.parent;
            }

            int[] path = new int[nodePath.Count];
            for (int i = 0; i < path.Length; i++)
            {
                temp = nodePath.Pop();
                path[i] = temp.GetValue();
            }

            return path;
        }
    }

    class Node
    {
        int value;
        public int cost = int.MaxValue;
        List<Tuple<Node, int>> outEdges = new List<Tuple<Node, int>>();
        public Node parent;

        public Node(int value)
        {
            this.value = value;
        }

        public void AddEdge(Tuple<Node, int> edge)
        {
            this.outEdges.Add(edge);
        }

        public List<Tuple<Node, int>> GetEdges()
        {
            return outEdges;
        }

        public int GetValue()
        {
            return value;
        }
    }
}
