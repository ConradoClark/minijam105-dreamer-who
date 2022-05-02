using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Events;
using Licht.Unity.Extensions;
using Licht.Unity.Pooling;
using UnityEngine;

public class Enemy : EffectPoolable
{
    public Collider2D Collider;
    public GameToolbox Toolbox;
    public int HitPoints;
    private int _currentHitPoints;
    public EnemyTag[] Tags;
    public int BonusTime;
    private EffectToolbox _effects;
    private IEventPublisher<TimerEvents, TimerChangedEventArgs> _eventPublisher;

    public AudioSource HitSound;

    private void OnEnable()
    {
        Collider.enabled = true;
        _effects = _effects != null ? _effects : FindObjectOfType<EffectToolbox>();
        _eventPublisher = this.RegisterAsEventPublisher<TimerEvents, TimerChangedEventArgs>();
        this.ObserveEvent<HitEvents, DamageEventArgs>(HitEvents.OnDamage, OnEvent);
    }

    private void OnEvent(DamageEventArgs obj)
    {
        if (_currentHitPoints == 0 || obj.Source.gameObject != gameObject) return;
        _currentHitPoints--;

        HitSound.PlayWithRandomPitch(1.3f,0.3f);

        if (_currentHitPoints != 0) return;
        Collider.enabled = false;

        _eventPublisher.PublishEvent(TimerEvents.OnTimerChanged, new TimerChangedEventArgs
        {
            AmountInSeconds = BonusTime
        });

        if (_effects.GetPool(Constants.Effects.PopupTimer).TryGetFromPool(out var popup) && popup is TimerEffect effect)
        {
            effect.transform.position = transform.position + Vector3.up;
            Toolbox.MainMachinery.Machinery.AddBasicMachine(effect.Popup(BonusTime));
        }

        Toolbox.MainMachinery.Machinery.AddBasicMachine(Die());
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
            transform.Rotate(Vector3.forward, 50f);
            yield return TimeYields.WaitOneFrameX;
        }
    }

    private IEnumerable<IEnumerable<Action>> Die()
    {
        var goUp = transform.GetAccessor().Position.Y
            .Increase(1f)
            .Over(1f)
            .UsingTimer(Toolbox.GameTimer.Timer)
            .Easing(EasingYields.EasingFunction.CubicEaseOut)
            .Build();

        yield return RotateAndScaleOnDeath().AsCoroutine().Combine(goUp);
        IsEffectOver = true;
    }

    public override void OnActivation()
    {
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;
        _currentHitPoints = HitPoints;
    }

    public override bool IsEffectOver { get; protected set; }
}
