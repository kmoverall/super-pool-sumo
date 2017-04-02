using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CPUWater : MonoBehaviour, IWater
{
    [SerializeField]
    Color waterColor;
    [SerializeField]
    Color ambientLight;
    [SerializeField]
    Texture toonRamp;

    [SerializeField]
    float SpringConstant = 0.025f;
    [SerializeField]
    float Dampening = 0.025f;
    [SerializeField]
    float WaveSpread = 0.5f;
    [SerializeField]
    int PropagationPasses = 8;

    [SerializeField]
    Texture baseTex;
    Texture2D waterData;
    Texture2D waterVis;
    Texture2D waterDeltas;

    void Awake () 
    {
        FindObjectOfType<GameManager>().pool = this;
        waterData = new Texture2D(baseTex.width / 8, baseTex.height / 8, TextureFormat.RGBAFloat, false, true);
        waterVis = new Texture2D(baseTex.width / 8, baseTex.height / 8, TextureFormat.RGBAFloat, false, true);
        waterDeltas = new Texture2D(baseTex.width / 8, baseTex.height / 8, TextureFormat.RGBAFloat, false, true);
    }
	
	void Update () 
    {
        for (int i = 0; i < waterData.width; i++)
        {
            for (int j = 0; j < waterData.height; j++)
            {
                UpdateWaterData(i, j);
            }
        }
    }

    void UpdateWaterData(int x, int y)
    {
        //data = (position, velocity, acceleration, isInsidePool)
        Color data = waterData.GetPixel(x, y);

        if (data.a == 0)
            return;

        data.b = -1 * SpringConstant * data.r - Dampening * data.g;
        data.r += data.g * Time.deltaTime;
        data.g += data.b * Time.deltaTime;

        waterData.SetPixel(x, y, data);
    }

    void UpdateWaterDeltas(int x, int y)
    {
    }

    public void Reset() 
    {
        waterData = new Texture2D(baseTex.width / 8, baseTex.height / 8, TextureFormat.RGBAFloat, false, true);
        waterVis = new Texture2D(baseTex.width / 8, baseTex.height / 8, TextureFormat.RGBAFloat, false, true);
    }
    public void Splash(Texture splash, float power, Vector3 position, float size) { }
    public float SampleWater(Vector3 position) { return 0; }
    public Vector2 SampleWaterGradient(Vector3 position, float dist) { return Vector2.zero; }
}
