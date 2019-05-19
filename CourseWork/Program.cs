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
            Alpha alpha = new Alpha();
            alpha.StartAlpha(false, "WFnet", @"C:\Users\Таймураз\Downloads\log05.sq3", @"C:/Users/Таймураз/Desktop/output", GraphFileType.DOTAndPNG);
            //alpha.StartAlpha(true, "WFnet", @"C:\Users\Таймураз\Downloads\log05.sq3", @"C:/Users/Таймураз/Desktop/output", GraphFileType.PNG);
        }
    }
}
// C:\Users\Таймураз\Downloads\bpic15-1132.sq3
// C:\Users\Таймураз\Downloads\log05.sq3