
using System.Collections.Generic;
using UnityEngine;


public class NetAnimator : NetComponent
{
    public NetAnimatorComponent[] Parameters; 
}




[System.Serializable]
public class NetAnimatorComponent
{

}
[System.Serializable]
public class NetAnimatorBool : NetAnimatorComponent
{
    public string name;
    public bool state;
}
[System.Serializable]
public class NetAnomatorFloat : NetAnimatorComponent
{
    public string name;
    public float value;
}