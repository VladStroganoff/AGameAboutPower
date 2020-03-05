
using System.Collections.Generic;
using UnityEngine;


public class NetAnimator : NetComponent
{
    public NetAnimatorComponent[] Parameters; 
}




[System.Serializable]
public class NetAnimatorComponent
{
    public string name;
}
[System.Serializable]
public class NetAnimatorBool : NetAnimatorComponent
{
    public bool state;
}
[System.Serializable]
public class NetAnimatorFloat : NetAnimatorComponent
{
    public float value;
}

[System.Serializable]
public class NetAnimatorTrigger : NetAnimatorComponent
{
    public bool state;
}

[System.Serializable]
public class NetAnimatorInt : NetAnimatorComponent
{
    public int value;
}