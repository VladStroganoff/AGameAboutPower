using System;
using System.Net.Sockets;
using GameServer.Player;


namespace GameServer
{
    class Client
    {
        public int ConnectionID;
        public PlayerData PlayerData = new PlayerData();
        public TcpClient Socket;
        public NetworkStream Stream;
        private byte[] receiveBuffer;
        public ByteBuffer Buffer;

        public void Start()
        {
            Socket.SendBufferSize = 4096;
            Socket.ReceiveBufferSize = 4096;
            Stream = Socket.GetStream();
            receiveBuffer = new byte[4096];
            Stream.BeginRead(receiveBuffer, 0, Socket.ReceiveBufferSize, OnReceiveData, null);
            Console.WriteLine("Incomming connection from '{0}'. ", Socket.Client.RemoteEndPoint.ToString());
        }

        private void OnReceiveData(IAsyncResult result)
        {
           
            try
            {
                int length = Stream.EndRead(result);
                if(length <=0)
                {
                    CloseConnection();
                    return;
                }

                byte[] newBytes = new byte[length];
                Array.Copy(receiveBuffer, newBytes, length);
                ServerHandleData.HandleData(ConnectionID, newBytes);
                Stream.BeginRead(receiveBuffer, 0, Socket.ReceiveBufferSize, OnReceiveData, null);
            }
            catch (Exception)
            {
                CloseConnection();
                return;
            }
        }

        private void CloseConnection()
        {
            if (ClientManager.Clients.ContainsKey(ConnectionID))
            {
                ClientManager.Clients.Remove(ConnectionID);
                Console.WriteLine("Removed player from list");
            }

            if(World.WorldController.instance.Model.Players.ContainsKey(ConnectionID))
            {
                World.WorldController.instance.Model.Players[ConnectionID].Online = false;
                ClientManager.UpdatePlayer(World.WorldController.instance.Model.Players[ConnectionID]);
            }



            Console.WriteLine("Connection from {0} has been terminated.", Socket.Client.RemoteEndPoint.ToString());
            Socket.Close();
        }

    }
}
