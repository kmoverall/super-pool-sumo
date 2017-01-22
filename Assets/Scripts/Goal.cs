using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Goal : MonoBehaviour {

    int _player;
    public int player 
    { 
        get { return _player; } 
        set 
        { 
            _player = value;
            GetComponent<SpriteRenderer>().sprite = sprites[player];
        }
    }
    GoalSpawner manager;

    [SerializeField]
    Sprite[] sprites;

	void Awake () 
    {
        manager = FindObjectOfType<GoalSpawner>();
	}

    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = sprites[player];   
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        manager.Score(this);
        other.GetComponent<Animator>().SetTrigger("Score");
    }
}
