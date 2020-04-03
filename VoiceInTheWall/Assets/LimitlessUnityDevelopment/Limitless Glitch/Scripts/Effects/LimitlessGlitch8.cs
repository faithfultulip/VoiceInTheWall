using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Limitless.Enums;

[Serializable]
[PostProcess(typeof(LimitlessGlitch_Glitch8Renderer), PostProcessEvent.BeforeStack, "Limitless Glitch/Glitch8", false)]
public sealed class LimitlessGlitch8 : PostProcessEffectSettings
{
    [Range(1f, 0f), Tooltip("Effect amount.")]
    public FloatParameter Amount = new FloatParameter { value = 0.5f };
    [Range(0.1f, 20f), Tooltip("Glitch lines width.")]
    public FloatParameter LinesWidth = new FloatParameter { value = 1f };
    [Range(0f, 1f), Tooltip("Effect speed.")]
    public FloatParameter Speed = new FloatParameter { value = 1f };
    [Range(0f, 13f), Tooltip("Offset on X axis.")]
    public FloatParameter Offset = new FloatParameter { value = 1f };
    [Range(0f, 1f), Tooltip("Effect alpha.")]
    public FloatParameter alpha = new FloatParameter { value = 1f };
    public BoolParameter unscaledTime = new BoolParameter { value = false };
}

public sealed class LimitlessGlitch_Glitch8Renderer : PostProcessEffectRenderer<LimitlessGlitch8>
{
    private float TimeX = 1.0f;


    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("LimitlessGlitch/Glitch8"));
        if (settings.unscaledTime)
            TimeX += Time.unscaledDeltaTime;
        else
            TimeX += Time.deltaTime;
        if (TimeX > 100) TimeX = 0;
        sheet.properties.SetFloat("_TimeX", TimeX * settings.Speed);
        sheet.properties.SetFloat("Amount", settings.Amount);
        sheet.properties.SetFloat("Offset", settings.Offset);
        sheet.properties.SetFloat("resM", settings.LinesWidth);
        sheet.properties.SetFloat("alpha", settings.alpha);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}