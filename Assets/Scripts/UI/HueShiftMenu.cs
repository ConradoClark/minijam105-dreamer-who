using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Builders;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class HueShiftMenu : BaseGameObject
{
    private SpriteRenderer _spriteRenderer;

    private void OnEnable()
    {   
        _spriteRenderer = GetComponent<SpriteRenderer>();
        DefaultMachinery.AddBasicMachine(HueShift());
    }

    private IEnumerable<IEnumerable<Action>> HueShift()
    {
        while (isActiveAndEnabled)
        {

            yield return new LerpBuilder(f => _spriteRenderer.material.SetFloat("_Hue", f),
                 () => _spriteRenderer.material.GetFloat("_Hue"))
             .SetTarget(255)
             .Over(5)
             .Easing(EasingYields.EasingFunction.CubicEaseInOut)
             .UsingTimer(GameTimer)
             .Build();

            yield return new LerpBuilder(f => _spriteRenderer.material.SetFloat("_Hue", f),
                () => _spriteRenderer.material.GetFloat("_Hue"))
            .SetTarget(0)
            .Over(5)
            .Easing(EasingYields.EasingFunction.CubicEaseInOut)
            .UsingTimer(GameTimer)
            .Build();
        }

    }
}
