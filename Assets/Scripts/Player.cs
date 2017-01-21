using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

    public float speed;
    public float splashingSpeed;
    public Texture splash;
    public float splashSize;

	void Start () {
	
	}
	
    public void Move(float xIn)
    {
        Quaternion rot = transform.rotation * Quaternion.AngleAxis(xIn * speed * Time.deltaTime, new Vector3(0,0,1));
        transform.rotation = rot;
    }
}
