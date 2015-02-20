using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RaspClient
{
    class ConnectionHandler
    {
        static TcpListener listener;
        static TcpClient c;
        const int port = 11000;

        public static void initialize()
        {
            //Initialize listener
            try
            {
                listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                AcceptNext();
                Logger.log(ERRORLEVEL.INFO, "Server startet, listening on port " + port);
            }
            catch (SocketException se)
            {
                Logger.log(se);
                Environment.Exit(se.ErrorCode);
            }
        }

        public static void AcceptNext()
        {
            Task task = AcceptClient();
        }

        private static async Task AcceptClient()
        {
            try 
            {
                c = await listener.AcceptTcpClientAsync();
            }
            catch (Exception e)
            {
                Logger.log(e);
            }

        }

        public bool SocketConnected(TcpClient c)
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
