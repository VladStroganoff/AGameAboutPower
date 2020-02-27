using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class FDebug : MonoBehaviour
{

    public static FDebug Log;
    bool isDown;

    float speed = 20;
    float startTime = 0f;
    float journeyLength = 0;
    Vector3 Destination;

    public RectTransform ConsoleWindow;
    public TextMeshProUGUI Text;
    string log;

    private void Awake()
    {
        Log = this;
    }

    public void Message(string msg)
    {
        log = log + Environment.NewLine + msg;
        Text.text = log;
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.F4))
        {

            if (isDown)
            {
                Destination = new Vector3(0, 0, 0);
            }
            else
            {
                Destination = new Vector3(0, 500, 0);
            }
            isDown = !isDown;
            journeyLength = Vector3.Distance(ConsoleWindow.anchoredPosition, Destination);
            StartCoroutine(Move());
        }
       
    }


    IEnumerator Move()
    {
        while (Vector3.Distance(ConsoleWindow.anchoredPosition, Destination) > 0.01f)
        {
            float distCovered = (Time.time - startTime) * speed;
            float fractionOfJourney = distCovered / journeyLength;
            ConsoleWindow.anchoredPosition = Vector3.Lerp(ConsoleWindow.anchoredPosition, Destination, fractionOfJourney);
            yield return new WaitForEndOfFrame();
        }
    }

}
