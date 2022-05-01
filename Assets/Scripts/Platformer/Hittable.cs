using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Events;
using UnityEngine;
using Random = UnityEngine.Random;

public class Hittable : MonoBehaviour
{
    public bool TakesDamage;
    public bool FlashOnHit;
    public Flash Flash;
    public GameToolbox Toolbox;
    public EffectToolbox Effects;
    public Collider2D Collider;
    public Transform Source;

    public HitEventType[] HitTypes;
    private IEventPublisher<HitEvents, DamageEventArgs> _damageEventPublisher;

    private void OnEnable()
    {
        Effects = Effects != null ? Effects : FindObjectOfType<EffectToolbox>();
        this.ObserveEvent<HitEvents, HitEventArgs>(HitEvents.OnHit, OnEvent);
        _damageEventPublisher = this.RegisterAsEventPublisher<HitEvents, DamageEventArgs>();
    }

    private void OnDisable()
    {
        this.StopObservingEvent<HitEvents, HitEventArgs>(HitEvents.OnHit, OnEvent);
    }

    private void OnEvent(HitEventArgs obj)
    {
        if (!FlashOnHit || !HitTypes.Contains(obj.HitType) || obj.Hit.collider != Collider) return;
        
        Toolbox.MainMachinery.Machinery.AddBasicMachine(
            Flash.Activate().AsCoroutine().Combine(SpawnStars(obj.Hit).AsCoroutine()));

        if (TakesDamage)
        {
            _damageEventPublisher.PublishEvent(HitEvents.OnDamage, new DamageEventArgs
            {
                Hit = obj.Hit,
                HitType = obj.HitType,
                Source = Source
            });
        }
    }

    private IEnumerable<IEnumerable<Action>> SpawnStars(RaycastHit2D hit)
    {
        var pool = Effects.GetPool(Constants.Effects.BounceEffect);
        for (var i = 0; i < 3; i++)
        {
            if (pool != null && pool.TryGetFromPool(out var obj))
            {
                obj.Component.transform.position = hit.point + Random.insideUnitCircle * 0.4f;
            }

            yield return TimeYields.WaitSeconds(Toolbox.GameTimer.Timer, 0.1f);
        }
    }
}
