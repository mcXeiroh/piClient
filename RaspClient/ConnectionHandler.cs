using piServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RaspClient
{
    class ConnectionHandler
    {
        static TcpListener listener;
        static TcpClient client;
        public static Thread session;
        static NetworkStream netstream;
        const int port = 11000;
        static int id;

        public static void initialize()
        {
            //Initialize listener
            try
            {
                listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                Logger.info("Server startet, listening on port " + port);
                session = new Thread(StartSession);
                session.Start();
            }
            catch (SocketException se)
            {
                Logger.error(se.Message);
                Environment.Exit(se.ErrorCode);
            }


        }

        private static void StartSession()
        {
            while (true)
            {
                Logger.debug("accepting client");
                client = listener.AcceptTcpClient();
                netstream = client.GetStream();
                Logger.info("client connected");

                try
                {
                    id = Convert.ToInt32(NextMsg());
                    Logger.info("Id is " + id);
                    PhysicalIO.sendAllInputs();
                }
                catch
                {
                    Logger.warn("protocol syntax error, invalid id");
                }

                while (client.TestConnection())
                {
                    ProtocolBuilder.interpretMessage(NextMsg());
                }
            }
        }

        private static string NextMsg()
        {
            StreamReader sr = new StreamReader(netstream);
            Logger.debug("awaiting message");
            string temp;
            try
            {
                temp = sr.ReadLineSingleBreak();
            }
            catch
            {
                Logger.warn("connection closed unexpected");
                return null;
            }
            if(temp != null) Logger.debug("recieved message: " + temp);
            return temp;
        }

        public static void SendMsg(string msg)
        {
            Logger.debug("trying to send message");
            if (client.TestConnection())
            {
                Logger.debug("writing msg to stream");
                StreamWriter sw = new StreamWriter(netstream);
                sw.AutoFlush = true;
                sw.WriteLine(msg);
            }
            else
            {
                Logger.warn("Socket is not connected");
            }
        }
    }
    public static class StreamReaderExtensions // additional function for reading a line because the original readline-function doesn't works properly
    {
        public static string ReadLineSingleBreak(this StreamReader self)
        {
            StringBuilder currentLine = new StringBuilder();
            int i;
            char c;
            while ((i = self.Read()) >= 0)
            {
                c = (char)i;
                if (c == '\r'
                    || c == '\n')
                {
                    break;
                }

                currentLine.Append(c);
            }

            return currentLine.ToString();
        }
    }
    public static class TcpClientExtensions // simple function to test if the client is still connected
    {
        public static bool TestConnection(this TcpClient c) 
        {
            bool part1 = c.Client.Poll(1000, SelectMode.SelectRead);
            bool part2 = (c.Available == 0);
            if (part1 && part2)
                return false;
            else
                return true;
        }
        
    }
}
