using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(LimitlessGlitchGlitch2Renderer), PostProcessEvent.BeforeStack, "Limitless Glitch/Glitch2", false)]
public sealed class LimitlessGlitch2 : PostProcessEffectSettings
{
    [Range(0f, 2f), Tooltip("Effect speed."), DisplayName("Speed")]
    public FloatParameter speed2 = new FloatParameter { value = 1f };
    [Range(1f, -1f), Tooltip("Effect intensity.")]
    public FloatParameter intensity = new FloatParameter { value = 1f };
    [Range(1f, 2f), Tooltip("block size (higher value = smaller blocks.")]
    public FloatParameter resolutionMultiplier = new FloatParameter { value = 1f };

    [Range(0f, 1f), Tooltip("blocks width (max value makes effect fullscreen).")]
    public FloatParameter stretchMultiplier = new FloatParameter { value = 0.88f };
}

public sealed class LimitlessGlitchGlitch2Renderer : PostProcessEffectRenderer<LimitlessGlitch2>
{
    private float T;
    private float amount = 0;
    RenderTexture _trashFrame1;
    RenderTexture _trashFrame2;
    Texture2D _noiseTexture;
    RenderTexture trashFrame;

    public override void Render(PostProcessRenderContext context)
    {
        if (_trashFrame1 != null || _trashFrame2 != null)
        {
            SetUpResources(settings.resolutionMultiplier);

        }
        if (UnityEngine.Random.value > Mathf.Lerp(0.9f, 0.5f, settings.speed2))
        {
            SetUpResources(settings.resolutionMultiplier);
            UpdateNoiseTexture(settings.resolutionMultiplier);
        }

        // Update trash frames.
        int fcount = Time.frameCount;

        if (fcount % 13 == 0) context.command.BlitFullscreenTriangle(context.source, _trashFrame1);
        if (fcount % 73 == 0) context.command.BlitFullscreenTriangle(context.source, _trashFrame2);

        trashFrame = UnityEngine.Random.value > 0.5f ? _trashFrame1 : _trashFrame2;

        var sheet = context.propertySheets.Get(Shader.Find("LimitlessGlitch/Glitch2"));
        sheet.properties.SetFloat("_Intensity", amount);
        sheet.properties.SetFloat("_ColorIntensity", settings.intensity);

        if (_noiseTexture == null)
        {
            UpdateNoiseTexture(settings.resolutionMultiplier);
        }

        sheet.properties.SetTexture("_NoiseTex", _noiseTexture);
        if (trashFrame != null)
            sheet.properties.SetTexture("_TrashTex", trashFrame);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }

    void SetUpResources(float g_2Res)
    {
        if (_trashFrame1 != null || _trashFrame2 != null)
        {
            return;
        }
        Vector2Int texVec = new Vector2Int((int)(g_2Res * 64), (int)(g_2Res * 62));
        _noiseTexture = new Texture2D(texVec.x, texVec.y, TextureFormat.ARGB32, false)
        {

            hideFlags = HideFlags.DontSave,
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Point
        };

        _trashFrame1 = new RenderTexture(Screen.width, Screen.height, 0)
        {
            hideFlags = HideFlags.DontSave
        };
        _trashFrame2 = new RenderTexture(Screen.width, Screen.height, 0)
        {
            hideFlags = HideFlags.DontSave
        };

        UpdateNoiseTexture(g_2Res);
    }
    void UpdateNoiseTexture(float g_2Res)
    {
        Color color = RandomColor();
        if (_noiseTexture == null)
        {
            Vector2Int texVec = new Vector2Int((int)(g_2Res * 64), (int)(g_2Res * 32));
            _noiseTexture = new Texture2D(texVec.x, texVec.y, TextureFormat.ARGB32, false);
        }
        for (var y = 0; y < _noiseTexture.height; y++)
        {
            for (var x = 0; x < _noiseTexture.width; x++)
            {
                if (UnityEngine.Random.value > settings.stretchMultiplier) color = RandomColor();
                _noiseTexture.SetPixel(x, y, color);
            }
        }

        _noiseTexture.Apply();
    }
    static Color RandomColor()
    {
        return new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
    }
}