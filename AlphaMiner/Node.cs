using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlphaMiner
{
    internal class Node
    {
        internal Node()
        {
            InputTasks = new List<string>();
            OutputTasks = new List<string>();
        }

        internal List<string> InputTasks { get; set; }

        internal List<string> OutputTasks { get; set; }

        internal string Vertex { get; set; }

        public static bool operator ==(Node first, Node second)
        {
            if (first.InputTasks.Except(second.InputTasks).Count() == 0 && first.OutputTasks.Except(second.OutputTasks).Count() == 0)
                return true;
            else
                return false;
        }

        public static bool operator !=(Node first, Node second)
        {
            return !(first == second);
        }

        public static bool operator >=(Node first, Node second)
        {
            if (second.InputTasks.All(x => first.InputTasks.Contains(x)))
            {
                if (second.OutputTasks.All(x => first.OutputTasks.Contains(x)))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool operator <=(Node first, Node second)
        {
            return second >= first;
        }
    }
}
