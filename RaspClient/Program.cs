using piServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspClient
{
    class Program
    {
        public static bool debug = false;
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(Logger.OnProcessExit);
            foreach (string arg in args)
            {
                if (arg == "debug")
                {
                    debug = true;
                }
            }


            Logger.initialize();
            PhysicalInOut.initialize();
            ConnectionHandler.initialize();
            string input;
            do
            {
                input = Console.ReadLine();
            } while (input != "exit");
        }

    }
}
