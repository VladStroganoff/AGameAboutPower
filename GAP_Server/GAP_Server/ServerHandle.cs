using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

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

        public static void PlayerMovement(int fromClient, Packet packet)
        {
            bool[] inputs = new bool[packet.ReadInt()];


            for (int i = 0; i < inputs.Length; i++)
            {
                inputs[i] = packet.ReadBool();
            }


            Quaternion rotation = packet.ReadQuaternion();

            Server.clients[fromClient].player.SetInput(inputs, rotation);
        }
    }
}
