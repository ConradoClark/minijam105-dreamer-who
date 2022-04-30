using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using UnityEngine;
using Random = UnityEngine.Random;

public class Hittable : MonoBehaviour
{
    public bool FlashOnHit;
    public Flash Flash;
    public GameToolbox Toolbox;
    public EffectToolbox Effects;

    public HitEventType[] HitTypes;

    private void OnEnable()
    {
        this.ObserveEvent<HitEvents, HitEventArgs>(HitEvents.OnHit, OnEvent);
    }

    private void OnDisable()
    {
        this.StopObservingEvent<HitEvents, HitEventArgs>(HitEvents.OnHit, OnEvent);
    }

    private void OnEvent(HitEventArgs obj)
    {
        if (FlashOnHit && HitTypes.Contains(obj.HitType))
        {
            Toolbox.MainMachinery.Machinery.AddBasicMachine(
                    Flash.Activate().AsCoroutine().Combine(SpawnStars(obj.Hit).AsCoroutine()));
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
