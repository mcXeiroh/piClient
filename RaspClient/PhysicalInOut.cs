﻿using System;
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
    class PhysicalInOut
    {
        static PiFaceDevice raspPi;
        static System.Timers.Timer testpulse;
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

                testpulse = new System.Timers.Timer(5);
                testpulse.Elapsed += new ElapsedEventHandler(checkInputs);
                testpulse.Start();

                Logger.log(ERRORLEVEL.DEBUG, "No Errors occured");
                return true;
            }
            catch (Exception e)
            {
                Logger.log(e);
                return false;
            }
        }

        static void setOut(byte channel, bool state)
        {
            if (raspPi != null)
            {
                raspPi.SetOutputPinState(channel, state);
                Logger.log(ERRORLEVEL.INFO, "Pin " + channel + " set to " + state);
            }
            else
            {
                Logger.log(ERRORLEVEL.ERROR, "Piface not initialized");
            }
        }

        static void checkInputs(object source, ElapsedEventArgs e)
        {
            for (int i = 0; i < 8; i++)
            {
                if (getIn((byte)i) != lastState[i])
                {
                    lastState[i] = getIn((byte)i);
                    ProtocolBuilder.buildMessage(i, lastState[i]);
                }
            }
        }

        static bool getIn(byte channel)
        {
            return raspPi.GetInputPinState(channel);
        }
    }
}
