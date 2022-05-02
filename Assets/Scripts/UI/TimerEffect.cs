using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Licht.Impl.Orchestration;
using Licht.Unity.Accessors;
using Licht.Unity.Extensions;
using Licht.Unity.Pooling;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class TimerEffect : EffectPoolable
{
    public TMP_Text TextComponent;
    public GameToolbox Toolbox;
    public override void OnActivation()
    {
        TextComponent.color = Color.white;
    }

    public override bool IsEffectOver { get; protected set; }

    public IEnumerable<IEnumerable<Action>> Popup(int value)
    {
        yield return TimeYields.WaitOneFrameX;

        TextComponent.text = $"{value} sec";

        var up = TextComponent.transform.GetAccessor()
            .Position.Y
            .Increase(1f)
            .Over(1f)
            .UsingTimer(Toolbox.GameTimer.Timer)
            .Build();

        yield return up.Combine(RandomizeColor(value).AsCoroutine());

        IsEffectOver = true;
    }

    private IEnumerable<IEnumerable<Action>> RandomizeColor(int value)
    {
        for (var i = 0; i < 10; i++)
        {
            TextComponent.color = Color.HSVToRGB(GetHue(value), 1f, 0.4f + Random.value * 0.25f);
            yield return TimeYields.WaitMilliseconds(Toolbox.GameTimer.Timer, 80);
        }

        TextComponent.color = Color.white;

        yield return new ColorAccessor(f => TextComponent.color = f, () => TextComponent.color)
            .A
            .SetTarget(0f)
            .Over(0.15f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseIn)
            .UsingTimer(Toolbox.GameTimer.Timer)
            .Build();
    }

    private float GetHue(int value)
    {
        return value > 0 ? 0.2f + Random.value * 0.2f : Random.value * 0.1f;
    }
}
