using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventLogConnector;


namespace AlphaMiner
{
    public class Alpha
    {
        /// <summary>
        /// Конструктор алгоритма, инициализирует ссылку на обрабатываемый лог и на
        /// каталог для сохранения графа в формате .DOT
        /// </summary>
        /// <param name="pathOfLog">Ссылка на лог</param>
        /// <param name="pathOfWrite">Ссылка на каталог для сохранения файла</param>
        public Alpha(string pathOfLog, string pathOfWrite)
        {
            PathOfLog = pathOfLog;
            PathOfWrite = pathOfWrite;            
        }

        bool isJoinNeeded = false;

        bool isSplitNeeded = false;
        /// <summary>
        /// Ссылка на обрабатываемый лог
        /// </summary>
        public string PathOfLog { get; set; }
        /// <summary>
        /// Ссылка на каталог для записи графа
        /// </summary>
        public string PathOfWrite { get; set; }

        List<string> FirstTasks { get; set; }

        List<string> LastTasks { get; set; }

        internal List<string> AllTasks { get; set; }

        AdjacencyMatrix Matrix { get; set; }

        List<Node> Nodes { get; set; }

        EventLogConnector.EvLog EventLog { get; set; }

        DotWriter Writer { get; set; }

        bool IsInLog = false; // Not needed in release
                
        public void StartAlpha(bool isInLog)
        {
            FirstTasks = new List<string>();
            LastTasks = new List<string>();
            AllTasks = new List<string>();
            Nodes = new List<Node>();
            //TODO: WARNING!!! - алгоритм UniteNodes работает НЕПРАВИЛЬНО, т.к. необходимо создавать НОВЫЕ Node,
            //      а не обрабатывать старые!!!!!!!!!!!!!!!!!!!!!!!!!
            //      0) Создать ГРАМОТНЫЙ алгоритм по созданию НАЧАЛЬНЫХ и КОНЕЧНЫХ Node,
            //      проверка на тесте (abcde, baced, c), а также необходимо создавать несколько AND-Split и AND-Join
            //      1) Написать XML-комментарии для каждого метода и свойства в программе - В процессе
            int num;
            this.IsInLog = isInLog;
            if (isInLog)
            {
                EventLog = new EvLog(PathOfLog);
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
            
            if (IsInLog)
            {
                Activity[] activities = EventLog.qryl_get_activities();
                foreach (var act in activities)
                {
                    if (AllTasks.Contains(act.Name))
                        throw new WrongFormatOfEventsException($"Некорректный набор встреающихся задач: " +
                            $"задача {act.Name} встретилась дважды!");
                    AllTasks.Add(act.Name);
                }
                return;
            }
            
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
            
            if (num == -1)
            {
                Trace[] traces = EventLog.FindEventsForAllTraces();
                foreach (var trace in traces)
                {
                    try
                    {
                        List<string> act = new List<string>();
                        foreach (var activity in trace.traceEvents)
                        {
                            act.Add(activity.activity.Name);
                        }
                        GetRelationships(act.ToArray());
                    }
                    catch
                    {
                        throw new WrongFormatOfTraceException($"Получен неверный формат трейса или обнаружена" +
                            $" необъявленная задача! Номер трейса: {Array.IndexOf(traces, trace)}");
                    }
                }
            }
            
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
            var firstNodes = GetInput();
            var secondNodes = GetOutput();            
            
            List<Node> checking = new List<Node>();
            foreach (var node in firstNodes)
            {
                checking.Add(node);
                CheckOutputOfNode(node, checking);
            }
            firstNodes = checking;
            checking = new List<Node>();
            foreach (var node in secondNodes)
            {
                checking.Add(node);
                CheckInputOfNode(node, checking);
            }
            secondNodes = checking;
            Nodes = UniteNodes(firstNodes, secondNodes);
            GetInputOutputNodes(); // Работает правильно (вроде)            
            return;
        }

        List<Node> GetInput()
        {
            var firstNodes = new List<Node>();
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
                    firstNodes.Add(node);
            }
            return firstNodes;
        }

        List<Node> GetOutput()
        {
            var secondNodes = new List<Node>();
            foreach (var firstTask in AllTasks)
            {
                var node = new Node();
                node.OutputTasks.Add(firstTask);

                foreach (var secondTask in AllTasks)
                {
                    if (Matrix[secondTask, firstTask] == ">")
                    {
                        node.InputTasks.Add(secondTask);
                    }
                }
                if (node.InputTasks.Count != 0)
                    secondNodes.Add(node);
            }
            return secondNodes;
        }

