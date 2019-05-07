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

        public static bool operator !=(Node first, Node second)
        {
            return !(first == second);
        }

        public static bool operator >=(Node first, Node second)
        {
            return second <= first;
        }

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
