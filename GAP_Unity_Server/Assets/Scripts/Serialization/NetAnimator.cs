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
    [SerializeField]
    public string Name;
}

[System.Serializable]
public class IntigerParam : NetAnimationParam
{
    [SerializeField]
    public int State;
}

[System.Serializable]
public class FloatParam : NetAnimationParam
{
    [SerializeField]
    public float State;
}

[System.Serializable]
public class BoolParam : NetAnimationParam
{
    [SerializeField]
    public bool State;
}

[System.Serializable]
public class TriggerParam : NetAnimationParam
{
    [SerializeField]
    public bool State;
}


