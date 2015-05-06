
/**
* author: Folke Gleumes on 20.02.2015
* edited by Jan
* edited again by Folke on 09.03.2015
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspClient
{
    class Logger
    {
        static StreamWriter logDatWriter;

        public static void warn(string msg)
        {
            log("WARN", msg);
        }

        public static void error(string msg)
        {
            log("ERROR", msg);
        }

        public static void debug(string msg)
        {
            if (Program.debug) log("DEBUG", msg);
        }

        public static void info(string msg)
        {
            log("INFO", msg);
        }

        private static void log(string errorLevel, string message)
        {
            //Output-Builder
            StringBuilder sb = new StringBuilder();
            {
                DateTime date = DateTime.Now;
                sb.Append("[");
                sb.Append(date.Year + "-");
                sb.Append(date.Month + "-");
                sb.Append(date.Day + " ");
                sb.Append(date.Hour + ":");
                sb.Append(date.Minute + ":");
                sb.Append(date.Second + " ");
                sb.Append(errorLevel + "] ");
                sb.Append(message);
            }
            string finalOut = sb.ToString();


            if (errorLevel == "ERROR")
            {
                ConsoleColor foreCurrent = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(finalOut);
                Console.ForegroundColor = foreCurrent;
            }
            else if (errorLevel == "WARN")
            {
                ConsoleColor foreCurrent = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(finalOut);
                Console.ForegroundColor = foreCurrent;
            }
            else if (errorLevel == "DEBUG")
            {
                ConsoleColor foreCurrent = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(finalOut);
                Console.ForegroundColor = foreCurrent;
            }
            else
            {
                Console.WriteLine(finalOut);
            }

            logDatWriter.WriteLine(finalOut);

        }

        public static void initialize()
        {
            //Add eventhandler to save file befor program starts or crashes
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(Logger.OnProcessExit);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(Logger.OnProcessCrash);

            //Create log-directory
            string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + @"logs");
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            //Create filepath
            StringBuilder fileName = new StringBuilder();
            {
                DateTime date = DateTime.Now;
                fileName.Append(date.Year + "-");
                fileName.Append(date.Month + "-");
                fileName.Append(date.Day + " ");
                fileName.Append(date.Hour + "-");
                fileName.Append(date.Minute + "-");
                fileName.Append(date.Second + ".txt");
            }
            string filePath = Path.Combine(logDirectory, fileName.ToString());

            //Create file
            if (!File.Exists(filePath))
            {
                logDatWriter = new StreamWriter(filePath);
            }


        }

        public static void OnProcessExit(object sender, EventArgs e)
        {
            closeFile();
        }
        public static void OnProcessCrash(object source, UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception ex = (Exception)e.ExceptionObject;
                error(ex.Message);
                closeFile();
            }
            catch
            {

            }
        }

        private static void closeFile()
        {
            logDatWriter.Close();
        }
    }
}
