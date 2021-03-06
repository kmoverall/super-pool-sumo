﻿using UnityEngine;
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
    Texture chargedSplashTex;
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

    [SerializeField]
    ParticleSystem splash;
    
    IWater pool;

    Quaternion startRot;
    
    [ReadOnly]
    public float splashCharge = 0;

    void Awake () 
    {
        startRot = transform.rotation;
        splashCharge = 0;
    }
    void Start ()
    {
        pool = FindObjectOfType<GameManager>().pool;
    }
	
    public void Move(float xIn)
    {
        bool isSplashing = GetComponent<Animator>().GetBool("IsSplashing");
        Quaternion rot;
        if (isSplashing)
            rot = transform.rotation * Quaternion.AngleAxis(xIn * speedWhileSplashing * Time.deltaTime, new Vector3(0, 0, 1));
        else
            rot = transform.rotation * Quaternion.AngleAxis(xIn * speed * Time.deltaTime, new Vector3(0, 0, 1));

        transform.rotation = rot;

        if (Mathf.Abs(xIn) > 0.05f)
        {
            head.localRotation = Quaternion.AngleAxis(Mathf.Sign(-xIn) * 30, new Vector3(0, 0, 1));
        }
        else
        {
            head.localRotation = Quaternion.identity;
        }

        GetComponent<Animator>().SetFloat("Speed", xIn);
    }

    public void Splash(bool isLeft)
    {
        Vector3 splashPoint = Vector3.zero;
        splashPoint = isLeft ? leftHand.position : rightHand.position;

        splashPoint += head.transform.up * 0.4f;
        pool.Splash(splashTex, splashStrength, splashPoint, splashSize);

        float splashPitch = Random.Range(0.8f, 1.2f);
        GetComponentInChildren<AudioSource>().pitch = splashPitch;
        GetComponentInChildren<AudioSource>().PlayOneShot(GetComponentInChildren<AudioSource>().clip);
        
        SplashParticles(isLeft);
    }

    public void ChargedSplash()
    {
        Vector3 splashPoint = Vector3.zero;
        splashPoint = (leftHand.position + rightHand.position) / 2;

        splashPoint += head.transform.up * 0.4f;
        pool.Splash(chargedSplashTex, splashStrength, splashPoint, splashSize);

        float splashPitch = Random.Range(0.8f, 1.2f);
        GetComponentInChildren<AudioSource>().pitch = splashPitch;
        GetComponentInChildren<AudioSource>().PlayOneShot(GetComponentInChildren<AudioSource>().clip);

        BigSplashParticles();
    }

    public void SplashParticles(bool isLeft)
    {
        Vector3 pos =  isLeft ? leftHand.position : rightHand.position;

        pos += head.transform.up * 0.4f;

        Instantiate(splash, pos, head.transform.rotation);
    }

    public void BigSplashParticles()
    {
        Vector3 pos = (leftHand.position + rightHand.position) / 2;

        pos += head.transform.up * 0.4f;

        Instantiate(splash, pos, head.transform.rotation);
    }

    public void Reset()
    {
        transform.rotation = startRot;
    }
}
