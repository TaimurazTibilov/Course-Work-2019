using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlphaMiner
{
    internal class AdjacencyMatrix
    {
        /// <summary>
        /// Здесь создается матрица смежности для всех Event, по умолчанию заполняется "#"
        /// </summary>
        /// <param name="allTasks">Список всех задач лога</param>
        internal AdjacencyMatrix(List<string> allTasks)
        {
            AllTasks = allTasks;
            Matrix = new string[AllTasks.Count, AllTasks.Count];
            for (int i = 0; i < AllTasks.Count; i++)
            {
                for (int j = 0; j < AllTasks.Count; j++)
                {
                    Matrix[i, j] = "#";
                }
            }
        }

        /// <summary>
        /// Список всех задач лога
        /// </summary>
        List<string> AllTasks { get; set; }

        /// <summary>
        /// Матрица зависимостей задач
        /// </summary>
        string[,] Matrix { get; set; }

        /// <summary>
        /// Индексатор, с помощью него можно  получить информацию об отношении 2-х Event
        /// </summary>
        /// <param name="i">i-я задача</param>
        /// <param name="j">j-я задача</param>
        /// <returns></returns>
        internal string this[string i, string j]
        {
            get { return Matrix[AllTasks.IndexOf(i), AllTasks.IndexOf(j)]; }
            set { Matrix[AllTasks.IndexOf(i), AllTasks.IndexOf(j)] = value; }
        }
    }
}
