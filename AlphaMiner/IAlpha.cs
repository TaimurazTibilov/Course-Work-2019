using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlphaMiner
{
    interface IAlpha
    {
        void StartAlpha(string nameOfGraph, string pathOfLog, string pathOfWrite, GraphFileType type);
    }
}
