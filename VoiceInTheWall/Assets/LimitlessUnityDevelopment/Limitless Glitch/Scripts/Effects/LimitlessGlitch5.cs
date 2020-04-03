using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Limitless.Enums;
[Serializable]
public sealed class OffsetAxesModeParameter : ParameterOverride<AxisMode> { };
[Serializable]
public sealed class IntervalModeParameter : ParameterOverride<IntervalMode> { };

[Serializable]
[PostProcess(typeof(LimitlessGlitch_Glitch5Renderer), PostProcessEvent.BeforeStack, "Limitless Glitch/Glitch5", false)]
public sealed class LimitlessGlitch5 : PostProcessEffectSettings
{
    [Tooltip(" Displacement axis. ")]
    public OffsetAxesModeParameter offsetAxis = new OffsetAxesModeParameter { };
    [Tooltip("shift axis")]
    public OffsetAxesModeParameter shiftMode = new OffsetAxesModeParameter { };
    [Tooltip(" Displacement lines width.")]
    public FloatParameter stretchResolution = new FloatParameter { value = 1f };
    [Space]
    [Tooltip(" Infinite - 0. Periodic- 1, Random - 2")]
    public IntervalModeParameter interval = new IntervalModeParameter { };
    [Tooltip("min/max ranom interval, if Interval = 2.")]
    public Vector2Parameter minMax = new Vector2Parameter { value = new Vector2(0.5f, 2.4f) };
    [Range(0f, 25f), Tooltip("Glitch periodic interval in seconds.")]
    public FloatParameter frequency = new FloatParameter { value = 1f };
    [Range(1f, 0f), Tooltip("Glitch decrease rate due time interval (0 - infinite).")]
    public FloatParameter rate = new FloatParameter { value = 1f };
    [Range(0f, 50f), Tooltip("Effect amount.")]
    public FloatParameter amount = new FloatParameter { value = 1f };
    [Range(0f, 1f), Tooltip("effect speed.")]
    public FloatParameter speed = new FloatParameter { value = 1f };

    [Space]
    [Tooltip("Time.unscaledTime .")]
    public BoolParameter unscaledTime = new BoolParameter { value = false };

    public float t;
}

public sealed class LimitlessGlitch_Glitch5Renderer : PostProcessEffectRenderer<LimitlessGlitch5>
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
        else
        {
            tempVFR = settings.frequency.value;
        }

        var sheet = context.propertySheets.Get(Shader.Find("LimitlessGlitch/Glitch5"));

        if (settings.unscaledTime) { _time = Time.unscaledTime; }
        else _time = Time.time;

        sheet.properties.SetFloat("screenLinesNum", settings.stretchResolution);
        sheet.properties.SetFloat("time_", _time);
        if (settings.interval.value == IntervalMode.Infinite) sheet.EnableKeyword("CUSTOM_INTERVAL"); else sheet.DisableKeyword("CUSTOM_INTERVAL");
        if (settings.shiftMode.value == AxisMode.Horizontal) sheet.EnableKeyword("SHIFT_H"); else sheet.DisableKeyword("SHIFT_H");

        sheet.properties.SetFloat("jitterHAmount", settings.amount);
        sheet.properties.SetFloat("speed", settings.speed);
        sheet.properties.SetFloat("jitterHFreq", tempVFR);
        sheet.properties.SetFloat("jitterHRate", settings.rate);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, settings.offsetAxis.value == AxisMode.Horizontal ? 0 : 1);
    }
    private void ParamSwitch(PropertySheet mat, bool paramValue, string paramName)
    {
        if (paramValue) mat.EnableKeyword(paramName);
        else mat.DisableKeyword(paramName);
    }
}
