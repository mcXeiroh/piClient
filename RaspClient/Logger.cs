using RaspClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace piServer
{
    /**
    * author: Folke Gleumes on 20.02.2015
    * edited by Jan
    */
    class Logger
    {
        static StreamWriter logDatWriter;

        public static void warn(string msg)
        {
            log(ERRORLEVEL.WARN, msg);
        }

        public static void error(string msg)
        {
            log(ERRORLEVEL.ERROR, msg);
        }

        public static void debug(string msg)
        {
            if (Program.debug) log(ERRORLEVEL.DEBUG, msg);
        }

        public static void info(string msg)
        {
            log(ERRORLEVEL.INFO, msg);
        }

        public static void log(string errorLevel, string message)
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

            Console.WriteLine(finalOut);
            logDatWriter.WriteLine(finalOut);

        }

        public static void initialize()
        {
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

        public static void log(Exception e)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(e.HResult);
            sb.Append(" : ");
            sb.Append(e.Message);

            log(ERRORLEVEL.ERROR, sb.ToString());
        }

        public static void closeFile()
        {
            logDatWriter.Close();
        }
    }
}
