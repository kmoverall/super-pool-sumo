using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Rewired;

public class PlayerInput : MonoBehaviour {
    [SerializeField]
    Player targetPlayer;

    [SerializeField]
    int playerID = 0;
    Rewired.Player rwPlayer;

    void Awake()
    {
        rwPlayer = ReInput.players.GetPlayer(playerID);
    }

	void Update () {
        targetPlayer.Move(rwPlayer.GetAxis("Move"));

        targetPlayer.GetComponent<Animator>().SetBool("SplashLeft", rwPlayer.GetButtonDown("Splash Left"));
        targetPlayer.GetComponent<Animator>().SetBool("SplashRight", rwPlayer.GetButtonDown("Splash Right"));
        if (rwPlayer.GetButtonDown("Splash Left"))
        {
            targetPlayer.Splash(true);
        }
        else if (rwPlayer.GetButtonDown("Splash Right"))
        {
            targetPlayer.Splash(false);
        }

        /*
        IEnumerable<ControllerPollingInfo> polls = ReInput.controllers.polling.PollAllControllersForAllButtons();
        if (polls == null)
            return;
        foreach (ControllerPollingInfo p in polls)
        {
            Debug.Log(p.elementIdentifierId + " | " + p.elementIdentifierName);
        }*/
    }

    public void Reset()
    {
        targetPlayer.Reset();
    }
}
