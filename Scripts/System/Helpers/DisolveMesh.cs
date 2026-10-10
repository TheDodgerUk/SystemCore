using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisolveMesh : MonoBehaviour
{
    private static readonly int _DissolveAmount = Shader.PropertyToID("_DissolveAmount");

    public static void Disolve(MonoBehaviour mono, MeshRenderer meshRendererRef, float time, Action callback)
    {
        _DisolveMesh(mono, meshRendererRef, time, 0f, 1f, callback);
    }

    public static void UnDisolve(MonoBehaviour mono, MeshRenderer meshRendererRef, float time, Action callback)
    {
        _DisolveMesh(mono, meshRendererRef, time, 1f, 0f, callback);
    }

    public static void _DisolveMesh(MonoBehaviour mono, MeshRenderer meshRendererRef, float time, float start, float end, Action callback)
    {
        Shader stored = meshRendererRef.material.shader;
        meshRendererRef.material.shader = Shader.Find("DissolverShader/DissolveShader");
        meshRendererRef.material.SetTexture("_DissolveMap", CalcNoise());
        mono.Create<ValueTween>(time, () =>
        {
            meshRendererRef.material.shader = stored;
            callback?.Invoke();
        })
      .Initialise(start, end, (e) =>
      {
          //Update the rotation each tick
          meshRendererRef.material.SetFloat(_DissolveAmount, e);
      });
    }


    private static Texture2D CalcNoise()
    {
        Texture2D noiseTex = new Texture2D(512, 512);
        Color[] pix = new Color[noiseTex.width * noiseTex.height];
        // For each pixel in the texture...
        float xOrg = UnityEngine.Random.Range(0.0f, 10f);
        float yOrg = UnityEngine.Random.Range(0.0f, 10f);
        float scale = UnityEngine.Random.Range(20f, 30f);

        for(int y = 0; y < noiseTex.height; y++)
        {
            for (int x = 0; x < noiseTex.width; x++)
            {
                float xCoord = (float)x / noiseTex.width  * scale + xOrg;
                float yCoord = (float)y / noiseTex.height * scale + yOrg;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                pix[(int)y * noiseTex.width + (int)x] = new Color(sample, sample, sample);
            }
        }

        // Copy the pixel data to the texture and load it into the GPU.
        noiseTex.SetPixels(pix);
        noiseTex.Apply();
        return noiseTex;
    }
}
