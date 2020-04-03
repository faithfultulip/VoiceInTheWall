using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(WormholeGenerator))]
public class WormholeAnimation : MonoBehaviour
{
    #region Variables
    bool running = false;
    Material TunnelMat;
    WormholeGenerator generator;
    #endregion

    public enum TextureType
    {
        Main,Alternative,FadeOut
    }

    public enum ColorType
    {
        MainAlbedo,MainEmmision,AlternativeAlbedo,AlternativeEmmision
    }


    // Start is called before the first frame update
    void Start()
    {
        TunnelMat = this.gameObject.GetComponent<Renderer>().material;
        if (TunnelMat.shader.name == "Shader Graphs/TunnelShader")
        {
            running = true;
                       
        }
        else
        {
            Debug.LogError("If you've encountered this error, please check the manual on page2 (Getting started). Most probably you are not running in LWRP or are missing the shader.");
        }
        generator = GetComponent<WormholeGenerator>();
    }


    public void SetRadius(float radius)
    {
        generator.radius = radius;
        generator.CreateWormhole();
    }

    public void SetDepth(float wormholedepth)
    {
        generator.WormholeLenght = wormholedepth;
        generator.CreateWormhole();
    }

    public void ChangeDirection(ColorType CT, Vector4 newDirection)
    {
        if (CT == ColorType.MainAlbedo)
        {
            TunnelMat.SetVector(Shader.PropertyToID("Vector2_C1C5D942"), newDirection);
        } else if (CT == ColorType.AlternativeAlbedo)
        {
            TunnelMat.SetVector(Shader.PropertyToID("Vector2_D5F14EBA"), newDirection);
        }
    }

   


    public void ChangeTiling(ColorType CT, Vector4 newDirection)
    {
        

        if (CT == ColorType.MainAlbedo)
        {
            TunnelMat.SetVector(Shader.PropertyToID("Vector2_9641F2B7"), newDirection);
        }
        else if (CT == ColorType.AlternativeAlbedo)
        {
            TunnelMat.SetVector(Shader.PropertyToID("Vector2_CAF798E6"), newDirection);
        }
    }

    public void ChangeColor(ColorType CT,Color NewColor)
    {
        if (CT == ColorType.MainAlbedo)
        {
            TunnelMat.SetColor(Shader.PropertyToID("Color_F0AD4101"), NewColor);
        } else if (CT == ColorType.MainEmmision)
        {
            TunnelMat.SetColor(Shader.PropertyToID("Color_797D65BA"), NewColor);
        }
        else if (CT == ColorType.AlternativeAlbedo)
        {
            TunnelMat.SetColor(Shader.PropertyToID("Color_9F136BDF"), NewColor);
        }
        else if (CT == ColorType.AlternativeEmmision)
        {
            TunnelMat.SetColor(Shader.PropertyToID("Color_F9FFF621"), NewColor);
        }
    }


    public Color GetColor(ColorType CT)
    {
        if (CT == ColorType.MainAlbedo)
        {
            return TunnelMat.GetColor(Shader.PropertyToID("Color_F0AD4101"));
        }
        else if (CT == ColorType.MainEmmision)
        {
            return TunnelMat.GetColor(Shader.PropertyToID("Color_797D65BA"));
        }
        else if (CT == ColorType.AlternativeAlbedo)
        {
            return TunnelMat.GetColor(Shader.PropertyToID("Color_9F136BDF"));
        }
        else if (CT == ColorType.AlternativeEmmision)
        {
            return TunnelMat.GetColor(Shader.PropertyToID("Color_F9FFF621"));
        }
        return Color.black;
    }

    public void ChangeTexture(Texture NewTexture, TextureType TexType)
    {
        if (NewTexture != null)
        {
            if (TexType == TextureType.Main)
            {
                TunnelMat.SetTexture(Shader.PropertyToID("Texture2D_10AB37DC"), NewTexture);
            }
            else if (TexType == TextureType.Alternative)
            {
                TunnelMat.SetTexture(Shader.PropertyToID("Texture2D_4C46FF2C"), NewTexture);
            }
            else if (TexType == TextureType.FadeOut)
            {
                TunnelMat.SetTexture(Shader.PropertyToID("Texture2D_3E7116E2"), NewTexture);
            }
        }
    }


    #region TextureFade
    public void FadeBetweenTextures(float target, float step, float delay)
    {
        StartCoroutine(FadeBetweenTexturesCO(target, step, delay));
    }


    IEnumerator FadeBetweenTexturesCO(float target, float step, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        int direction = 1;
        if (target > TunnelMat.GetFloat(Shader.PropertyToID("Vector1_755340CE"))) {
            direction = 1;
        } else
        {
            direction = -1;
        }

        while (TunnelMat.GetFloat(Shader.PropertyToID("Vector1_755340CE"))*direction < target * direction)
        {
            TunnelMat.SetFloat(Shader.PropertyToID("Vector1_755340CE"), TunnelMat.GetFloat(Shader.PropertyToID("Vector1_755340CE")) + (direction * step)); 
        }


    }
    #endregion


   
}
