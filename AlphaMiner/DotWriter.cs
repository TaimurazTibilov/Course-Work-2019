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
        /// <summary>
        /// Указывает на необходимость дополнительного соединяющего узла в конце графа
        /// </summary>
        internal bool isJoinNeeded = false;

        /// <summary>
        /// Указывает на необходимость дополнительного соединяющего узла в начале графа
        /// </summary>
        internal bool isSplitNeeded = false;

        /// <summary>
        /// Список всех задач лога
        /// </summary>
        List<string> AllTasks { get; set; }

        /// <summary>
        /// Список всех узлов сети Петри
        /// </summary>
        List<Node> Nodes { get; set; }

        /// <summary>
        /// Генератор обозначений вершин
        /// </summary>
        string Vertex
        {
            get { return "A" + (counter++).ToString(); }
        }

        /// <summary>
        /// Копилка
        /// </summary>
        int counter = 1;

        /// <summary>
        /// Конструктор объекта записи графа в формате .DOT
        /// </summary>
        /// <param name="allTasks">Список всех задач</param>
        /// <param name="nodes">Все узлы графа</param>
        internal DotWriter(List<string> allTasks, List<Node> nodes)
        {
            AllTasks = allTasks;
            Nodes = nodes;
        }

        /// <summary>
        /// Представляет граф в формате .DOT исходя из данных объекта
        /// </summary>
        /// <returns>Строковое представлеие графа</returns>
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

        /// <summary>
        /// Метод записи графа в указанный каталог и его сохранения в формате .DOT
        /// </summary>
        /// <param name="PATH">Ссылка на каталог для сохранения графа</param>
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

        /// <summary>
        /// Метод для создания дуг сети Петри по полученным вершинам
        /// </summary>
        /// <param name="graph">Строковое представление графа для записи</param>
        /// <param name="vertexes">Список вершин по задачам</param>
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
