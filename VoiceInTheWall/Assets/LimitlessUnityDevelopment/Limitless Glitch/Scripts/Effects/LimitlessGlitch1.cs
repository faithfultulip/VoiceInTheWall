using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(LimitlessGlitchGlitch1Renderer), PostProcessEvent.BeforeStack, "Limitless Glitch/Glitch1", false)]
public sealed class LimitlessGlitch1 : PostProcessEffectSettings
{
    [Header("Random Seed")]
    [Range(-2f, 200f), Tooltip("seed x")]
    public FloatParameter x = new FloatParameter { value = 127.1f };
    [Range(-2f, 10002f), Tooltip("seed y")]
    public FloatParameter y = new FloatParameter { value = 43758.5453123f };
    [Range(-2f, 200f), Tooltip("seed z")]
    public FloatParameter z = new FloatParameter { value = 311.7f };
    [Space]
    [Range(0f, 2f), Tooltip("Effect amount")]
    public FloatParameter amount = new FloatParameter { value = 1f };

    [Range(0f, 4f), Tooltip("Stretch on X axes")]
    public FloatParameter stretch = new FloatParameter { value = 0.02f };
    [Range(0f, 1f), Tooltip("Effect speed.")]
    public FloatParameter speed = new FloatParameter { value = 0.5f };
    [Range(0f, 1f), Tooltip("Effect fade.")]
    public FloatParameter fade = new FloatParameter { value = 1f };
    [Space]
    [Range(-1f, 2f), Tooltip("Red.")]
    public FloatParameter rMultiplier = new FloatParameter { value = 1f };
    [Range(-1f, 2f), Tooltip("Green.")]
    public FloatParameter gMultiplier = new FloatParameter { value = 1f };
    [Range(-1f, 2f), Tooltip("Blue.")]
    public FloatParameter bMultiplier = new FloatParameter { value = 0f };
}

public sealed class LimitlessGlitchGlitch1Renderer : PostProcessEffectRenderer<LimitlessGlitch1>
{
    private float T;
    public override void Render(PostProcessRenderContext context)
    {
        T += Time.deltaTime;
        if (T > 100) T = 0;
        var sheet = context.propertySheets.Get(Shader.Find("LimitlessGlitch/Glitch1"));
        sheet.properties.SetFloat("Strength", settings.amount);

        sheet.properties.SetFloat("x", settings.x);
        sheet.properties.SetFloat("y", settings.y);
        sheet.properties.SetFloat("angleY", settings.z);
        sheet.properties.SetFloat("Stretch", settings.stretch);
        sheet.properties.SetFloat("Speed", settings.speed);

        sheet.properties.SetFloat("mR", settings.rMultiplier);
        sheet.properties.SetFloat("mG", settings.gMultiplier);
        sheet.properties.SetFloat("mB", settings.bMultiplier);

        sheet.properties.SetFloat("Fade", settings.fade);
        sheet.properties.SetFloat("T", T);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}