using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICursorController
{
    CursorClick click { get; set; }
}
public delegate void CursorClick(Vector2 pos);

public class CursorController : MonoBehaviour, ICursorController
{
    public CursorClick click { get; set; }


    public void Update()
    {
        if(Input.GetMouseButton(0))
        {
            if (click != null)
                click.Invoke(Input.mousePosition);
        }

    }
}
