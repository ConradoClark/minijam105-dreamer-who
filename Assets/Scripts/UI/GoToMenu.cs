using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;

public class GoToMenu : BaseGameObject
{
    private bool _mButtonPressed;

    private bool _started = false;
    // Start is called before the first frame update
    void Start()
    {
        InputSystem.onEvent +=
            (eventPtr, device) =>
            {
                if (!eventPtr.IsA<StateEvent>() && !eventPtr.IsA<DeltaStateEvent>())
                    return;
                var controls = device.allControls;
                var buttonPressPoint = InputSystem.settings.defaultButtonPressPoint;
                foreach (var control in controls.Select(t => t as ButtonControl).Where(control => control is
                         {
                             synthetic: false, noisy: false
                         }))
                {
                    if (!control.ReadValueFromEvent(eventPtr, out var value) || !(value >= buttonPressPoint)) continue;
                    _mButtonPressed = true;
                    break;
                }
            };
        DefaultMachinery.AddBasicMachine(Finalize());
    }

    IEnumerable<IEnumerable<Action>> Finalize()
    {
        yield return TimeYields.WaitSeconds(GameTimer, 2);

        while (isActiveAndEnabled)
        {
            if (_mButtonPressed && !_started)
            {
                _started = true;

                DefaultMachinery.FinalizeWith(() =>
                {
                    SceneManager.LoadScene("Scenes/Menu", LoadSceneMode.Single);
                });
            }

            yield return TimeYields.WaitOneFrameX;
        }
    }
}
