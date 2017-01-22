using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInput : MonoBehaviour {
    [SerializeField]
    string horizontalAxis;

    [SerializeField]
    string splashButton;

    [SerializeField]
    Player targetPlayer;

	void Update () {
        targetPlayer.Move(Input.GetAxisRaw(horizontalAxis));
        
        if (Input.GetButtonUp(splashButton))
        {
            targetPlayer.Splash();
        }
	}
}