        // РАБОТАЕТ НЕПРАВИЛЬНО!!!!!!!!!!!!!!!!!!!!!
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

        // СРОЧНО ПЕРЕПИСАТЬ!!!!!!!!!!!!!!!!!!!!!!!!!
        void UniteInputTasks(List<string> first, List<string> second)
        {
            foreach (var firstTask in first)
            {
                bool flag = true;
                foreach (var secondTask in second)
                {
                    if (Matrix[firstTask, secondTask] != "#" || Matrix[secondTask, firstTask] != "#")
                    //if (Matrix[firstTask, secondTask] == "||")
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
        void CheckOutputOfNode(Node node, List<Node> nodes)
        {
            foreach (var first in node.OutputTasks)
            {
                foreach (var second in node.OutputTasks)
                {
                    if (Matrix[first, second] != "#" || Matrix[second, first] != "#")
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
                        CheckOutputOfNode(firstNode, nodes);
                        CheckOutputOfNode(secondNode, nodes);
                        goto EndOfChecking;
                    }
                }
            }
        EndOfChecking:
            return;
        }

        void CheckInputOfNode(Node node, List<Node> nodes)
        {
            foreach (var first in node.InputTasks)
            {
                foreach (var second in node.InputTasks)
                {
                    if (Matrix[first, second] != "#" || Matrix[second, first] != "#")
                    {
                        string[] tasks = new string[node.InputTasks.Count];
                        node.InputTasks.CopyTo(tasks);
                        var firstList = new List<string>(tasks);
                        var secondList = new List<string>(tasks);
                        firstList.Remove(first);
                        secondList.Remove(second);
                        var firstNode = new Node()
                        {
                            InputTasks = firstList,
                            OutputTasks = node.OutputTasks
                        };
                        var secondNode = new Node()
                        {
                            InputTasks = secondList,
                            OutputTasks = node.OutputTasks
                        };
                        nodes.Remove(node);
                        nodes.Add(firstNode);
                        nodes.Add(secondNode);
                        CheckInputOfNode(firstNode, nodes);
                        CheckInputOfNode(secondNode, nodes);
                        goto EndOfChecking;
                    }
                }
            }
        EndOfChecking:
            return;
        }

        List<Node> UniteNodes(List<Node> input, List<Node> output)
        {
            var nodes = new List<Node>();
            foreach (var first in input)
            {
                foreach (var second in output)
                {
                    if (second.InputTasks.Contains(first.InputTasks[0]))
                    {
                        var node = new Node();
                        node.OutputTasks = new List<string>(first.OutputTasks);
                        node.InputTasks = new List<string>(first.InputTasks);
                        foreach (var task in second.InputTasks)
                        {
                            if (first.OutputTasks.All(x => Matrix[task, x] == ">") && !node.InputTasks.Contains(task))
                                node.InputTasks.Add(task);
                        }
                        nodes.Add(node);
                    }
                    if (first.OutputTasks.Contains(second.OutputTasks[0]))
                    {
                        var node = new Node();
                        node.OutputTasks = new List<string>(second.OutputTasks);
                        node.InputTasks = new List<string>(second.InputTasks);
                        foreach (var task in first.OutputTasks)
                        {
                            if (second.InputTasks.All(x => Matrix[x, task] == ">") && !node.OutputTasks.Contains(task))
                                node.OutputTasks.Add(task);
                        }
                        nodes.Add(node);
                    }
                }
            }
            var result = new List<Node>(nodes);
            // До данного момента работает правильно, переделать проверку!!!
            foreach (var node in nodes)
            {
                if (result.Any(x => node <= x))
                    result.Remove(node);
            }
            return result;
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
            CheckOutputOfNode(outputNodes[0], outputNodes);
            CheckOutputOfNode(inputNodes[0], inputNodes);
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

/*
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
                    firstNodes.Add(node);
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
            if (unity.Count > 0)
            {
                Nodes = unity;
                unity = null;
            }



            checking = new List<Node>();
            foreach (var node in Nodes)
            {
                if (checking.Count(x => x == node) < 1)
                    checking.Add(node);
            }
            Nodes = checking;
            checking = null;
            */
