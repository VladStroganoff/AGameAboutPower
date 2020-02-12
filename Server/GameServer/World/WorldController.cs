using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Player;

namespace GameServer.World
{

    public class WorldController
    {
        public static WorldController instance = new WorldController();

        public WorldModel Model { get; set; } = new WorldModel();

        public void AddPlayerToWorld(PlayerData newPlayer)
        {
            Model.Players.Add(newPlayer.ConnectionID, newPlayer);
            ClientManager.NewPlayer(newPlayer);
        }

        public void UpdatePlayerInWorld(PlayerData player)
        {
            if (Model.Players.ContainsKey(player.ConnectionID))
            {
                ClientManager.UpdatePlayer(player);
                Model.Players[player.ConnectionID] = player;
            }
        }


    }
}
