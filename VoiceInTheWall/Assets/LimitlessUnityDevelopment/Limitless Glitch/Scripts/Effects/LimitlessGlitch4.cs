using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(LimitlessGlitch_Glitch4_Renderer), PostProcessEvent.BeforeStack, "Limitless Glitch/Glitch4", false)]
public sealed class LimitlessGlitch4 : PostProcessEffectSettings
{
    [Range(0f, 21f), Tooltip("Glitch periodic interval in seconds.")]
    public FloatParameter interval = new FloatParameter { value = 1f };
    [Range(1f, 0f), Tooltip("Glitch decrease rate due time interval (0 - infinite).")]
    public FloatParameter rate = new FloatParameter { value = 1f };
    [Range(0f, 50f), Tooltip("color shift.")]
    public FloatParameter RGBSplit = new FloatParameter { value = 1f };
    [Range(0f, 1f), Tooltip("effect speed.")]
    public FloatParameter speed = new FloatParameter { value = 1f };
    [Range(0f, 2f), Tooltip("effect amount.")]
    public FloatParameter amount = new FloatParameter { value = 1f };
    [Tooltip(" true - Enables ability to adjust resolution. false - screen resolution.")]
    public BoolParameter customResolution = new BoolParameter { value = false };
    [Tooltip("jitter resolution.")]
    public Vector2Parameter resolution = new Vector2Parameter { value = new Vector2(640f, 480f) };
}

public sealed class LimitlessGlitch_Glitch4_Renderer : PostProcessEffectRenderer<LimitlessGlitch4>
{
    public override void Render(PostProcessRenderContext context)
    {

        var sheet = context.propertySheets.Get(Shader.Find("LimitlessGlitch/Glitch4"));
        sheet.properties.SetFloat("_GlitchInterval", settings.interval);
        sheet.properties.SetFloat("_GlitchRate", settings.rate);
        sheet.properties.SetFloat("_RGBSplit", settings.RGBSplit);
        sheet.properties.SetFloat("_Speed", settings.speed);
        sheet.properties.SetFloat("_Amount", settings.amount);
        if (settings.customResolution)
            sheet.properties.SetVector("_Res", settings.resolution);
        else
            sheet.properties.SetVector("_Res", new Vector2(Screen.width, Screen.height));
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}
