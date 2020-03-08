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

                NetTransform trans = MakeEntity.GetComponent<NetTransform>(entity);
                if(trans != null)
                    Console.WriteLine("Updates Player: " + entity.ConnectionID + " position: " + trans.position.x + ", " + trans.position.y + ", " + trans.position.z);


                NetAnimator anim = MakeEntity.GetComponent<NetAnimator>(entity);
                if (anim != null)
                {
                    foreach (NetAnimatorComponent component in anim.Parameters)
                    {
                        if (component is NetAnimatorBool)
                        {
                            NetAnimatorBool boolio = component as NetAnimatorBool;
                            Console.WriteLine(boolio.name + ", " + boolio.state);
                        }
                        else if (component is NetAnimatorFloat)
                        {
                            NetAnimatorFloat floatio = component as NetAnimatorFloat;
                            Console.WriteLine(floatio.name + ", " + floatio.value);
                        }
                        else if (component is NetAnimatorInt)
                        {
                            NetAnimatorInt intelio = component as NetAnimatorInt;
                            Console.WriteLine(intelio.name + ", " + intelio.value);
                        }
                        else if (component is NetAnimatorTrigger)
                        {
                            NetAnimatorTrigger triggero = component as NetAnimatorTrigger;
                            Console.WriteLine(triggero.name + ", " + triggero.state);
                        }
                    }
                }
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
