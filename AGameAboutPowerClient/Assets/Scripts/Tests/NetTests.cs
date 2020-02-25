using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class NetTests
    {
        NetEntity entity;
        PlayerData playerData;
        NetTransform transform;
        NetAnimator animator;

        [Test]
        public void Entity()
        {
            entity = new NetEntity();
            Assert.That(entity != null);

            entity.ConnectionID = 123456789;
            entity.Online = true;
            Assert.That(entity.ConnectionID != null && entity.ConnectionID == 123456789);
        }

        [Test]
        public void PlayerData()
        {
            playerData = new PlayerData();
            Assert.That(playerData != null);

            playerData.Name = "New Player";
            playerData.PrefabName = "Player";

            Assert.That(playerData.Name != null && playerData.Name == "New Player");
        }

        [Test]
        public void NetTransfrom()
        {
            transform = new NetTransform();


            transform.position.x = 25.1f;
            transform.position.y = 11.548f;
            transform.position.z = 2.51f;

            transform.rotation.x = 997.32f;
            transform.rotation.y = 7.2f;
            transform.rotation.z = 57.15f;
            transform.rotation.w = 557.358f;


            Assert.That(transform != null && transform.position.x == 25.1f && transform.rotation.z == 57.15f);
        }

        [Test]
        public void NetAnimator()
        {
            animator = new NetAnimator();


            animator.CurrentState = "Bananas";

            Assert.That(animator != null && animator.CurrentState == "Bananas");

        }

        [Test]
        public void EntityComponents()
        {
            MakeEntity.AddComponent(entity, playerData);

            Assert.That(entity.Components.Length > 0);

            MakeEntity.AddComponent(entity, transform);

            Assert.That(entity.Components.Length > 1);

            MakeEntity.AddComponent(entity, animator);

            Assert.That(entity.Components.Length > 2);

            Assert.That(entity.Components[1] != null);
        }


    }
}
