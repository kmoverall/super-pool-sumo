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
    float springConstant = 0.025f;
    [SerializeField]
    float dampening = 0.025f;
    [SerializeField]
    float waveSpread = 0.5f;
    [SerializeField]
    float splashStrength = 0.1f;


    [SerializeField]
    Texture splashTex;

    [SerializeField]
    Shader waterShader;
    Material waterMat;

    Material rendererMat;
    
    Texture baseTex;

    RenderTexture waterHeight;
    RenderTexture targetHeight;
    RenderTexture waterForce;
    RenderTexture targetForce;
    RenderTexture waterVisual;
    RenderTexture targetVisual;

    Texture result;

	void Awake () 
    {
        rendererMat = GetComponent<MeshRenderer>().material;
        baseTex = rendererMat.mainTexture;
        waterMat = new Material(waterShader);
        waterMat.SetColor("_Color", waterColor);
        waterMat.SetColor("_Ambient", ambientLight);
        waterMat.SetFloat("_SpringK", springConstant);
        waterMat.SetFloat("_Dampening", dampening);
        waterMat.SetFloat("_Spread", waveSpread);

        waterHeight = new RenderTexture(baseTex.width / 4, baseTex.height / 4, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
        targetHeight = new RenderTexture(baseTex.width / 4, baseTex.height / 4, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
        waterForce = new RenderTexture(baseTex.width / 4, baseTex.height / 4, 0, RenderTextureFormat.RGFloat, RenderTextureReadWrite.Linear);
        targetForce = new RenderTexture(baseTex.width / 4, baseTex.height / 4, 0, RenderTextureFormat.RGFloat, RenderTextureReadWrite.Linear);
        waterVisual = new RenderTexture(baseTex.width, baseTex.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Default);
        targetVisual = new RenderTexture(baseTex.width, baseTex.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Default);

        Graphics.Blit(baseTex, waterHeight, waterMat, 0);
        Graphics.Blit(baseTex, waterForce, waterMat, 1);

        rendererMat.mainTexture = waterVisual;
    }

    void OnMouseDown()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Splash(splashTex, splashStrength, pos, 1f);
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
	
	void LateUpdate ()
    {
        waterMat.SetFloat("_SpringK", springConstant);
        waterMat.SetFloat("_Dampening", dampening);
        waterMat.SetFloat("_Spread", waveSpread);

        Graphics.Blit(waterHeight, targetHeight, waterMat, 0);
        waterHeight.Release();
        waterHeight = targetHeight;
        targetHeight = new RenderTexture(baseTex.width / 4, baseTex.height / 4, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);

        Graphics.Blit(waterForce, targetForce, waterMat, 1);
        waterForce.Release();
        waterForce = targetForce;
        targetForce = new RenderTexture(baseTex.width / 4, baseTex.height / 4, 0, RenderTextureFormat.RGFloat, RenderTextureReadWrite.Linear);


        Graphics.Blit(waterHeight, waterVisual, waterMat, 2);
    }

    void OnApplicationQuit()
    {
        DestroyImmediate(waterMat);
        waterHeight.Release();
        waterForce.Release();
        waterVisual.Release();
    }
}
