using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlphaMiner
{
    internal class DotWriter
    {
        internal bool isJoinNeeded = false;

        internal bool isSplitNeeded = false;

        List<string> AllTasks { get; set; }

        List<Node> Nodes { get; set; }

        VertexesOfTasks Vertexes { get; set; }

        string Graph { get; set; }

        string Vertex
        {
            get { return "A" + (counter++).ToString(); }
        }

        int counter = 1;

        internal DotWriter(List<string> allTasks, List<Node> nodes)
        {
            AllTasks = allTasks;
            Nodes = nodes;
        }

        internal void WriteGraphToDOT(string PATH)
        {
            if (PATH[PATH.Length - 1] == '/')
                PATH += @"WFNet.DOT";
            else
                PATH += @"/WFNet.DOT";
            Vertexes = new VertexesOfTasks(AllTasks);
            Graph = "digraph WFnet {\n";
            foreach (var node in Nodes)
            {
                string v = Vertex;
                node.Vertex = v; 
                Graph += $"\t{v} [label=\"\", shape=circle];\n";
            }
            foreach (var task in AllTasks)
            {
                if (task == "AND-split" && (!isSplitNeeded))
                    continue;
                if (task == "AND-join" && (!isJoinNeeded))
                    continue;
                Graph += $"\t{Vertexes[task]} [label=\"{task}\", shape=box];\n";
            }
            GetEdges();
            Graph += "}";
            try
            {
                using (StreamWriter writer = new StreamWriter(new FileStream(PATH, FileMode.Create), Encoding.UTF8))
                {
                    writer.WriteLine(Graph);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        void GetEdges()
        {
            foreach (var node in Nodes)
            {
                if (node.InputTasks != null)
                {
                    foreach (var task in node.InputTasks)
                    {
                        Graph += $"\t{Vertexes[task]} -> {node.Vertex};\n";
                    }
                }
                if (node.OutputTasks != null)
                {
                    foreach (var task in node.OutputTasks)
                    {
                        Graph += $"\t{node.Vertex} -> {Vertexes[task]};\n";
                    }
                }
            }
        }
    }
}
