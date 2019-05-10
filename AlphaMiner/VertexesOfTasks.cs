using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlphaMiner
{
    internal class VertexesOfTasks
    {
        int counter = 1;

        string[] vertexes;
        string[] tasks;

        /// <summary>
        /// Конструктор, создает строковое представление (уникальное обозначение)
        /// для вершин событий (задач)
        /// </summary>
        /// <param name="allTasks">Задачи (события), встречающиеся в логе</param>
        public VertexesOfTasks(List<string> allTasks)
        {
            int count = 0;
            vertexes = new string[allTasks.Count];
            tasks = new string[allTasks.Count];
            foreach (var task in allTasks)
            {
                tasks[count] = task;
                vertexes[count] = Vertex;
                count++;
            }
            Array.Sort(tasks);
        }

        /// <summary>
        /// Генератор уникального обозначения вершины
        /// </summary>
        string Vertex
        {
            get { return "B" + (counter++).ToString(); }
        }

        /// <summary>
        /// Индексатор, возвращает уникальное обозначение вершины по ее значению
        /// </summary>
        /// <param name="task">Значение вершины события (задачи)</param>
        /// <returns></returns>
        internal string this[string task]
        {
            get { return vertexes[Array.BinarySearch<string>(tasks, task)]; }
        }
    }
}
