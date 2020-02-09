using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.World
{

    public class WorldController
    {
        public static WorldController instance = new WorldController();

        WorldModel Model { get; set; } = new WorldModel();

        public void AddPlayerToWorld(PlayerData newPlayer)
        {
            if(!Model.Players.ContainsKey(newPlayer.ConnectionID))
            {
                ClientManager.PlayerUpdate(newPlayer);
                Model.Players.Add(newPlayer.ConnectionID, newPlayer);
            }
            else
            {
                ClientManager.PlayerUpdate(newPlayer);
                Model.Players[newPlayer.ConnectionID] = newPlayer;
            }
        }

        public void UpdatePlayerInWorld(PlayerData newPlayer)
        {
            if (!Model.Players.ContainsKey(newPlayer.ConnectionID))
            {
                ClientManager.PlayerUpdate(newPlayer);
                Model.Players.Add(newPlayer.ConnectionID, newPlayer);
            }
            else
            {
                ClientManager.PlayerUpdate(newPlayer);
                Model.Players[newPlayer.ConnectionID] = newPlayer;
            }
        }


    }
}
