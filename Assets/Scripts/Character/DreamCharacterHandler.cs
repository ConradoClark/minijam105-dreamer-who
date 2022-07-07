using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Events;
using Licht.Unity.Builders;
using Licht.Unity.CharacterControllers;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using Licht.Unity.Physics.CollisionDetection;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class DreamCharacterHandler : BaseGameObject
{
    public ScriptIdentifier Grounded;
    public LichtPhysicsObject PhysicsObject;
    public LichtPlatformerJumpController JumpController;

    public int PointsPerLevel;

    public DreamCharacterAnimator Animator;
    public EffectToolbox Effects;
    public float PlatformYOffset;

    public bool IsRecoiling { get; private set; }
    private bool _hasRecoiled;

    public AudioSource NextLevelSound;
    public AudioSource JumpSound;

    public LichtPlatformerJumpController.CustomJumpParams BounceParams;
    public ScriptableHighScore Score;
    public LichtPhysicsCollisionDetector BounceDetector;
    private IEventPublisher<HitEvents, HitEventArgs> _hitEventPublisher;
    private IEventPublisher<CharacterEvents> _charEventsPublisher;
    private IEventPublisher<TimerEvents, TimerChangedEventArgs> _timerEventPublisher;

    private void OnEnable()
    {
        DefaultMachinery.AddBasicMachine(HandleController());
        _hitEventPublisher = this.RegisterAsEventPublisher<HitEvents, HitEventArgs>();
        _charEventsPublisher = this.RegisterAsEventPublisher<CharacterEvents>();
        _timerEventPublisher = this.RegisterAsEventPublisher<TimerEvents, TimerChangedEventArgs>();
    }

    private void OnDisable()
    {
        this.UnregisterAsEventPublisher<HitEvents, HitEventArgs>();
        this.UnregisterAsEventPublisher<TimerEvents, TimerChangedEventArgs>();
    }

    private IEnumerable<IEnumerable<Action>> HandleController()
    {
        while (isActiveAndEnabled)
        {
            yield return HandleWrap().AsCoroutine().Combine(HandleBounce().AsCoroutine());

            yield return TimeYields.WaitOneFrameX;
        }
    }

    private IEnumerable<IEnumerable<Action>> HandleBounce()
    {
        while (isActiveAndEnabled)
        {
            var bouncing = BounceDetector.Triggers.FirstOrDefault();
            if (!JumpController.IsJumping && bouncing.TriggeredHit && !PhysicsObject.GetPhysicsTrigger(Grounded))
            {
                _hitEventPublisher.PublishEvent(HitEvents.OnHit, new HitEventArgs
                {
                    Hit = bouncing.Hit,
                    HitType = HitEventType.Fall
                });

                yield return JumpController.ExecuteJump(customParams: BounceParams).AsCoroutine();
                yield return TimeYields.WaitMilliseconds(GameTimer, 50);
            }

            yield return TimeYields.WaitOneFrameX;
        }
    }

    private IEnumerable<IEnumerable<Action>> HandleWrap()
    {
        while (isActiveAndEnabled)
        {
            if (transform.position.x > 7.9)
            {
                _charEventsPublisher.PublishEvent(CharacterEvents.ExitedMap);
                transform.position = new Vector3(-7f, transform.position.y);

                NextLevelSound.Play();

                Score.LevelsBeaten++;

                _timerEventPublisher.PublishEvent(TimerEvents.OnTimerChanged, new TimerChangedEventArgs
                {
                    AmountInSeconds = PointsPerLevel
                });

                if (Effects.GetPool(Constants.Effects.PopupTimer).TryGetFromPool(out var popup) && popup is TimerEffect effect)
                {
                    effect.transform.position = transform.position + Vector3.right;
                    DefaultMachinery.AddBasicMachine(effect.Popup(PointsPerLevel));
                }

            }
            yield return TimeYields.WaitOneFrameX;
        }
    }

    //public void StartRecoil()
    //{
    //    CanMove = false;
    //    IsRecoiling = true;
    //}

    //public void EndRecoil()
    //{
    //    _hasRecoiled = true;
    //    IsRecoiling = false;
    //}
}
