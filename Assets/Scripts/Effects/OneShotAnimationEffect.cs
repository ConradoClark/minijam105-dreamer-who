using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Pooling;
using UnityEngine;

public class OneShotAnimationEffect : EffectPoolable
{
    public Animator Animator;
    public string State;

    public override void OnActivation()
    {
        Animator.Play(State);
        BasicMachineryObject.Machinery.AddBasicMachine(WaitUntilAnimationIsFinished());
    }

    public override bool IsEffectOver { get; protected set; }

    IEnumerable<IEnumerable<Action>> WaitUntilAnimationIsFinished()
    {
        yield return TimeYields.WaitOneFrameX;

        while (Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1 || Animator.IsInTransition(0))
        {
            yield return TimeYields.WaitOneFrameX;
        }

        IsEffectOver = true;
    }
}