using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Limitless.Enums;

[Serializable]
[PostProcess(typeof(LimitlessGlitch_Glitch6Renderer), PostProcessEvent.BeforeStack, "Limitless Glitch/Glitch6", false)]
public sealed class LimitlessGlitch6 : PostProcessEffectSettings
{
    [Tooltip("Infinite - 0. Periodic- 1, Random - 2")]
    public IntervalModeParameter interval = new IntervalModeParameter { };
    [Tooltip("min/max ranom interval, if Interval = 2.")]
    public Vector2Parameter minMax = new Vector2Parameter { value = new Vector2(0.1f, 2f) };
    [Range(0f, 25f), Tooltip("Glitch periodic interval in seconds.")]
    public FloatParameter frequency = new FloatParameter { value = 1f };
    [Range(1f, 0f), Tooltip("Glitch decrease rate due time interval (0 - infinite).")]
    public FloatParameter rate = new FloatParameter { value = 1f };
    [Range(0f, 200f), Tooltip("Effect amount.")]
    public FloatParameter amount = new FloatParameter { value = 1f };
    [Range(0f, 15f), Tooltip("effect speed.")]
    public FloatParameter speed = new FloatParameter { value = 1f };
    [Space]
    [Tooltip("Time.unscaledTime .")]
    public BoolParameter unscaledTime = new BoolParameter { value = false };


    public float t;

}

public sealed class LimitlessGlitch_Glitch6Renderer : PostProcessEffectRenderer<LimitlessGlitch6>
{
    private float _time;

    private float tempVFR;
    public override void Render(PostProcessRenderContext context)
    {
        if (settings.interval.value == IntervalMode.Random)
        {
            settings.t -= Time.deltaTime;
            if (settings.t <= 0)
            {
                tempVFR = UnityEngine.Random.Range(settings.minMax.value.x, settings.minMax.value.y);
                settings.t = tempVFR;
            }
        }

        var sheet = context.propertySheets.Get(Shader.Find("LimitlessGlitch/Glitch6"));

        if (settings.unscaledTime) { _time = Time.unscaledTime; }
        else _time = Time.time;

        sheet.properties.SetFloat("time_", _time);

        sheet.EnableKeyword("VHS_JITTER_V_ON");

        if (settings.interval.value == IntervalMode.Infinite) sheet.EnableKeyword("JITTER_V_CUSTOM"); else sheet.DisableKeyword("JITTER_V_CUSTOM");
        if (settings.interval.value == IntervalMode.Random)
            sheet.properties.SetFloat("jitterVFreq", tempVFR);
        else
            sheet.properties.SetFloat("jitterVFreq", settings.frequency);

        sheet.properties.SetFloat("jitterVRate", settings.rate);

        sheet.properties.SetFloat("jitterVAmount", settings.amount);
        sheet.properties.SetFloat("jitterVSpeed", settings.speed);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
    private void ParamSwitch(PropertySheet mat, bool paramValue, string paramName)
    {
        if (paramValue) mat.EnableKeyword(paramName);
        else mat.DisableKeyword(paramName);
    }
}
