using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Builders;
using Licht.Unity.Extensions;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class HueShiftMenu : MonoBehaviour
{
    public GameToolbox Toolbox;
    private SpriteRenderer _spriteRenderer;

    private void OnEnable()
    {   
        _spriteRenderer = GetComponent<SpriteRenderer>();
        Toolbox.MainMachinery.Machinery.AddBasicMachine(HueShift());
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
             .UsingTimer(Toolbox.GameTimer.Timer)
             .Build();

            yield return new LerpBuilder(f => _spriteRenderer.material.SetFloat("_Hue", f),
                () => _spriteRenderer.material.GetFloat("_Hue"))
            .SetTarget(0)
            .Over(5)
            .Easing(EasingYields.EasingFunction.CubicEaseInOut)
            .UsingTimer(Toolbox.GameTimer.Timer)
            .Build();
        }

    }
}
