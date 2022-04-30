using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Collider2D Collider;
    public GameToolbox Toolbox;
    public int HitPoints;

    private void OnEnable()
    {
        Collider.enabled = true;
        this.ObserveEvent<HitEvents, DamageEventArgs>(HitEvents.OnDamage, OnEvent);
    }

    private void OnEvent(DamageEventArgs obj)
    {
        if (HitPoints == 0 || obj.Source.gameObject != gameObject) return;
        HitPoints--;

        if (HitPoints == 0)
        {
            Collider.enabled = false;
            Toolbox.MainMachinery.Machinery.AddBasicMachine(Die());
        }
    }

    private void OnDisable()
    {
        this.StopObservingEvent<HitEvents, DamageEventArgs>(HitEvents.OnDamage, OnEvent);
    }

    private IEnumerable<IEnumerable<Action>> RotateAndScaleOnDeath()
    {
        foreach (var _ in transform.GetAccessor().UniformScale()
                     .SetTarget(0f)
                     .Over(1f)
                     .UsingTimer(Toolbox.GameTimer.Timer)
                     .Easing(EasingYields.EasingFunction.CubicEaseOut)
                     .Build())
        {
            transform.Rotate(Vector3.forward, 30f);
            yield return TimeYields.WaitOneFrameX;
        }

    }

    private IEnumerable<IEnumerable<Action>> Die()
    {
        var goUp = transform.GetAccessor().Position.Y
            .Increase(3f)
            .Over(1f)
            .UsingTimer(Toolbox.GameTimer.Timer)
            .Easing(EasingYields.EasingFunction.CubicEaseOut)
            .Build();

        yield return RotateAndScaleOnDeath().AsCoroutine().Combine(goUp);
        gameObject.SetActive(false);
    }
}
