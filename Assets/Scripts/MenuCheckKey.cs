using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuCheckKey : MonoBehaviour {

	void Start () {
	
	}
	
	void Update () {
	    if (Input.anyKeyDown) {
            GetComponent<Animator>().SetTrigger("Exit");
        }
	}
}
