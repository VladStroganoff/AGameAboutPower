using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NetEntity
{

}


[System.Serializable]
public class NetAnimator 
{
    public NetAnimationParam[] Parameters;
}

[System.Serializable]
public class NetAnimationParam
{
    public string Name;
}

[System.Serializable]
public class IntigerParam : NetAnimationParam
{
    public int State;
}

[System.Serializable]
public class FloatParam : NetAnimationParam
{
    public float State;
}

[System.Serializable]
public class BoolParam : NetAnimationParam
{
    public bool State;
}

[System.Serializable]
public class TriggerParam : NetAnimationParam
{
    public bool State;
}


