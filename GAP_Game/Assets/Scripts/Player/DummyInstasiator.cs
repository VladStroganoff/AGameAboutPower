using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyInstasiator : MonoBehaviour
{
    public GameObject LocalPlayer;
    public Transform locatoon;
    void Start()
    {
        Instantiate(LocalPlayer, locatoon.position, locatoon.rotation);
    }

  
}
