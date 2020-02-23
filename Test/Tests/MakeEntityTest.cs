using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Test.Tests
{
    [TestClass]
    class MakeEntityTest
    {

        [TestMethod]
        public void TestMake()
        {
            NetEntity enitiy = new NetEntity();

            enitiy.ConnectionID = 123456789;

            PlayerData data = new PlayerData();

            data.Name = "Player yall";
            data.PrefabName = "Player";

            MakeEntity.AddComponent(enitiy, data);

            GameObject obj = new GameObject();
            obj.transform.position = new Vector3(Random.RandomRange(1, 25), Random.RandomRange(1, 25), Random.RandomRange(1, 25));

            obj.name = "a name yall";
            NetTransform trans = new NetTransform();

            trans.position.x = obj.transform.position.x;
            trans.position.y = obj.transform.position.y;
            trans.position.z = obj.transform.position.z;

            MakeEntity.AddComponent(enitiy, trans);



            NetTransform trans2 = MakeEntity.GetComponent<NetTransform>(enitiy);

            PlayerData playerData = MakeEntity.GetComponent<PlayerData>(enitiy);


            Debug.Log("Did it work?");
        }
   

    }
}
