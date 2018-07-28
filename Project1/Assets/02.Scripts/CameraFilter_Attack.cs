using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFilter_Attack : MonoBehaviour
{
    #region Variables
    public Shader SCShader;
    private float TimeX = 1.0f;

    private Material SCMaterial;
    [HideInInspector]
    [Range(0, 20)]
    public float Value = 6.0f;
    [Range(0, 10)]
    public float Speed = 1.0f;
    [Range(0, 1)]
    public float Wavy = 1f;
    [Range(0, 1)]
    public float Wave = 0f;
    [Range(0, 1)]
    public float Fade = 1.0f;
    #endregion

    #region Properties
    Material material
    {
        get
        {
            if (SCMaterial == null)
            {
                SCMaterial = new Material(SCShader);
                SCMaterial.hideFlags = HideFlags.HideAndDontSave;
            }
            return SCMaterial;
        }
    }
    #endregion

    void Start()
    {
        SCShader = Shader.Find("Custom/SAttack");

        if (!SystemInfo.supportsImageEffects)
        {
            enabled = false;
            return;
        }
    }

    void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
    {
        if (SCShader != null)
        {
            TimeX += Time.deltaTime;
            if (TimeX > 100) TimeX = 0;
            material.SetFloat("_TimeX", TimeX);
            material.SetFloat("_Value", Value);
            material.SetFloat("_Speed", Speed);
            material.SetFloat("_Wave", Wave);
            material.SetFloat("_Wavy", Wavy);
            material.SetFloat("_Fade", Fade);

            material.SetVector("_ScreenResolution", new Vector4(sourceTexture.width, sourceTexture.height, 0.0f, 0.0f));
            Graphics.Blit(sourceTexture, destTexture, material);
        }
        else
        {
            Graphics.Blit(sourceTexture, destTexture);
        }
    }

    void OnDisable()
    {
        if (SCMaterial)
            DestroyImmediate(SCMaterial);
    }
}
