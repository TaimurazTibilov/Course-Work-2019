using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlphaMiner
{
    public class AlphaMiner
    {
        public AlphaMiner(string pathOfLog, string pathOfWrite)
        {
            PathOfLog = pathOfLog;
            PathOfWrite = pathOfWrite;
        }

        public string PathOfLog { get; set; }

        public string PathOfWrite { get; set; }

        List<string> FirstTasks { get; set; }

        List<string> LastTasks { get; set; }

        internal List<string> AllTasks { get; set; }

        AdjacencyMatrix Matrix { get; set; }

        List<Node> Nodes { get; set; }

        // Метод запуска алгоритма
        public void StartAlpha()
        {

        }

        // Получает и записывает все Task (Event), встречающиеся в логе
        void GetTasks()
        {
            //TODO: Get IEnumerated strings of all tasks
            Console.WriteLine("Write down all types of tasks that used in log in format A, B, etc.");
            foreach (var task in Console.ReadLine().Split(new char[] { ' ', ',', ';', '\'', '\"' }, StringSplitOptions.RemoveEmptyEntries))
            {
                AllTasks.Add(task);
            }
            return;
        }

        // Обрабатывает полученный на вход Trace, составляет по нему отношение между задачами
        void GetRelationships(string[] trace)
        {
            if (!FirstTasks.Contains(trace[0]))
            {
                FirstTasks.Add(trace[0]);
            }
            if (!LastTasks.Contains(trace[trace.Length - 1]))
            {
                LastTasks.Add(trace[trace.Length - 1]);
            }
            if (trace.Length == 1)
                return;
            for (int i = 0; i < trace.Length - 1; i++)
            {
                Matrix[trace[i], trace[i + 1]] = ">";
            }
        }

        // Получает поочередно строку Trace по их количеству, в релизе НЕ НУЖЕН! 
        void GetData(int num)
        {
            //TODO: Get Enumerated Strings that contains traces
            Console.WriteLine("Write down all traces in format A B C, etc. : ");
            for (int i = 0; i < num; i++)
            {
                GetRelationships(Console.ReadLine().Split(new char[] { ' ', ',', ';', '\'', '\"' },
                    StringSplitOptions.RemoveEmptyEntries));
            }
        }

        // Создает узлы для их дальнейшей записи и представления в формате DOT
        void GetNodes()
        {
            foreach (var firstTask in AllTasks)
            {
                if (!LastTasks.Contains(firstTask))
                {
                    var node = new Node();
                    node.InputTasks.Add(firstTask);

                    foreach (var secondTask in AllTasks)
                    {
                        if (Matrix[firstTask, secondTask] == ">")
                        {
                            node.OutputTasks.Add(secondTask);
                        }
                    }
                }
            }
            foreach (var first in Nodes)
            {
                foreach (var second in Nodes)
                {
                    if (first != second)
                        UniteNodes(first, second);
                }
            }
            foreach (var node in Nodes)
                CheckNode(node);
            //TODO: Сформировать узлы для начальных и конечных позиций
            return;
        }

        // Объединяет найденные узлы и изменяет их
        void UniteNodes(Node first, Node second)
        {
            if (first.OutputTasks.All(x => second.OutputTasks.Contains(x)))
            {
                if (second.OutputTasks.All(x => first.OutputTasks.Contains(x)))
                {
                    UniteInputTasks(first.InputTasks, second.InputTasks);
                    if (first.InputTasks.All(x => second.InputTasks.Contains(x)))
                    {
                        if (second.InputTasks.All(x => first.InputTasks.Contains(x)))
                        {
                            Nodes.Remove(second);
                        }
                    }
                }
                UniteInputTasks(second.InputTasks, first.InputTasks);
            }
            if (second.OutputTasks.All(x => first.OutputTasks.Contains(x)))
            {
                UniteInputTasks(first.InputTasks, second.InputTasks);
            }
        }

        // Производит объединение 2-х множеств InputTasks по установленному порядку
        void UniteInputTasks(List<string> first, List<string> second)
        {
            foreach (var firstTask in first)
            {
                bool flag = true;
                foreach (var secondTask in second)
                {
                    if (Matrix[firstTask, secondTask] != "#" || Matrix[secondTask, firstTask] != "#")
                        flag = false;
                    if (firstTask == secondTask)
                        flag = false;
                }
                if (flag)
                    second.Add(firstTask);
            }
            return;
        }

        // Проверяет мн-во OutputTasks на предмет несоотв. связей для узла
        void CheckNode(Node node)
        {
            foreach (var first in node.OutputTasks)
            {
                foreach (var second in node.OutputTasks)
                {
                    if (Matrix[first, second] != "#" || Matrix[second, first] != "#")
                    {
                        string[] tasks = null;
                        node.OutputTasks.CopyTo(tasks);
                        var firstList = new List<string>(tasks);
                        var secondList = new List<string>(tasks);
                        firstList.Remove(first);
                        secondList.Remove(second);
                        var firstNode = new Node()
                        {
                            InputTasks = node.InputTasks,
                            OutputTasks = firstList
                        };
                        var secondNode = new Node()
                        {
                            InputTasks = node.InputTasks,
                            OutputTasks = secondList
                        };
                        Nodes.Remove(node);
                        Nodes.Add(firstNode);
                        Nodes.Add(secondNode);
                        CheckNode(firstNode);
                        CheckNode(secondNode);
                        goto EndOfChecking;
                    }
                }
            }
        EndOfChecking:
            return;
        }
    }
}
