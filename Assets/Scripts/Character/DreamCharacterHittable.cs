using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Events;
using Licht.Unity.CharacterControllers;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using Licht.Unity.Physics.CollisionDetection;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DreamCharacterHittable : BaseGameObject
{
    public float YLimit;
    public int FallDamage;

    public LichtPhysicsObject PhysicsObject;
    public ScriptIdentifier Grounded;
    public LichtPhysicsCollisionDetector HitBox;
    public LichtPhysicsCollisionDetector BounceDetector;
    public LichtPlatformerJumpController JumpController;
    public LichtPlatformerMoveController MoveController;
    private SpriteRenderer _spriteRenderer;
    private EffectToolbox _effectToolbox;

    private IEventPublisher<TimerEvents, TimerChangedEventArgs> _eventPublisher;
    private Vector3 _originalPosition;

    public AudioSource HitDamageSound;

    private void OnEnable()
    {
        _originalPosition = transform.position;
        _effectToolbox = _effectToolbox != null ? _effectToolbox : FindObjectOfType<EffectToolbox>();

        _spriteRenderer = _spriteRenderer != null ? _spriteRenderer : GetComponent<SpriteRenderer>();

        _eventPublisher = this.RegisterAsEventPublisher<TimerEvents, TimerChangedEventArgs>();

         DefaultMachinery.AddBasicMachine(HandleEnemyCollision());
         DefaultMachinery.AddBasicMachine(HandleOutOfBounds());
    }

    private void OnDisable()
    {
        this.UnregisterAsEventPublisher<TimerEvents, TimerChangedEventArgs>();
    }

    private IEnumerable<IEnumerable<Action>> HandleEnemyCollision()
    {
        while (isActiveAndEnabled)
        {
            var bouncing = BounceDetector.Triggers.FirstOrDefault();
            var hitbox = HitBox.Triggers.FirstOrDefault();

            if (hitbox.TriggeredHit && (JumpController.IsJumping || !bouncing.TriggeredHit || PhysicsObject.GetPhysicsTrigger(Grounded)))
            {
                HitDamageSound.Play();
                const int damage = -2;
                _eventPublisher.PublishEvent(TimerEvents.OnTimerChanged, new TimerChangedEventArgs
                {
                    AmountInSeconds = damage
                });
                var dir = -MoveController.LatestDirection;
                var delay = 50f;

                if (_effectToolbox.GetPool(Constants.Effects.PopupTimer).TryGetFromPool(out var popup) && popup is TimerEffect effect)
                {
                    effect.transform.position = hitbox.Hit.point + Vector2.up;
                    DefaultMachinery.AddBasicMachine(effect.Popup(damage));
                }

                MoveController.BlockMovement(this);

                var horizontalRecoil = PhysicsObject.GetSpeedAccessor()
                    .X
                    .Increase(0.8f * dir)
                    .Over(0.2f)
                    .Easing(EasingYields.EasingFunction.CubicEaseOut)
                    .UsingTimer(GameTimer)
                    .Build();

                var verticalRecoil = PhysicsObject.GetSpeedAccessor()
                    .Y
                    .Increase(0.25f)
                    .Over(0.2f)
                    .Easing(EasingYields.EasingFunction.CubicEaseOut)
                    .UsingTimer(GameTimer)
                    .Build();

                DefaultMachinery.AddBasicMachine(Flash());

                yield return horizontalRecoil.Combine(verticalRecoil);

                MoveController.UnblockMovement(this);
            }

            yield return TimeYields.WaitOneFrameX;
        }
    }

    private IEnumerable<IEnumerable<Action>> HandleOutOfBounds()
    {
        while (isActiveAndEnabled)
        {
            while (transform.position.y > YLimit) yield return TimeYields.WaitOneFrameX;

            HitDamageSound.Play();
            _eventPublisher.PublishEvent(TimerEvents.OnTimerChanged, new TimerChangedEventArgs
            {
                AmountInSeconds = -FallDamage
            });

            if (_effectToolbox.GetPool(Constants.Effects.PopupTimer).TryGetFromPool(out var popup) && popup is TimerEffect effect)
            {
                effect.transform.position = transform.position + Vector3.up;
                DefaultMachinery.AddBasicMachine(effect.Popup(-FallDamage));
            }

            transform.position = _originalPosition;

            yield return Flash().AsCoroutine();
        }
    }

    private IEnumerable<IEnumerable<Action>> Flash()
    {
        _spriteRenderer.enabled = true;
        for (var i = 0; i < 6; i++)
        {
            _spriteRenderer.enabled = !_spriteRenderer.enabled;
            yield return TimeYields.WaitMilliseconds(GameTimer, 100);
        }
        _spriteRenderer.enabled = true;
    }
}

