using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuitGame : MonoBehaviour {
	
	void Update () {
	    if (Input.GetKeyDown(KeyCode.Escape))
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
            Application.Quit();
        }
	}
}
