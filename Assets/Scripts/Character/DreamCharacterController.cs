using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using Licht.Impl.Orchestration;
using Licht.Unity.Builders;
using Licht.Unity.Extensions;
using UnityEngine;
using UnityEngine.InputSystem;

public class DreamCharacterController : MonoBehaviour
{
    public GameToolbox Toolbox;
    public float Speed;

    private PlayerInput _input;
    private float _refSpeed;

    public bool CanMove { get; private set; }

    private void OnEnable()
    {
        Toolbox.MainMachinery.Machinery.AddBasicMachine(HandleController());
        _input = PlayerInput.GetPlayerByIndex(0);
    }

    private IEnumerable<IEnumerable<Action>> HandleController()
    {
        while (isActiveAndEnabled)
        {
            yield return HandleMovement().AsCoroutine()
                .Combine(HandleJumping().AsCoroutine());

            yield return TimeYields.WaitOneFrameX;
        }
    }

    private IEnumerable<IEnumerable<Action>> Move()
    {
        var updatedTime = (float)Toolbox.GameTimer.Timer.UpdatedTimeInMilliseconds * Constants.FrameUpdateMultiplier;
        transform.position = transform.position += new Vector3(_refSpeed, 0) * updatedTime;
        yield return TimeYields.WaitOneFrameX;
    }

    private IEnumerable<Action> MovementStart(InputAction moveAction)
    {
         return new LerpBuilder(f => _refSpeed = f, () => _refSpeed)
            .SetTarget(Speed * moveAction.ReadValue<float>())
            .Over(0.15f)
            .UsingTimer(Toolbox.GameTimer.Timer)
            .BreakIf(() => !_input.actions[Constants.Actions.Move].IsPressed())
            .Easing(EasingYields.EasingFunction.QuadraticEaseIn)
            .Build();
    }

    private IEnumerable<Action> MovementEnd()
    {
         return new LerpBuilder(f => _refSpeed = f, () => _refSpeed)
            .SetTarget(0f)
            .Over(Mathf.Abs(_refSpeed) * 0.25f)
            .UsingTimer(Toolbox.GameTimer.Timer)
            .BreakIf(() => _input.actions[Constants.Actions.Move].IsPressed())
            .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
            .Build();
    }


    private IEnumerable<IEnumerable<Action>> HandleMovement()
    {
        var moveAction = _input.actions[Constants.Actions.Move];
        while (isActiveAndEnabled)
        {
            if (moveAction.IsPressed())
            {
                foreach (var _ in MovementStart(moveAction))
                {
                    yield return Move().AsCoroutine();
                }
                
                while (moveAction.IsPressed())
                {
                    yield return Move().AsCoroutine();
                }

                foreach (var _ in MovementEnd())
                {
                    yield return Move().AsCoroutine();
                }
            }
            else
            {
                yield return TimeYields.WaitOneFrameX;
            }
        }
    }

    private IEnumerable<IEnumerable<Action>> HandleJumping()
    {
        yield break;
    }
}
