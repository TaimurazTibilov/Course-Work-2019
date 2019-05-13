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

        /// <summary>
        /// Указывает на необходимость дополнительного соединяющего узла в конце графа
        /// </summary>
        bool isJoinNeeded = false;

        /// <summary>
        /// Указывает на необходимость дополнительного соединяющего узла в начале графа
        /// </summary>
        bool isSplitNeeded = false;

        /// <summary>
        /// Ссылка на обрабатываемый лог
        /// </summary>
        public string PathOfLog { get; set; }

        /// <summary>
        /// Ссылка на каталог для записи графа
        /// </summary>
        public string PathOfWrite { get; set; }

        /// <summary>
        /// Список задач, встречающихся в начале "следа"
        /// </summary>
        List<string> FirstTasks { get; set; }

        /// <summary>
        /// Список задач, встречающихся в конце "следа"
        /// </summary>
        List<string> LastTasks { get; set; }

        /// <summary>
        /// Список всех задач лога
        /// </summary>
        internal List<string> AllTasks { get; set; }

        /// <summary>
        /// Матрица зависимостей задач в логе
        /// </summary>
        AdjacencyMatrix Matrix { get; set; }

        /// <summary>
        /// Список узлов будущей сети Петри
        /// </summary>
        List<Node> Nodes { get; set; }

        /// <summary>
        /// Объект сторонней библиотеки, обрабатывающей логи
        /// </summary>
        EventLogConnector.EvLog EventLog { get; set; }

        /// <summary>
        /// Объект для записи конечного графа в формате .DOT с помощью полученных узлов
        /// </summary>
        DotWriter Writer { get; set; }

        bool IsInLog = false;
        
        /// <summary>
        /// Метод запуска алгоритма для обработки лога и записи полученной сети Петри
        /// </summary>
        /// <exception cref="WrongFormatOfEventsException"></exception>
        /// <exception cref="WrongFormatOfTraceException"></exception>
        /// <exception cref="WrongFormatOfPetriPathException"></exception>
        public void StartAlpha(bool isInLog)
        {
            FirstTasks = new List<string>();
            LastTasks = new List<string>();
            AllTasks = new List<string>();
            Nodes = new List<Node>();
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
            Writer.WriteGraph(PathOfWrite);
            return;
        }

        /// <summary>
        /// Получает и записывает все Task (Event), встречающиеся в логе
        /// </summary>
        /// <exception cref="WrongFormatOfEventsException"></exception>
        void GetTasks()
        {
            
            if (IsInLog)
            {
                Activity[] activities = EventLog.QrylGetActivities();
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

        /// <summary>
        /// Обрабатывает полученный на вход Trace, составляет по нему отношение между задачами
        /// </summary>
        /// <param name="trace">Массив Trace</param>
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

        /// <summary>
        /// Получает массив элементов Trace
        /// </summary>
        void GetData(int num)
        {            
            if (num == -1)
            {
                Trace[] traces = EventLog.FindEventsForAllTraces();
                foreach (var trace in traces)
                {

                        List<string> act = new List<string>();
                        foreach (var activity in trace.traceEvents)
                        {
                            act.Add(activity.Activity.Name);
                        }
                        GetRelationships(act.ToArray());
                    try
                    {
                    }
                    catch (Exception e)
                    {
                        throw new WrongFormatOfTraceException($"Получен неверный формат трейса или обнаружена" +
                            $" необъявленная задача! Номер трейса: {Array.IndexOf(traces, trace)}" + e.Message);
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

        /// <summary>
        /// Создает узлы для их дальнейшей записи и представления в формате DOT
        /// </summary>
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
            GetInputOutputNodes();
            var result = new List<Node>(Nodes);
            foreach (var node in Nodes)
            {
                if (result.Count(x => node <= x) > 1)
                    result.Remove(node);
            }
            Nodes = result;
            return;
        }

        /// <summary>
        /// Для каждой задачи создает множество узлов задач, в которые происходит переход
        /// </summary>
        /// <returns>Возвращает множество узлов задач</returns>
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

        /// <summary>
        /// Для каждой задачи создает множество узлов задач, из которых происходит переход
        /// </summary>
        /// <returns>Возвращает множество узлов задач</returns>
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

        /// <summary>
        /// Проверяет мн-во OutputTasks на предмет несоотв. связей для узла
        /// </summary>
        /// <param name="node">Проверяемый узел</param>
        /// <param name="nodes">Список узлов, из которого взят проверяемый узел</param>
        void CheckOutputOfNode(Node node, List<Node> nodes)
        {
            // СРОЧНО!!! Здесь необходимо сделать for вместо  foreach сложности n log (n), т.е.
            // для каждой задачи и вложенный от выбранной первой задачи до конца
            foreach (var first in node.OutputTasks)
            {
                foreach (var second in node.OutputTasks)
                {
                    if (Matrix[first, second] != "#" || Matrix[second, first] != "#")
                    {
                        var firstList = new List<string>(node.OutputTasks);
                        var secondList = new List<string>(node.OutputTasks);
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

        /// <summary>
        /// Проверяет мн-во InputTasks на предмет несоотв. связей для узла
        /// </summary>
        /// <param name="node">Проверяемый узел</param>
        /// <param name="nodes">Список узлов, из которого взят проверяемый узел</param>
        void CheckInputOfNode(Node node, List<Node> nodes)
        {
            // СРОЧНО!!! Здесь необходимо сделать for вместо  foreach сложности n log (n), т.е.
            // для каждой задачи и вложенный от выбранной первой задачи до конца
            foreach (var first in node.InputTasks)
            {
                foreach (var second in node.InputTasks)
                {
                    if (Matrix[first, second] != "#" || Matrix[second, first] != "#")
                    {
                        var firstList = new List<string>(node.InputTasks);
                        var secondList = new List<string>(node.InputTasks);
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

        /// <summary>
        /// Объединяет списки узлов с разными наборами последних
        /// </summary>
        /// <param name="input">Узлы, содержащие одну задачу на входе</param>
        /// <param name="output">Узлы, содержащие одну задачу на выходе</param>
        /// <returns>Список всех конечных узлов без учета проверок и начальных/конечных узлов</returns>
        List<Node> UniteNodes(List<Node> input, List<Node> output)
        {
            var nodes = new List<Node>();
            // СРОЧНО!!! Здесь необходимо сделать for вместо  foreach сложности n log (n), т.е.
            // для каждого узла и вложенный от выбранного первого лога до конца
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
            return nodes;
        }

        /// <summary>
        /// Создает начальные и конечные узлы
        /// </summary>
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
