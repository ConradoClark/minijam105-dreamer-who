using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Events;
using Licht.Unity.Extensions;
using UnityEngine;

[RequireComponent(typeof(DreamCharacterCollisionDetector))]
[RequireComponent(typeof(DreamCharacterController))]
[RequireComponent(typeof(SpriteRenderer))]
public class DreamCharacterHittable : MonoBehaviour
{
    public float YLimit;
    public int FallDamage;

    private FrameVariablesUpdater _frameVars;
    private DreamCharacterCollisionDetector _collisionDetector;
    private DreamCharacterController _controller;
    private SpriteRenderer _spriteRenderer;
    private EffectToolbox _effectToolbox;

    private IEventPublisher<TimerEvents, TimerChangedEventArgs> _eventPublisher;
    private Vector3 _originalPosition;

    public GameToolbox Toolbox;

    public AudioSource HitDamageSound;

    private void OnEnable()
    {
        _originalPosition = transform.position;
        _effectToolbox = _effectToolbox != null ? _effectToolbox : FindObjectOfType<EffectToolbox>();

        _collisionDetector = _collisionDetector != null
            ? _collisionDetector
            : GetComponent<DreamCharacterCollisionDetector>();

        _controller = _controller != null
            ? _controller
            : GetComponent<DreamCharacterController>();

        _spriteRenderer = _spriteRenderer != null ? _spriteRenderer : GetComponent<SpriteRenderer>();

        _frameVars = _frameVars != null ? _frameVars : FindObjectOfType<FrameVariablesUpdater>();

        _eventPublisher = this.RegisterAsEventPublisher<TimerEvents, TimerChangedEventArgs>();

         Toolbox.MainMachinery.Machinery.AddBasicMachine(HandleEnemyCollision());
         Toolbox.MainMachinery.Machinery.AddBasicMachine(HandleOutOfBounds());
    }

    private void OnDisable()
    {
        this.UnregisterAsEventPublisher<TimerEvents, TimerChangedEventArgs>();
    }

    private IEnumerable<IEnumerable<Action>> HandleEnemyCollision()
    {
        while (isActiveAndEnabled)
        {
            var col = _collisionDetector.HandleCollision(DreamCharacterCollisionDetector.CharacterColliders.Horizontal,
                new Vector2(_controller.Direction * 0.1f, 0), 1);

            if (!_controller.IsFalling && col && col.transform.gameObject.layer == LayerMask.NameToLayer(Constants.Layers.Enemy))
            {
                HitDamageSound.Play();
                const int damage = -2;
                _eventPublisher.PublishEvent(TimerEvents.OnTimerChanged, new TimerChangedEventArgs
                {
                    AmountInSeconds = damage
                });
                var dir = -_controller.Direction;
                var delay = 50f;

                if (_effectToolbox.GetPool(Constants.Effects.PopupTimer).TryGetFromPool(out var popup) && popup is TimerEffect effect)
                {
                    effect.transform.position = col.point + Vector2.up;
                    Toolbox.MainMachinery.Machinery.AddBasicMachine(effect.Popup(damage));
                }

                _controller.StartRecoil();

                var horizontalRecoil = _controller.transform.GetAccessor()
                    .Position.X
                    .Increase(1.1f * -_controller.Direction)
                    .Over(0.2f)
                    .Easing(EasingYields.EasingFunction.CubicEaseOut)
                    .UsingTimer(Toolbox.GameTimer.Timer)
                    .Build();

                var verticalRecoil = _controller.transform.GetAccessor()
                    .Position.Y
                    .Increase(0.5f)
                    .Over(0.2f)
                    .Easing(EasingYields.EasingFunction.CubicEaseOut)
                    .UsingTimer(Toolbox.GameTimer.Timer)
                    .Build();

                Toolbox.MainMachinery.Machinery.AddBasicMachine(Flash());

                foreach (var _ in horizontalRecoil.Combine(verticalRecoil))
                {
                    delay -= (float) Toolbox.GameTimer.Timer.UpdatedTimeInMilliseconds;
                    if (delay <= 0 && _collisionDetector.HandleCollision(DreamCharacterCollisionDetector.CharacterColliders.Horizontal,
                            new Vector2(dir * 0.1f, 0),1))
                    {
                        break;
                    }
                    yield return TimeYields.WaitOneFrameX;
                }
                _controller.EndRecoil();
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
                Toolbox.MainMachinery.Machinery.AddBasicMachine(effect.Popup(-FallDamage));
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
            yield return TimeYields.WaitMilliseconds(Toolbox.GameTimer.Timer, 100);
        }
        _spriteRenderer.enabled = true;
    }
}

