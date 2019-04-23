using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using EventLogConnector;


namespace AlphaMiner
{
    public class Alpha
    {
        public Alpha(string pathOfLog, string pathOfWrite)
        {
            PathOfLog = pathOfLog;
            PathOfWrite = pathOfWrite;
            FirstTasks = new List<string>();
            LastTasks = new List<string>();
            AllTasks = new List<string>();
            Nodes = new List<Node>();
        }

        bool isJoinNeeded = false;

        bool isSplitNeeded = false;

        public string PathOfLog { get; set; }

        public string PathOfWrite { get; set; }

        List<string> FirstTasks { get; set; }

        List<string> LastTasks { get; set; }

        internal List<string> AllTasks { get; set; }

        AdjacencyMatrix Matrix { get; set; }

        List<Node> Nodes { get; set; }

        //EventLogConnector.EvLog EventLog { get; set; }

        DotWriter Writer { get; set; }

        bool IsInLog = false; // Not needed in release

        // Метод запуска алгоритма
        public void StartAlpha(bool isInLog)
        {
            //TODO: 1) Написать XML-комментарии для каждого метода и свойства в программе
            //      2) Разобраться, почему нумерация у вершин задач идет в порядке 1,4 (,5,6...)
            //      3) Реализовать класс Event, в котором будут задачи и их ID
            int num;
            this.IsInLog = isInLog;
            if (isInLog)
            {
                //EventLog = new EvLog(PathOfLog);
            }
            GetTasks();
            AllTasks.Add("AND-split");
            AllTasks.Add("AND-join");
            Matrix = new AdjacencyMatrix(AllTasks);
            if (!isInLog)
            {
                do
                {
                    Console.WriteLine("Write number of traces");
                } while (!int.TryParse(Console.ReadLine(), out num) || num < 1);
                GetData(num);
            }
            else
                GetData(-1);
            GetNodes();
            Writer = new DotWriter(AllTasks, Nodes)
            {
                isSplitNeeded = this.isSplitNeeded,
                isJoinNeeded = this.isJoinNeeded
            };
            Writer.WriteGraphToDOT(PathOfWrite);
            return;
        }

        // Получает и записывает все Task (Event), встречающиеся в логе
        void GetTasks()
        {
            /*
            if (IsInLog)
            {
                //Activity[] activities = EventLog.qryl_get_activities();
                foreach (var act in activities)
                {
                    AllTasks.Add(act.Name);
                }
                return;
            }
            */
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
                if (Matrix[trace[i + 1], trace[i]] == "#")
                    Matrix[trace[i], trace[i + 1]] = ">";
                else
                {
                    Matrix[trace[i], trace[i + 1]] = "||";
                    Matrix[trace[i + 1], trace[i]] = "||";
                }
            }
        }

