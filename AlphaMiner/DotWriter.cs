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

        string CreateGraph()
        {
            string graph;
            VertexesOfTasks vertexes = new VertexesOfTasks(AllTasks);
            graph = "digraph WFnet {\n";
            foreach (var node in Nodes)
            {
                string v = Vertex;
                node.Vertex = v;
                graph += $"\t{v} [label=\"\", shape=circle];\n";
            }
            foreach (var task in AllTasks)
            {
                if (task == "AND-split" && (!isSplitNeeded))
                    continue;
                if (task == "AND-join" && (!isJoinNeeded))
                    continue;
                graph += $"\t{vertexes[task]} [label=\"{task}\", shape=box];\n";
            }
            GetEdges(ref graph, vertexes);
            graph += "}";
            return graph;
        }

        internal void WriteGraph(string PATH)
        {           
            try
            {
                if (!Directory.Exists(PATH))
                    Directory.CreateDirectory(PATH);
                if (PATH[PATH.Length - 1] == '/')
                    PATH += @"WFNet.DOT";
                else
                    PATH += @"/WFNet.DOT";
                using (StreamWriter writer = new StreamWriter(new FileStream(PATH, FileMode.Create), Encoding.UTF8))
                {
                    writer.WriteLine(CreateGraph());
                }
            }
            catch (Exception e)
            {
                throw new WrongFormatOfPetriPathException(e.Message);
            }
        }

        void GetEdges(ref string graph, VertexesOfTasks vertexes)
        {
            foreach (var node in Nodes)
            {
                if (node.InputTasks != null)
                {
                    foreach (var task in node.InputTasks)
                    {
                        graph += $"\t{vertexes[task]} -> {node.Vertex};\n";
                    }
                }
                if (node.OutputTasks != null)
                {
                    foreach (var task in node.OutputTasks)
                    {
                        graph += $"\t{node.Vertex} -> {vertexes[task]};\n";
                    }
                }
            }
        }
    }
}
