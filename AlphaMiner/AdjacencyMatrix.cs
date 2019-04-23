using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlphaMiner
{
    internal class AdjacencyMatrix
    {
        //Здесь создается матрица смежности для всех Event, по умолчанию заполняется "#"
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

        List<string> AllTasks { get; set; }

        string[,] Matrix { get; set; }

        // Индексатор, с помощью него можно  получить информацию об отношении 2-х Event
        internal string this[string i, string j]
        {
            get { return Matrix[AllTasks.IndexOf(i), AllTasks.IndexOf(j)]; }
            set { Matrix[AllTasks.IndexOf(i), AllTasks.IndexOf(j)] = value; }
        }

        // Ищет задачи, происходящие параллельно друг от друга (применяется после заполнения)
        void FindParallels()
        {
            for (int i = 0; i < AllTasks.Count; i++)
            {
                for (int j = i + 1; j < AllTasks.Count; i++)
                {
                    if (Matrix[i, j] == ">" && Matrix[j, i] == ">")
                    {
                        Matrix[i, j] = "||";
                        Matrix[j, i] = "||";
                    }
                }
            }
        }
    }
}
