using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Water : MonoBehaviour 
{
    [SerializeField]
    Color waterColor;
    [SerializeField]
    Color ambientLight;
    [SerializeField]
    Texture toonRamp;

    [SerializeField]
    float springConstant = 0.025f;
    [SerializeField]
    float dampening = 0.025f;
    [SerializeField]
    float waveSpread = 0.5f;

    [SerializeField]
    Shader waterShader;
    Material waterMat;

    Material rendererMat;
    
    Texture baseTex;
    Texture2D sampleTexture;

    RenderTexture waterHeight;
    RenderTexture targetHeight;
    RenderTexture waterVisual;

	void Awake () 
    {
        rendererMat = GetComponent<MeshRenderer>().material;
        baseTex = rendererMat.mainTexture;
        waterMat = new Material(waterShader);
        waterMat.SetColor("_Color", waterColor);
        waterMat.SetColor("_Ambient", ambientLight);
        waterMat.SetTexture("_ToonRamp", toonRamp);

        waterMat.SetFloat("_SpringK", springConstant);
        waterMat.SetFloat("_Dampening", dampening);
        waterMat.SetFloat("_Spread", waveSpread);

        waterHeight = new RenderTexture(baseTex.width / 4, baseTex.height / 4, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
        targetHeight = new RenderTexture(baseTex.width / 4, baseTex.height / 4, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
        waterVisual = new RenderTexture(baseTex.width, baseTex.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Default);

        Graphics.Blit(baseTex, waterHeight, waterMat, 0);

        rendererMat.mainTexture = waterVisual;

        sampleTexture = new Texture2D(baseTex.width / 4, baseTex.height / 4, TextureFormat.RGBAFloat, false, true);
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

        Graphics.Blit(waterHeight, waterHeight, waterMat, 2);
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
        Graphics.Blit(waterHeight, targetHeight, waterMat, 0);
        waterHeight.Release();
        waterHeight = targetHeight;
        targetHeight = new RenderTexture(baseTex.width / 4, baseTex.height / 4, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);

        Graphics.Blit(waterHeight, targetHeight, waterMat, 0);
        waterHeight.Release();
        waterHeight = targetHeight;
        targetHeight = new RenderTexture(baseTex.width / 4, baseTex.height / 4, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);

        Graphics.Blit(waterHeight, waterVisual, waterMat, 1);

        RenderTexture.active = waterHeight;
        sampleTexture.ReadPixels(new Rect(0, 0, waterHeight.width, waterHeight.height), 0, 0);
        sampleTexture.Apply();
        RenderTexture.active = null;
    }

    void OnApplicationQuit()
    {
        DestroyImmediate(waterMat);
        waterHeight.Release();
        waterVisual.Release();
        targetHeight.Release();
    }
}
