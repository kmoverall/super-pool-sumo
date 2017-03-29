using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CPUWater : MonoBehaviour, IWater
 {

	void Awake () {
        FindObjectOfType<GameManager>().pool = this;
    }
	
	void Update () {
	
	}

    public void Reset() { }
    public void Splash(Texture splash, float power, Vector3 position, float size) { }
    public float SampleWater(Vector3 position) { return 0; }
    public Vector2 SampleWaterGradient(Vector3 position, float dist) { return Vector2.zero; }
}
