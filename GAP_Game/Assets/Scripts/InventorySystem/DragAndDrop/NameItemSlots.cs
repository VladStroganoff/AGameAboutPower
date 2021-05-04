#if UNITY_EDITOR

using UnityEngine;
[ExecuteInEditMode]
public class NameItemSlots : MonoBehaviour
{
    public bool nameThem;
    public string name;

    private void Update()
    {
        if(nameThem)
        {
            nameThem = false;
            int index = 1;

            foreach (Transform child in transform)
            {
                child.gameObject.name = name + index;
                index++;
            }
        }
    }
}
#endif