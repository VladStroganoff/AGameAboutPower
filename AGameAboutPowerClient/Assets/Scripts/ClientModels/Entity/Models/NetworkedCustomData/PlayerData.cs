

[System.Serializable]
public class PlayerData : NetworkedCustomData
{
    public string Name = "";

    public PlayerData(string name)
    {
        Name = name;
    }

}