        // Получает поочередно строку Trace по их количеству, в релизе НЕ НУЖЕН! 
        void GetData(int num)
        {
            /*
            if (num == -1)
            {
                Trace[] traces = EventLog.FindEventsForAllTraces();
                foreach (var trace in traces)
                {
                    List<string> act = new List<string>();
                    foreach (var activity in trace.traceEvents)
                    {
                        act.Add(activity.activity.Name);
                    }
                    GetRelationships(act.ToArray());
                }
            }
            */
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
                var node = new Node();
                node.InputTasks.Add(firstTask);

                foreach (var secondTask in AllTasks)
                {
                    if (Matrix[firstTask, secondTask] == ">")
                    {
                        node.OutputTasks.Add(secondTask);
                    }
                }
                if (node.OutputTasks.Count != 0)
                    Nodes.Add(node);
            }
            List<Node> unity = new List<Node>();
            for (int i = 0; i < Nodes.Count; i++)
            {
                for (int j = 0; j < Nodes.Count; j++)
                {
                    if (i != j)
                        Nodes[i] = UniteNodes(Nodes[i], Nodes[j], unity);
                }
            }
            if(unity.Count>0)
            {
                Nodes = unity;
                unity = null;
            }
            List<Node> checking = new List<Node>();
            foreach (var node in Nodes)
            {
                checking.Add(node);
                CheckNode(node, checking);
            }
            Nodes = checking;
            checking = new List<Node>();
            foreach (var node in Nodes)
            {
                if (checking.Count(x => x == node) < 1)
                    checking.Add(node);
            }
            Nodes = checking;
            checking = null;
            GetInputOutputNodes(); // Работает немного неправильно, для нескольких параллельных задач, необходимо протестить
            return;
        }

        // Объединяет найденные узлы и изменяет их
        Node UniteNodes(Node first, Node second, List<Node> nodes)
        {
            if (first.OutputTasks.All(x => second.OutputTasks.Contains(x)))
            {
                if (second.OutputTasks.All(x => first.OutputTasks.Contains(x)))
                {
                    UniteInputTasks(first.InputTasks, second.InputTasks);
                    if (first == second)
                    {
                        nodes.Add(first);
                        if (nodes.Count(x => x == first) > 1)
                        {
                            nodes.RemoveAll(x => x == first);
                            nodes.Add(first);
                        }
                        return first;
                    }
                    goto EndOfUnion;
                }
                UniteInputTasks(second.InputTasks, first.InputTasks);
                goto EndOfUnion;
            }
            if (second.OutputTasks.All(x => first.OutputTasks.Contains(x)))
            {
                UniteInputTasks(first.InputTasks, second.InputTasks);
                goto EndOfUnion;
            }
            EndOfUnion:
            nodes.Add(first);
            nodes.Add(second);
            if (nodes.Count(x => x == first) > 1)
            {
                nodes.RemoveAll(x => x == first);
                nodes.Add(first);
            }
            if (nodes.Count(x => x == second) > 1)
            {
                nodes.RemoveAll(x => x == second);
                nodes.Add(second);
            }
            return first;
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
        void CheckNode(Node node, List<Node> nodes)
        {
            foreach (var first in node.OutputTasks)
            {
                foreach (var second in node.OutputTasks)
                {
                    //if (Matrix[first, second] != "#" || Matrix[second, first] != "#")
                    if (Matrix[first, second] == "||")
                    {
                        string[] tasks = new string[node.OutputTasks.Count];
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
                        nodes.Remove(node);
                        nodes.Add(firstNode);
                        nodes.Add(secondNode);
                        CheckNode(firstNode, nodes);
                        CheckNode(secondNode, nodes);
                        goto EndOfChecking;
                    }
                }
            }
        EndOfChecking:
            return;
        }

        // Создает начальные и конечные узлы
        void GetInputOutputNodes()
        {
            List<Node> inputNodes = new List<Node>()
            { new Node()
            {
                InputTasks = null,
                OutputTasks = FirstTasks
            }
            };
            List<Node> outputNodes = new List<Node>()
            { new Node()
            {
                OutputTasks = LastTasks,
                InputTasks = null
            }
            };
            CheckNode(outputNodes[0], outputNodes);
            CheckNode(inputNodes[0], inputNodes);
            if (inputNodes.Count > 1)
            {
                isSplitNeeded = true;
                foreach (var node in inputNodes)
                {
                    node.InputTasks = new List<string>() { "AND-split" };
                    Nodes.Add(node);
                }
                Nodes.Add(new Node()
                {
                    InputTasks = null,
                    OutputTasks = new List<string> { "AND-split" }
                });
            }
            else
                Nodes.Add(inputNodes[0]);
            if (outputNodes.Count > 1)
            {
                isJoinNeeded = true;
                foreach (var node in outputNodes)
                {
                    node.InputTasks = node.OutputTasks;
                    node.OutputTasks = new List<string> { "AND-join" };
                    Nodes.Add(node);
                }
                Nodes.Add(new Node()
                {
                    InputTasks = new List<string> { "AND-join" },
                    OutputTasks = null
                });
            }
            else
            {
                outputNodes[0].InputTasks = outputNodes[0].OutputTasks;
                outputNodes[0].OutputTasks = null;
                Nodes.Add(outputNodes[0]);
            }
            return;
        }
    }
}
