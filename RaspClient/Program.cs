using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspClient
{
    class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);

            Logger.initialize();
            PhysicalInOut.initialize();
            ConnectionHandler.initialize();
            Console.ReadKey();
        }

        static void OnProcessExit(object sender, EventArgs e)
        {
            Logger.closeFile();
        }
    }
}
