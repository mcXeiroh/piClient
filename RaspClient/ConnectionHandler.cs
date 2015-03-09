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
        static TcpClient c;
        static Thread session;
        static NetworkStream netstream;
        const int port = 11000;

        public static void initialize()
        {
            //Initialize listener
            try
            {
                listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                Logger.info("Server startet, listening on port " + port);
                session = new Thread(startAccepting);
                session.Start();
            }
            catch (SocketException se)
            {
                Logger.error(se.Message);
                Environment.Exit(se.ErrorCode);
            }


        }

        private static void startAccepting()
        {
            while (true)
            {
                Logger.debug("accepting client");
                c = listener.AcceptTcpClient();
                netstream = c.GetStream();
                Logger.info("client connected");

                while (SocketConnected(c))
                {
                    ProtocolBuilder.interpretMessage(dataReader());
                }
            }
        }

        private static string dataReader()
        {
            StreamReader sr = new StreamReader(netstream);
            Logger.debug("awaiting message");
            string temp = sr.ReadLineSingleBreak();
            Logger.debug("recieved message: " + temp);
            return temp;
        }

        public static bool SocketConnected(TcpClient c) //simple function to test if the client is still connected
        {
            bool part1 = c.Client.Poll(1000, SelectMode.SelectRead);
            bool part2 = (c.Available == 0);
            if (part1 && part2)
                return false;
            else
                return true;
        }

        public static void sendEvent(byte[] encryptedMessage)
        {
            if (SocketConnected(c))
            {
                netstream.Write(encryptedMessage, 0, encryptedMessage.Length);
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
}
