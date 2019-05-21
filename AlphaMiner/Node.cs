using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlphaMiner
{
    internal class Node
    {
        /// <summary>
        /// Конструктор узла, создает списки входных и выходных задач (событий) лога
        /// </summary>
        internal Node()
        {
            InputTasks = new List<string>();
            OutputTasks = new List<string>();
        }

        /// <summary>
        /// Входные задачи (события) лога
        /// </summary>
        internal List<string> InputTasks { get; set; }

        /// <summary>
        /// Выходные задачи (события) лога
        /// </summary>
        internal List<string> OutputTasks { get; set; }

        /// <summary>
        /// Обозначение вершины позиции для записи в формате .DOT
        /// </summary>
        internal string Vertex { get; set; }

        /// <summary>
        /// Переопределение оператора сравнения на равенство узлов
        /// </summary>
        /// <param name="first">Сравниваемый узел</param>
        /// <param name="second">Сравниваемый узел</param>
        /// <returns>Возвращает, равны ли узлы или нет</returns>
        public static bool operator ==(Node first, Node second)
        {
            if (first.InputTasks == null && second.InputTasks == null)
                if (first.OutputTasks.Except(second.OutputTasks).Count() == 0)
                    return true;
            if (first.OutputTasks == null && second.OutputTasks == null)
                if (first.InputTasks.Except(second.InputTasks).Count() == 0)
                    return true;
            if (first.OutputTasks == null || second.OutputTasks == null)
                return false;
            if (first.InputTasks == null || second.InputTasks == null)
                return false;
            if (first.InputTasks.Except(second.InputTasks).Count() == 0 && first.OutputTasks.Except(second.OutputTasks).Count() == 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Переопределение оператора сравнения на неравенство узлов
        /// </summary>
        /// <param name="first">Сравниваемый узел</param>
        /// <param name="second">Сравниваемый узел</param>
        /// <returns>Возвращает, неравны ли узлы или нет</returns>
        public static bool operator !=(Node first, Node second)
        {
            return !(first == second);
        }

        /// <summary>
        /// Переопределение оператора сравнения на вхождение узлов
        /// </summary>
        /// <param name="first">Сравниваемый узел</param>
        /// <param name="second">Сравниваемый узел</param>
        /// <returns>Возвращает, содержит ли первый узел значения второго узла</returns>
        public static bool operator >=(Node first, Node second)
        {
            return second <= first;
        }

        /// <summary>
        /// Переопределение оператора сравнения на вхождение узлов
        /// </summary>
        /// <param name="first">Сравниваемый узел</param>
        /// <param name="second">Сравниваемый узел</param>
        /// <returns>Возвращает, содержит ли второй узел значения первого узла</returns>
        public static bool operator <=(Node first, Node second)
        {
            if (first.InputTasks == null && second.InputTasks != null)
                return false;
            if (first.InputTasks != null && second.InputTasks == null)
                return false;
            if (first.OutputTasks != null && second.OutputTasks == null)
                return false;
            if (first.OutputTasks == null && second.OutputTasks != null)
                return false;
            if ((first.InputTasks == null && second.InputTasks == null) || first.InputTasks.All(x => second.InputTasks.Contains(x)))
            {
                if ((first.OutputTasks == null && second.OutputTasks == null) || first.OutputTasks.All(x => second.OutputTasks.Contains(x)))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
