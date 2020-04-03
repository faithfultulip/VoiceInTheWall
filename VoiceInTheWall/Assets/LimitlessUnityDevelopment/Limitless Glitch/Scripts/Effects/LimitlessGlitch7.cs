using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Limitless.Enums;

[Serializable]
[PostProcess(typeof(LimitlessGlitch_Glitch7Renderer), PostProcessEvent.BeforeStack, "Limitless Glitch/Glitch7", false)]
public sealed class LimitlessGlitch7 : PostProcessEffectSettings
{

    [Range(0f, 1f), Tooltip("Effect fade")]
    public FloatParameter Fade = new FloatParameter { value = 1f };
    [Range(0f, 1f), Tooltip("Effect speed.")]
    public FloatParameter Speed = new FloatParameter { value = 1f };
    [Range(0f, 10f), Tooltip("Block damage offset amount.")]
    public FloatParameter Amount = new FloatParameter { value = 1f };
    public BoolParameter unscaledTime = new BoolParameter { value = false };
}

public sealed class LimitlessGlitch_Glitch7Renderer : PostProcessEffectRenderer<LimitlessGlitch7>
{
    private float TimeX = 1.0f;


    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("LimitlessGlitch/Glitch7"));
        if (settings.unscaledTime)
            TimeX += Time.unscaledDeltaTime;
        else
            TimeX += Time.deltaTime;
        if (TimeX > 100) TimeX = 0;
        sheet.properties.SetFloat("_TimeX", TimeX * settings.Speed);

        sheet.properties.SetFloat("Offset", settings.Amount);
        sheet.properties.SetFloat("Fade", settings.Fade);
        sheet.properties.SetVector("_ScreenResolution", new Vector4(Screen.width, Screen.height, 0.0f, 0.0f));

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}