using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ResultsRetriever : MonoBehaviour {

    [SerializeField]
    Text P1Score;
    [SerializeField]
    Text P2Score;
    [SerializeField]
    Text Results;
    [SerializeField]
    string P1WinMessage;
    [SerializeField]
    string P2WinMessage;
    [SerializeField]
    string DrawMessage;
    [SerializeField]
    Color DrawColor;

    GameManager manager;

    void OnEnable () {
        manager = FindObjectOfType<GameManager>();

        P1Score.text = manager.scores[0].ToString();
        P2Score.text = manager.scores[1].ToString();

        if (manager.scores[0] > manager.scores[1])
        {
            Results.text = P1WinMessage;
            Results.color = P1Score.color;
        }
        else if (manager.scores[0] < manager.scores[1])
        {
            Results.text = P2WinMessage;
            Results.color = P2Score.color;
        }
        else
        {
            Results.text = DrawMessage;
            Results.color = DrawColor;
        }
    }
}
