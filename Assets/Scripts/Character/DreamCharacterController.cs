using System;
using System.Collections.Generic;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Events;
using Licht.Unity.Builders;
using Licht.Unity.Extensions;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class DreamCharacterController : MonoBehaviour
{
    public GameToolbox Toolbox;
    public float Speed;
    public DreamCharacterCollisionDetector CollisionDetector;
    public FrameVariablesUpdater FrameVars;
    public DreamCharacterAnimator Animator;

    public int PointsPerLevel;

    public EffectToolbox Effects;

    private PlayerInput _input;
    private float _refXSpeed;
    public float PlatformYOffset;
    public float Direction { get; private set; }

    public bool CanMove { get; set; } = true;
    public bool IsJumping { get; private set; }
    public bool IsFalling { get; private set; }

    public bool IsRecoiling { get; private set; }
    private bool _hasRecoiled;

    public AudioSource NextLevelSound;
    public AudioSource JumpSound;

    private IEventPublisher<HitEvents, HitEventArgs> _hitEventPublisher;
    private IEventPublisher<CharacterEvents> _charEventsPublisher;
    private IEventPublisher<TimerEvents, TimerChangedEventArgs> _timerEventPublisher;

    private void OnEnable()
    {
        Toolbox.MainMachinery.Machinery.AddBasicMachine(HandleController());
        _input = PlayerInput.GetPlayerByIndex(0);
        _hitEventPublisher = this.RegisterAsEventPublisher<HitEvents, HitEventArgs>();
        _charEventsPublisher = this.RegisterAsEventPublisher<CharacterEvents>();
        _timerEventPublisher = this.RegisterAsEventPublisher<TimerEvents, TimerChangedEventArgs>();
    }

    private void OnDisable()
    {
        this.UnregisterAsEventPublisher<HitEvents, HitEventArgs>();
    }

    private IEnumerable<IEnumerable<Action>> HandleController()
    {
        while (isActiveAndEnabled)
        {
            yield return HandleMovement().AsCoroutine()
                .Combine(HandleVerticals().AsCoroutine())
                .Combine(HandleWrap().AsCoroutine());

            yield return TimeYields.WaitOneFrameX;
        }
    }

    private IEnumerable<IEnumerable<Action>> Move(float speed)
    {
        var updatedTime = (float)Toolbox.GameTimer.Timer.UpdatedTimeInMilliseconds * Constants.FrameUpdateMultiplier;

        var hit = CollisionDetector.HandleCollision(DreamCharacterCollisionDetector.CharacterColliders.Horizontal, new Vector2(speed, 0));

        if (hit == default || Math.Sign(hit.normal.x) == Math.Sign(speed))
        {
            transform.position += new Vector3(speed, 0) * updatedTime;
        }
        else if (hit.distance > 0.01f)
        {
            transform.position = new Vector2(hit.point.x, transform.position.y) - new Vector2(CollisionDetector.HorizontalCollider.bounds.extents.x * Mathf.Sign(speed), 0);
        }

        yield return TimeYields.WaitOneFrameX;
    }

    private IEnumerable<Action> MovementStart(InputAction moveAction)
    {
        return new LerpBuilder(f => _refXSpeed = f, () => _refXSpeed)
           .SetTarget(Speed * moveAction.ReadValue<float>())
           .Over(0.15f)
           .UsingTimer(Toolbox.GameTimer.Timer)
           .BreakIf(() => !IsMovingPressed(), false)
           .Easing(EasingYields.EasingFunction.QuadraticEaseIn)
           .Build();
    }

    private IEnumerable<Action> MovementEnd()
    {
        return new LerpBuilder(f => _refXSpeed = f, () => _refXSpeed)
           .SetTarget(0f)
           .Over(Mathf.Abs(_refXSpeed) * 0.25f)
           .UsingTimer(Toolbox.GameTimer.Timer)
           .BreakIf(IsMovingPressed, false)
           .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
           .Build();
    }

    private IEnumerable<IEnumerable<Action>> HandleMovement()
    {
        var moveAction = _input.actions[Constants.Actions.Move];
        while (isActiveAndEnabled)
        {
            if (IsMovingPressed())
            {
                Animator.SetWalking(true);
                var axis = moveAction.ReadValue<float>();
                Face(axis);
                foreach (var _ in MovementStart(moveAction))
                {
                    yield return Move(_refXSpeed).AsCoroutine();
                }

                while (IsMovingPressed())
                {
                    axis = moveAction.ReadValue<float>();
                    Face(axis);
                    yield return Move(Speed * axis).AsCoroutine();
                }

                if (!CanMove) continue;

                foreach (var _ in MovementEnd())
                {
                    yield return Move(_refXSpeed).AsCoroutine();
                }
            }
            else
            {
                Animator.SetWalking(false);
                yield return TimeYields.WaitOneFrameX;
            }
        }
    }

    private void Face(float axis)
    {
        Animator.Face(axis);
        Direction = Math.Sign(axis);
    }

    private bool IsMovingPressed()
    {
        var moveAction = _input.actions[Constants.Actions.Move];
        return CanMove && moveAction.IsPressed() && Mathf.Abs(moveAction.ReadValue<float>()) > 0f;
    }

    private IEnumerable<IEnumerable<Action>> HandleVerticals()
    {
        var hit = new FrameVariableDefinition<RaycastHit2D>("VerticalCollision",
            () => CollisionDetector.HandleCollision(DreamCharacterCollisionDetector.CharacterColliders.Vertical, new Vector2(0, -0.1f), 0.2f));

        yield return HandleGravity(hit).AsCoroutine().Combine(HandleJumping(hit).AsCoroutine());
    }

    private IEnumerable<IEnumerable<Action>> HandleGravity(FrameVariableDefinition<RaycastHit2D> raycastDef)
    {
        while (isActiveAndEnabled)
        {
            while (IsJumping || IsRecoiling || FrameVars.Get(raycastDef))
            {
                yield return TimeYields.WaitOneFrameX;
            }

            Animator.SetFalling(true);
            IsFalling = true;
            yield return transform.GetAccessor()
                .Position.Y
                .Decrease(1.5f)
                .Over(0.35f)
                .UsingTimer(Toolbox.GameTimer.Timer)
                .BreakIf(() => FrameVars.Get(raycastDef) || IsJumping, false)
                .Easing(EasingYields.EasingFunction.QuadraticEaseIn)
                .Build();

            while (!FrameVars.Get(raycastDef) && !IsJumping)
            {
                var updatedTime = (float)Toolbox.GameTimer.Timer.UpdatedTimeInMilliseconds * Constants.FrameUpdateMultiplier;
                transform.position = new Vector3(transform.position.x, transform.position.y - 1.75f * updatedTime, transform.position.z);

                yield return TimeYields.WaitOneFrameX;
            }

            var hit = FrameVars.Get(raycastDef);
            if (hit)
            {
                transform.position = new Vector3(transform.position.x, hit.transform.position.y + PlatformYOffset);
            }

            if (_hasRecoiled)
            {
                _hasRecoiled = false;
                CanMove = true;
            }

            IsFalling = false;
            Animator.SetFalling(false);

            if (hit && hit.transform.gameObject.layer == LayerMask.NameToLayer(Constants.Layers.Enemy))
            {
                // Register Fall Hit
                _hitEventPublisher.PublishEvent(HitEvents.OnHit, new HitEventArgs
                {
                    HitType = HitEventType.Fall,
                    Hit = hit
                });

                // Bounce
                var jumpAction = _input.actions[Constants.Actions.Jump];
                yield return Jump(jumpAction.IsPressed() ? 1.75f : 1.25f, false).AsCoroutine();
            }
        }
    }

    private IEnumerable<IEnumerable<Action>> Jump(float height = 1.75f, bool playSound=true)
    {
        if (IsJumping)
        {
            yield return TimeYields.WaitOneFrameX;
            yield break;
        }

        if (playSound) JumpSound.PlayWithRandomPitch(0.75f, 0.5f);
        Animator.SetJumping(true);
        IsJumping = true;
        yield return transform.GetAccessor()
            .Position.Y
            .Increase(height)
            .Over(0.35f)
            .UsingTimer(Toolbox.GameTimer.Timer)
            .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
            .Build();
        IsJumping = false;
        Animator.SetJumping(false);
    }

    private IEnumerable<IEnumerable<Action>> HandleJumping(FrameVariableDefinition<RaycastHit2D> raycastDef)
    {
        var jumpAction = _input.actions[Constants.Actions.Jump];
        while (isActiveAndEnabled)
        {
            while (!FrameVars.Get(raycastDef))
            {
                // Buffer input
                if (jumpAction.WasPerformedThisFrame() && !IsJumping && !IsRecoiling)
                {
                    foreach (var _ in TimeYields.WaitSeconds(Toolbox.GameTimer.Timer, 0.15))
                    {
                        if (FrameVars.Get(raycastDef))
                        {
                            yield return Jump().AsCoroutine();
                            break;
                        }

                        yield return TimeYields.WaitOneFrameX;
                    }
                }

                yield return TimeYields.WaitOneFrameX;
            }

            if (FrameVars.Get(raycastDef))
            {
                if (jumpAction.WasPerformedThisFrame() && !IsJumping && !IsRecoiling)
                {
                    yield return Jump().AsCoroutine();
                }
                else
                {
                    yield return TimeYields.WaitOneFrameX;
                }
            }

            // Coyote time
            foreach (var _ in TimeYields.WaitSeconds(Toolbox.GameTimer.Timer, 0.1))
            {
                if (jumpAction.WasPerformedThisFrame() && !IsJumping && !IsRecoiling)
                {
                    yield return Jump().AsCoroutine();
                    break;
                }

                yield return TimeYields.WaitOneFrameX;
            }
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

                _timerEventPublisher.PublishEvent(TimerEvents.OnTimerChanged, new TimerChangedEventArgs
                {
                    AmountInSeconds = PointsPerLevel
                });

                if (Effects.GetPool(Constants.Effects.PopupTimer).TryGetFromPool(out var popup) && popup is TimerEffect effect)
                {
                    effect.transform.position = transform.position + Vector3.right;
                    Toolbox.MainMachinery.Machinery.AddBasicMachine(effect.Popup(PointsPerLevel));
                }

            }
            yield return TimeYields.WaitOneFrameX;
        }
    }

    public void StartRecoil()
    {
        CanMove = false;
        IsRecoiling = true;
    }

    public void EndRecoil()
    {
        _hasRecoiled = true;
        IsRecoiling = false;
    }
}
