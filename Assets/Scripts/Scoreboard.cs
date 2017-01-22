using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Scoreboard : MonoBehaviour {

    [SerializeField]
    Text p1Score;

    [SerializeField]
    Text p2Score;

    GameManager manager;

	void OnEnable () {
        manager = FindObjectOfType<GameManager>();
	}
	
	void Update () {
        if (p1Score.text != manager.scores[0].ToString())
            GetComponent<Animator>().SetTrigger("Pop1");

        p1Score.text = manager.scores[0].ToString();
        
        if (p2Score.text != manager.scores[1].ToString())
            GetComponent<Animator>().SetTrigger("Pop2");

        p2Score.text = manager.scores[1].ToString();
    }
}
