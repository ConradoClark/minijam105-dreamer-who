using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Orchestration;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public GameToolbox Toolbox;
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
        Toolbox.MainMachinery.Machinery.AddBasicMachine(Finalize());
    }

    IEnumerable<IEnumerable<Action>> Finalize()
    {
        yield return TimeYields.WaitSeconds(Toolbox.GameTimer.Timer, 0.35);

        while (isActiveAndEnabled)
        {
            if (_mButtonPressed && !_started)
            {
                _started = true;

                Toolbox.MainMachinery.Machinery.FinalizeWith(() =>
                {
                    SceneManager.LoadScene("Scenes/MainGame", LoadSceneMode.Single);
                });
            }

            yield return TimeYields.WaitOneFrameX;
        }
    }
}
