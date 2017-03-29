using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IWater {
    void Reset();
    void Splash(Texture splash, float power, Vector3 position, float size);
    float SampleWater(Vector3 position);
    Vector2 SampleWaterGradient(Vector3 position, float dist);
}
