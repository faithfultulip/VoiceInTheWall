using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

[RequireComponent(typeof(WormholeAnimation),typeof(WormholeGenerator))]
public class YoutubeExample : MonoBehaviour
{
    public GameObject ring;
    public GameObject rays;

    public Texture[] Textures;
    int currentTexture = 0;
    WormholeAnimation wa;
    Color MainAlbedo = Color.blue;
    Color MainEmmision = Color.blue;
    Color AlternativeAlbedo = Color.red;
    Color AlternativeEmmision = Color.red;
    Vector4 MainDirection = new Vector4(0f, -0.05f);
    Vector4 AlternativeDirection = new Vector4(.2f, -0.1f);
    Vector4 MainTiling = new Vector4(3f, 2f);
    Vector4 AlternativeTiling = new Vector4(2f, 2f);
    public TMPro.TextMeshProUGUI bottomText;


    public void Open()
    {
        StartCoroutine(OpenWormhole());
    }

    public void Close()
    {
        StartCoroutine(CloseWormhole());

    }

    IEnumerator OpenWormhole()
    {
        WormholeGenerator generator = GetComponent<WormholeGenerator>();
        rays.SetActive(false);
        
        ring.SetActive(true);
        bottomText.text = "Opening Wormhole";
        generator.radius = 1f;
        generator.WormholeLenght = 300f;
        generator.CreateWormhole();
        rays.SetActive(true);
        yield return new WaitForSeconds(1f);
        /*for (int t = 300; t >= 150; t-=3)
        {
            generator.WormholeLenght = t;
            generator.CreateWormhole();
            yield return null;
        }
        for (int t = 150;t>=9;t--)
        {
            generator.WormholeLenght= t;
            generator.CreateWormhole();
            yield return null;
        }*/
        float r = 0;
        while (generator.WormholeLenght > 9)
        {
            
            generator.WormholeLenght = Mathf.Lerp(300, 5, r);
            generator.CreateWormhole();
            ring.transform.position = generator.transform.position + (generator.transform.up * this.GetComponent<Renderer>().bounds.size.z) + (generator.transform.up * 5);
            r += Time.deltaTime /5f;
            if (generator.WormholeLenght < 25)
            {
                GetComponent<MeshRenderer>().enabled = false;
                
                //ring.SetActive(false);
            }
            yield return null;
        }
       
        GetComponent<MeshRenderer>().enabled = false;
        rays.SetActive(true);
        yield return new WaitForSeconds(3.5f);
        GetComponent<MeshRenderer>().enabled = true;
        ring.SetActive(true);
        r = 0;
        while (generator.radius < 50)
        {
            generator.radius = Mathf.Lerp(1, 50, r);
            generator.WormholeLenght = Mathf.Lerp(30, 271, r);
            generator.CreateWormhole();
            r += Time.deltaTime / 10f;
            yield return null;
        }





        bottomText.text = "Wormhole FX";
    }

    IEnumerator CloseWormhole()
    {
        rays.SetActive(false);
        ring.SetActive(true);
        bottomText.text = "Closing Wormhole";
        yield return null;
        rays.GetComponent<ParticleSystem>().Stop();
        WormholeGenerator generator = GetComponent<WormholeGenerator>();
        float start = generator.radius;
        float t = 0;
        while (generator.radius > 0)
        {
            generator.radius = Mathf.Lerp(start, 0, t);
            generator.CreateWormhole();
            t += Time.deltaTime /2f;
            yield return null;
        }

            

        ring.GetComponent<ParticleSystem>().Stop();
        yield return new WaitForSeconds(.25f);
        ring.SetActive(false);
        bottomText.text = "Wormhole FX";
    }

    // Start is called before the first frame update
    void Start()
    {
        this.wa = this.gameObject.GetComponent<WormholeAnimation>();

        StartCoroutine(WaitGetColor());
        

    }

    public void RingEnabled(bool value)
    {
        ring.SetActive(value);
    }

    public void RaysEnabled(bool value)
    {
        rays.SetActive(value);
    }



    public void DirectionSlider(Slider slider)
    {
        SliderInfo2 si = slider.gameObject.GetComponent<SliderInfo2>();
        if (si.ColorType == WormholeAnimation.ColorType.MainAlbedo)
        {
            if (si.xy == SliderInfo2.xyz.x)
            {
                MainDirection.x = slider.value;
            }
            else
            {
                MainDirection.y = slider.value;
            }
            wa.ChangeDirection(si.ColorType, MainDirection);
        }
        else if (si.ColorType == WormholeAnimation.ColorType.AlternativeAlbedo)
        {
            if (si.xy == SliderInfo2.xyz.x)
            {
                AlternativeDirection.x = slider.value;
            }
            else
            {
                AlternativeDirection.y = slider.value;
            }
            wa.ChangeDirection(si.ColorType, AlternativeDirection);
        }

    }


