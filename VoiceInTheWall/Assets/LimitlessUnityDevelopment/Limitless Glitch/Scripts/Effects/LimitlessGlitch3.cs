using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(LimitlessGlitchGlitch3Renderer), PostProcessEvent.BeforeStack, "Limitless Glitch/Glitch3", false)]
public sealed class LimitlessGlitch3 : PostProcessEffectSettings
{
    [Range(0f, 50f), Tooltip("Effect speed.")]
    public FloatParameter speed = new FloatParameter { value = 1f };
    [ Range(0f, 505f),Tooltip("block size (higher value = smaller blocks).")]
    public FloatParameter blockSize = new FloatParameter { value = 1f };

    [Range(0f, 25f),Tooltip("maximum color shift on X axis.")]
    public FloatParameter maxOffsetX = new FloatParameter { value = 1f };
        [Range(0f, 25f),Tooltip("maximum color shift on Y axis.")]
    public FloatParameter maxOffsetY = new FloatParameter { value = 1f };
}

public sealed class LimitlessGlitchGlitch3Renderer : PostProcessEffectRenderer<LimitlessGlitch3>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("LimitlessGlitch/Glitch3"));

        sheet.properties.SetFloat("speed", settings.speed);
        sheet.properties.SetFloat("blockSize", settings.blockSize);
        sheet.properties.SetFloat("maxOffsetX", settings.maxOffsetX);
        sheet.properties.SetFloat("maxOffsetY", settings.maxOffsetY);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}