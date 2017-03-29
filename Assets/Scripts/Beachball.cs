using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Beachball : MonoBehaviour
{

    IWater pool;
    [SerializeField]
    float sampleDistance = 0.2f;
    [SerializeField]
    float forceMultiplier = 1;
    [SerializeField]
    float ballBob = 1;
    [SerializeField]
    ParticleSystem[] particles;
    [SerializeField]
    ParticleSystem splash;
    [SerializeField]
    AudioClip bounceSound;
    [SerializeField]
    float velocityVolumeScale;

    bool waitOne = true;

    void Awake()
    {
        pool = FindObjectOfType<GameManager>().pool;
    }

    void Update()
    {
        if (waitOne)
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            waitOne = false;
            return;
        }

        float scale = 1 + pool.SampleWater(transform.position) * ballBob;
        transform.localScale = new Vector3(scale, scale, scale);
    }

    void FixedUpdate()
    {
        Vector2 force = pool.SampleWaterGradient(transform.position, sampleDistance);
        GetComponent<Rigidbody2D>().AddForce(force * forceMultiplier, ForceMode2D.Force);
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        GetComponent<AudioSource>().pitch = Random.Range(0.8f, 1.2f);
        float volume = coll.relativeVelocity.magnitude * velocityVolumeScale;
        GetComponent<AudioSource>().PlayOneShot(bounceSound, volume);
    }

    public void Score(int player)
    {
        GetComponent<Animator>().SetTrigger("Score");
        particles[player].Play();
    }

    public void Splash()
    {
        Instantiate(splash, transform.position, Quaternion.identity);
        GetComponent<AudioSource>().pitch = Random.Range(1.8f, 2.2f);
        GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip);
    }
}
