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
    GameManager manager;

    [SerializeField]
    Sprite[] sprites;

	void Awake () 
    {
        manager = FindObjectOfType<GameManager>();
	}

    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = sprites[player];   
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!enabled)
            return;

        manager.Score(this, other.GetComponent<Beachball>());
        GetComponent<Animator>().SetTrigger("Score");
        GetComponent<AudioSource>().Play();
        other.GetComponent<Beachball>().Score(_player);
    }
}
