using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingsland.PiFaceSharp.Spi;
using System.Threading;
using System.Timers;
using piServer;

namespace RaspClient
{
    class PhysicalIO
    {
        static PiFaceDevice raspPi;
        static System.Timers.Timer scanPulse;
        static bool[] lastState = new bool[8];
        
        public static bool initialize()
        {
            try
            {
                raspPi = new PiFaceDevice();
                for (int i = 0; i < 8; i++)
                {
                    raspPi.SetOutputPinState((byte)i, true);
                    Thread.Sleep(10);
                    raspPi.SetOutputPinState((byte)i, false);
                }

                for (int i = 0; i < 8; i++)
                {
                    raspPi.GetInputPinState((byte)i);
                }
                Logger.debug("No Errors occured");
            }
            catch (Exception e)
            {
                Logger.log(e);
                return false;
            }

            scanPulse = new System.Timers.Timer(1);
            scanPulse.Elapsed += new ElapsedEventHandler(getIn);
            scanPulse.Start();
            Logger.debug("testpulse started");
            return true;
        }
        public static void sendAllInputs()
        {
            if (raspPi != null)
            {
                Logger.debug("sending all input informations");
                for (int i = 0; i < 8; i++)
                {
                    ProtocolBuilder.buildPinEventMessage(i + 1, raspPi.GetInputPinState((byte)i));
                }
            }
            else
            {
                Logger.error("Piface not initialized");
            }
        }
        public static void setOut(byte channel, bool state)
        {
            if (raspPi != null)
            {
                raspPi.SetOutputPinState(channel, state);
                Logger.debug("Pin " + channel + " set to " + state);
            }
            else
            {
                Logger.error("Piface not initialized");
            }
        }
        private static void getIn(object source, ElapsedEventArgs e)
        {
            for (int i = 0; i < 8; i++)
            {
                bool curState;
                if ((curState = raspPi.GetInputPinState((byte)i)) != lastState[i])
                {
                    Logger.debug("input " + i + " changed");
                    lastState[i] = curState;
                    ProtocolBuilder.buildPinEventMessage(i, lastState[i]);
                }
            }
        }
    }
}
