using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Assets.Scripts
{
    static class ClientTCP
    {
        private static TcpClient clientSocket;
        private static NetworkStream myStream;
        private static byte[] receiveBuffer;

        public static void InitializeNetworking()
        {
            clientSocket = new TcpClient();
            clientSocket.ReceiveBufferSize = 4096;
            clientSocket.SendBufferSize = 4096;
            receiveBuffer = new byte[4096 * 2];

            clientSocket.BeginConnect("10.0.0.4", 5587, new AsyncCallback(ClientConnectCallback), clientSocket);
        }

        private static void ClientConnectCallback(IAsyncResult result)
        {
            clientSocket.EndConnect(result);
            if(clientSocket.Connected == false)
            {
                return;
            }
            else
            {
                clientSocket.NoDelay = true;
                myStream = clientSocket.GetStream();
                myStream.BeginRead(receiveBuffer, 0, 4096 * 2, ReceiveCallback, null);
            }
        }


        private static void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                int length = myStream.EndRead(result);
                if (length <= 0)
                {
                    return;
                }

                byte[] newBytes = new byte[length];

                Array.Copy(receiveBuffer, newBytes, length);

                UnityThread.executeInFixedUpdate(() =>
                {
                    ClientHandleData.HandleData(newBytes);
                });




            }
            catch(Exception)
            {
                throw;
            }
        }

        public static void SendingData(byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInt(data.GetUpperBound(0) - data.GetLowerBound(0) + 1);
            buffer.WriteBytes(data);
            myStream.BeginWrite(buffer.ToArray(), 0, buffer.ToArray().Length, null, null);
            buffer.Dispose();
        }

        public static void Disconnect()
        {
            clientSocket.Dispose();
        }

    }
}
