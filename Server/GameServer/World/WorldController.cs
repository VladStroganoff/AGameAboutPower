using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Entity;

namespace GameServer.World
{

    public class WorldController
    {
        public static WorldController instance = new WorldController();

        public WorldModel Model { get; set; } = new WorldModel();

        public void AddPlayerToWorld(int connectionID)
        {

            NetEntity newEntity = CreatePlayer(connectionID);
            

            Model.Players.Add(newEntity.ConnectionID, newEntity);
            ClientManager.NewPlayer(newEntity);
        }

        public void UpdatePlayerInWorld(NetEntity entity)
        {
            if (Model.Players.ContainsKey(entity.ConnectionID))
            {
                ClientManager.UpdatePlayer(entity);
                Model.Players[entity.ConnectionID] = entity;
            }
        }

        NetEntity CreatePlayer(int id)
        {
            PlayerData data = new PlayerData();
            data.Name = "Player name";
            data.PrefabName = "Player";

            NetEntity entity = new NetEntity();
            entity.ConnectionID = id;
            entity.Online = true;
            MakeEntity.AddComponent(entity, data);

            return entity;
        }

    }
}
