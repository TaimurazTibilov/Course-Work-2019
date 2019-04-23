using AlphaMiner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseWork
{
    class Program
    {
        static void Main(string[] args)
        {            
            Alpha alpha = new Alpha(@"Sample.sq3", @"C:/Users/Таймураз/Desktop/output");
            alpha.StartAlpha(false);
            //alpha.StartAlpha(true);
        }
    }
}
