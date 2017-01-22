using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Timer : MonoBehaviour {

    GameManager manager;
    [SerializeField]
    Color normalColor;
    [SerializeField]
    Color warningColor;

    [SerializeField]
    AudioClip endBuzzer;


    float lastFrac = 0;

    bool timeIsRunning = true;

	void Awake () 
    {
        manager = FindObjectOfType<GameManager>();
	}

    void Start()
    {
        GetComponent<Text>().color = normalColor;
    }
	
	void Update () 
    {

        float time = manager.timeRemaining;

        if (time > 0)
        {
            timeIsRunning = true;
            GetComponent<Text>().color = normalColor;
        }

        if (!timeIsRunning)
            return;


        string t = "";
        if (time >= 60)
        {
            int minutes = Mathf.CeilToInt(time) / 60;
            int seconds = Mathf.CeilToInt(time) % 60;
            t = minutes + ":" + seconds.ToString("00");
        }
        else
        {
            t = time.ToString("0.00");
        }

        GetComponent<Text>().text = t;

        if (time <= 0)
        {
            timeIsRunning = false;
            GetComponent<AudioSource>().PlayOneShot(endBuzzer);
            GetComponent<Text>().color = warningColor;
            return;
        }

        if (time <= 10)
        {
            if (time - Mathf.Floor(time) > lastFrac)
            {
                GetComponent<AudioSource>().Play();
            }
            GetComponent<Text>().color = Color.Lerp(normalColor, warningColor, time - Mathf.Floor(time));
            lastFrac = time - Mathf.Floor(time);
        }
    }
}
