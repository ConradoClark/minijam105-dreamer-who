using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Builders;
using Licht.Unity.Objects;
using UnityEngine;

public class Flash : BaseGameObject
{
    public SpriteRenderer SpriteRenderer;

    public IEnumerable<IEnumerable<Action>> Activate()
    {
        yield return new LerpBuilder(f => SpriteRenderer.material.SetFloat("_Luminance", f),
                () => SpriteRenderer.material.GetFloat("_Luminance"))
            .SetTarget(1f)
            .Over(0.25f)
            .UsingTimer(GameTimer)
            .Easing(EasingYields.EasingFunction.CubicEaseOut)
            .Build();

        yield return new LerpBuilder(f => SpriteRenderer.material.SetFloat("_Luminance", f),
                () => SpriteRenderer.material.GetFloat("_Luminance"))
            .SetTarget(0f)
            .Over(0.15f)
            .UsingTimer(GameTimer)
            .Easing(EasingYields.EasingFunction.CubicEaseIn)
            .Build();
    }
}