    public void TilingSlider(Slider slider)
    {
        SliderInfo2 si = slider.gameObject.GetComponent<SliderInfo2>();
        if (si.ColorType == WormholeAnimation.ColorType.MainAlbedo)
        {
            if (si.xy == SliderInfo2.xyz.x)
            {
                MainTiling.x = slider.value;
            }
            else
            {
                MainTiling.y = slider.value;
            }
            wa.ChangeTiling(si.ColorType, MainTiling);
        }
        else if (si.ColorType == WormholeAnimation.ColorType.AlternativeAlbedo)
        {
            if (si.xy == SliderInfo2.xyz.x)
            {
                AlternativeTiling.x = slider.value;
            }
            else
            {
                AlternativeTiling.y = slider.value;
            }
            wa.ChangeTiling(si.ColorType, AlternativeTiling);
        }

    }


    IEnumerator WaitGetColor()
    {
        yield return new WaitForSeconds(1f);
        MainAlbedo = wa.GetColor(WormholeAnimation.ColorType.MainAlbedo);
        MainEmmision = wa.GetColor(WormholeAnimation.ColorType.MainEmmision);
        AlternativeAlbedo = wa.GetColor(WormholeAnimation.ColorType.AlternativeAlbedo);
        AlternativeEmmision = wa.GetColor(WormholeAnimation.ColorType.AlternativeEmmision);
    }


    public void ChangeTexture(int t)
    {
        if (Textures.Length > 0)
        {
            if (currentTexture >= Textures.Length)
            {
                currentTexture = 0;
            }

            wa.ChangeTexture(Textures[currentTexture], (WormholeAnimation.TextureType)t);


            currentTexture++;
        }
    }

    public void TextureSlider(float value)
    {
        wa.FadeBetweenTextures(value, 0.05f, 0f);
    }


    public void ColorSlider(Slider slider)
    {
        if (slider.GetComponent<SliderInfo>())
        {
            SliderInfo si = slider.GetComponent<SliderInfo>();
            if (si.ColorType == WormholeAnimation.ColorType.MainAlbedo)
            {
                if (si.ColorChannel == SliderInfo.ColorChannels.r)
                {
                    MainAlbedo.r = slider.value;
                }
                else if (si.ColorChannel == SliderInfo.ColorChannels.g)
                {
                    MainAlbedo.g = slider.value;
                }
                else if (si.ColorChannel == SliderInfo.ColorChannels.b)
                {
                    MainAlbedo.b = slider.value;
                }

                wa.ChangeColor(si.ColorType, MainAlbedo);
            }

            if (si.ColorType == WormholeAnimation.ColorType.MainEmmision)
            {
                if (si.ColorChannel == SliderInfo.ColorChannels.r)
                {
                    MainEmmision.r = slider.value;
                }
                else if (si.ColorChannel == SliderInfo.ColorChannels.g)
                {
                    MainEmmision.g = slider.value;
                }
                else if (si.ColorChannel == SliderInfo.ColorChannels.b)
                {
                    MainEmmision.b = slider.value;
                }

                wa.ChangeColor(si.ColorType, MainEmmision);
            }

            if (si.ColorType == WormholeAnimation.ColorType.AlternativeAlbedo)
            {
                if (si.ColorChannel == SliderInfo.ColorChannels.r)
                {
                    AlternativeAlbedo.r = slider.value;
                }
                else if (si.ColorChannel == SliderInfo.ColorChannels.g)
                {
                    AlternativeAlbedo.g = slider.value;
                }
                else if (si.ColorChannel == SliderInfo.ColorChannels.b)
                {
                    AlternativeAlbedo.b = slider.value;
                }

                wa.ChangeColor(si.ColorType, AlternativeAlbedo);
            }

            if (si.ColorType == WormholeAnimation.ColorType.AlternativeEmmision)
            {
                if (si.ColorChannel == SliderInfo.ColorChannels.r)
                {
                    AlternativeEmmision.r = slider.value;
                }
                else if (si.ColorChannel == SliderInfo.ColorChannels.g)
                {
                    AlternativeEmmision.g = slider.value;
                }
                else if (si.ColorChannel == SliderInfo.ColorChannels.b)
                {
                    AlternativeEmmision.b = slider.value;
                }

                wa.ChangeColor(si.ColorType, AlternativeEmmision);
            }





        }

    }


}
