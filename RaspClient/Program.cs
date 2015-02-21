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
            string input;
            do
            {
                input = Console.ReadLine();
            } while (input != "exit");
        }

        static void OnProcessExit(object source, EventArgs e)
        {
            Logger.closeFile();
        }
    }
}
