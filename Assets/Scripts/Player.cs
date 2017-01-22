using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

    [SerializeField]
    float speed;
    [SerializeField]
    float speedWhileSplashing;

    [SerializeField]
    Texture splashTex;
    [SerializeField]
    float splashSize;
    [SerializeField]
    float splashStrength;

    [SerializeField]
    Transform head;
    [SerializeField]
    Transform leftHand;
    [SerializeField]
    Transform rightHand;

    bool splashLeft = true;

    Water pool;

    void Awake () {
        pool = FindObjectOfType<Water>();
	}
	
    public void Move(float xIn)
    {
        Quaternion rot = transform.rotation * Quaternion.AngleAxis(xIn * speed * Time.deltaTime, new Vector3(0,0,1));
        transform.rotation = rot;

        if (Mathf.Abs(xIn) > 0.05f)
        {
            head.localRotation = Quaternion.AngleAxis(Mathf.Sign(-xIn) * 30, new Vector3(0, 0, 1));
        }
        else
        {
            head.localRotation = Quaternion.identity;
        }
    }

    public void Splash()
    {
        Vector3 splashPoint = Vector3.zero;
        splashPoint = splashLeft ? leftHand.position : rightHand.position;

        splashPoint += head.transform.up * 0.4f;
        pool.Splash(splashTex, splashStrength, splashPoint, splashSize);


        splashLeft = !splashLeft;
    }
}
