using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication10
{
    class Program
    {

       


        static void Main(string[] args)
        {
            Process currentProcess = System.Diagnostics.Process.GetCurrentProcess();
            long totalBytesOfMemoryUsed = currentProcess.WorkingSet64;

            Console.Write(( totalBytesOfMemoryUsed/1024)/1024);
            Console.ReadKey();

        }
    }
}
