﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Make 
{

    public static PlayerData NewPlyer(Transform transform, int ID, string name)
    {
        PlayerData player = new PlayerData();

        player.ConnectionID = ID;
        player.Name = name;
        player.position.x = transform.GetChild(0).position.x;
        player.position.y = transform.GetChild(0).position.y;
        player.position.z = transform.GetChild(0).position.z;

        player.rotation.x = transform.GetChild(0).rotation.x;
        player.rotation.y = transform.GetChild(0).rotation.y;
        player.rotation.z = transform.GetChild(0).rotation.z;
        player.rotation.w = transform.GetChild(0).rotation.w;

        return player;
    }

    public static PlayerData PlayerUpdate(PlayerData data, Transform transform)
    {
        data.position.x = transform.GetChild(0).position.x;
        data.position.y = transform.GetChild(0).position.y;
        data.position.z = transform.GetChild(0).position.z;

        data.rotation.x = transform.GetChild(0).rotation.x;
        data.rotation.y = transform.GetChild(0).rotation.y;
        data.rotation.z = transform.GetChild(0).rotation.z;
        data.rotation.w = transform.GetChild(0).rotation.w;

        return data;
    }

}
