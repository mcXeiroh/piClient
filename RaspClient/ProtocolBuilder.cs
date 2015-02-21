using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspClient
{
    class ProtocolBuilder
    {
        public static void buildMessage(int pin, bool state)
        {
            //TODO
        }

        public static void interpretMessage(byte[] encryptedMessage)
        {
            string decrypted = Encoding.UTF8.GetString(encryptedMessage);
            //TODO
        }
    }
}
