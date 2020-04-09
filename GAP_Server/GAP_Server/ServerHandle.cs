using System;
using System.Collections.Generic;
using System.Text;

namespace GAP_Server
{
    class ServerHandle
    {
        public static void WelcomeRecieved(int fromClient, Packet packet)
        {
            int clientIDCheck = packet.ReadInt();
            string welcomeMessage = packet.ReadString();

            Console.WriteLine("PLayer: " + welcomeMessage + " joined the server.");

            if(fromClient != clientIDCheck)
            {
                Console.WriteLine("there's a mole... at the highest level of the Circus...");
            }

            Server.clients[fromClient].SendIntoGame(welcomeMessage);

        }
    }
}
