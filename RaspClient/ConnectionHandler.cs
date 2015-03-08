using piServer;
using System;
using System.Collections.Generic;
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
        static NetworkStream netstream;
        const int port = 11000;

        public static void initialize()
        {
            //Initialize listener
            try
            {
                listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                Logger.log(ERRORLEVEL.INFO, "Server startet, listening on port " + port);
            }
            catch (SocketException se)
            {
                Logger.log(se);
                Environment.Exit(se.ErrorCode);
            }
        }

        private static void startAccepting()
        {
            while (true)
            {
                c = listener.AcceptTcpClient();

                while (SocketConnected(c))
                {
                    ProtocolBuilder.interpretMessage(dataReader());
                }
            }
        }

        private static string dataReader()
        {
            byte[] rcvBuffer = new byte[512];
            StringBuilder sb = new StringBuilder();

            while (!netstream.DataAvailable) Thread.Sleep(5);

            while (netstream.Read(rcvBuffer, 0, rcvBuffer.Length) > 0)
            {
                sb.Append(Encoding.UTF8.GetString(rcvBuffer));
                rcvBuffer = new byte[512];
            }
            return sb.ToString();
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
                Logger.log(ERRORLEVEL.WARN, "Socket is not connected");
            }
        }
    }
}
