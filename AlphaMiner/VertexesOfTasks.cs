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

        string Vertex
        {
            get { return "B" + (counter++).ToString(); }
        }

        internal string this[string task]
        {
            get { return vertexes[Array.BinarySearch<string>(tasks, task)]; }
        }
    }
}
