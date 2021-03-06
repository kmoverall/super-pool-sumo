﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Water : MonoBehaviour, IWater
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
    float WaterDeadZone = 0.0001f;
    [SerializeField]
    int PropagationPasses = 8;

    [SerializeField]
    Shader waterShader;
    Material waterMat;

    Material rendererMat;
    
    Texture baseTex;
    Texture2D sampleTexture;

    RenderTexture waterHeight;
    RenderTexture tmpHeight;
    RenderTexture waterVisual;

    bool waitOneFrame = true;

	void Awake () 
    {
        FindObjectOfType<GameManager>().pool = this;
        rendererMat = GetComponent<MeshRenderer>().material;
        baseTex = rendererMat.mainTexture;
        waterMat = new Material(waterShader);
        waterMat.SetColor("_Color", waterColor);
        waterMat.SetColor("_Ambient", ambientLight);
        waterMat.SetTexture("_ToonRamp", toonRamp);

        waterMat.SetFloat("_SpringK", SpringConstant);
        waterMat.SetFloat("_Dampening", Dampening);
        waterMat.SetFloat("_Spread", WaveSpread);
        waterMat.SetFloat("_DeadZone", WaterDeadZone);

        Reset();
    }

    public void Reset()
    {
        if (waterHeight != null)
            RenderTexture.ReleaseTemporary(waterHeight);
        if (waterVisual != null)
            waterVisual.Release();
            
        waterHeight = RenderTexture.GetTemporary(baseTex.width / 8, baseTex.height / 8, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
        tmpHeight = RenderTexture.GetTemporary(baseTex.width / 8, baseTex.height / 8, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
        waterVisual = new RenderTexture(baseTex.width, baseTex.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Default);

        Graphics.Blit(baseTex, waterHeight, waterMat, 0);

        rendererMat.mainTexture = waterVisual;

        sampleTexture = new Texture2D(baseTex.width / 8, baseTex.height / 8, TextureFormat.RGBAFloat, false, true);

        waitOneFrame = true;
    }

    public void Splash(Texture splash, float power, Vector3 position, float size)
    {
        waterMat.SetTexture("_Decal", splash);
        waterMat.SetFloat("_SplashPow", power);

        Vector4 pos = Vector4.zero;
        pos.x = (position.x - GetComponent<MeshRenderer>().bounds.min.x) / GetComponent<MeshRenderer>().bounds.size.x;
        pos.y = (position.y - GetComponent<MeshRenderer>().bounds.min.y) / GetComponent<MeshRenderer>().bounds.size.y;
        waterMat.SetVector("_DecalPos", pos);

        float scl = size / GetComponent<MeshRenderer>().bounds.size.x;
        waterMat.SetFloat("_DecalScale", scl);

        Graphics.Blit(waterHeight, waterHeight, waterMat, 3);
    }

    public float SampleWater(Vector3 position)
    {
        Vector2 pos = Vector2.zero;
        pos.x = (position.x - GetComponent<MeshRenderer>().bounds.min.x) / GetComponent<MeshRenderer>().bounds.size.x;
        pos.y = (position.y - GetComponent<MeshRenderer>().bounds.min.y) / GetComponent<MeshRenderer>().bounds.size.y;

        return sampleTexture.GetPixelBilinear(pos.x, pos.y).r;
    }

    public Vector2 SampleWaterGradient(Vector3 position, float dist)
    {
        Vector2 grad = Vector2.zero;
        grad.x += SampleWater(position + new Vector3(dist, 0));
        grad.x -= SampleWater(position + new Vector3(-dist, 0));
        grad.y += SampleWater(position + new Vector3(0, dist));
        grad.y -= SampleWater(position + new Vector3(0, -dist));

        return grad;
    }

    void LateUpdate ()
    {
        if (waitOneFrame)
        {
            waitOneFrame = false;
            return;
        }
        waterMat.SetFloat("_SpringK", SpringConstant);
        waterMat.SetFloat("_Dampening", Dampening);
        waterMat.SetFloat("_Spread", WaveSpread);

        Graphics.Blit(waterHeight, tmpHeight, waterMat, 0);
        RenderTexture.ReleaseTemporary(waterHeight);
        waterHeight = tmpHeight;
        tmpHeight = RenderTexture.GetTemporary(baseTex.width / 8, baseTex.height / 8, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);

        for (int i = 0; i < PropagationPasses; i++)
        {
            Graphics.Blit(waterHeight, tmpHeight, waterMat, 1);
            RenderTexture.ReleaseTemporary(waterHeight);
            waterHeight = tmpHeight;
            tmpHeight = RenderTexture.GetTemporary(baseTex.width / 8, baseTex.height / 8, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
        }

        Graphics.Blit(waterHeight, waterVisual, waterMat, 2);
    }

    void OnRenderObject ()
    {
        RenderTexture.active = waterHeight;
        sampleTexture.ReadPixels(new Rect(0, 0, waterHeight.width, waterHeight.height), 0, 0);
        sampleTexture.Apply();
        RenderTexture.active = null;
    }

    void OnApplicationQuit()
    {
        DestroyImmediate(waterMat);
        RenderTexture.ReleaseTemporary(waterHeight);
        waterVisual.Release();
        RenderTexture.ReleaseTemporary(tmpHeight);
    }
}
