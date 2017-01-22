using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioPlayer : MonoBehaviour {

    public void SetPitch(float pitch)
    {
        GetComponent<AudioSource>().pitch = pitch;
    }

    public void SetVolume(float vol)
    {
        GetComponent<AudioSource>().volume = vol;
    }

    public void PlaySound(AudioClip sound)
    {
        GetComponent<AudioSource>().PlayOneShot(sound);
    }
}
