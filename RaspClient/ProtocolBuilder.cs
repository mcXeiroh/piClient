using piServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspClient
{
    class ProtocolBuilder
    {
        public static void buildPinEventMessage(int pin, bool state)
        {
            ConnectionHandler.SendMsg("i " + (pin + 1) + " " + state.ToString());
        }
        
        public static void interpretMessage(string msg)
        {
            if (msg == null) return;
            string[] splitMsg = msg.Split(' ');
            
            switch (splitMsg[0])
            {
                case "o":
                    {
                        Logger.debug("message recognized as output instruction");
                        try
                        {
                            PhysicalIO.setOut(Convert.ToByte(splitMsg[1]), (splitMsg[2] == "True") ? true : false);
                        }
                        catch (FormatException)
                        {
                            Logger.warn("protocol syntax error");
                        }
                        break;
                    }
                default:
                    {
                        Logger.warn("protocol syntax error");
                        break;
                    }
            }
        }
    }
}
