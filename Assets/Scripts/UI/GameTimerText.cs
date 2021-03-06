using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(TMP_Text))]
public class GameTimerText : BaseGameObject
{
    public float StartingTimer;
    private TMP_Text _text;

    private float _timer;

    private void OnEnable()
    {
        _text = _text != null ? _text : GetComponent<TMP_Text>();
        DefaultMachinery.AddBasicMachine(HandleTimer());
        this.ObserveEvent<TimerEvents,TimerChangedEventArgs>(TimerEvents.OnTimerChanged, OnEvent);
    }

    private void OnEvent(TimerChangedEventArgs obj)
    {
        _timer += obj.AmountInSeconds;
    }

    private void OnDisable()
    {
        this.StopObservingEvent<TimerEvents, TimerChangedEventArgs>(TimerEvents.OnTimerChanged, OnEvent);
    }

    private IEnumerable<IEnumerable<Action>> HandleTimer()
    {
        _timer = StartingTimer;
        while (isActiveAndEnabled)
        { 
            while (_timer > 0)
            {
                var timeSpan = TimeSpan.FromSeconds(_timer);
                _text.text = timeSpan.ToString("mm':'ss'.'fff");

                _timer -= (float)GameTimer.UpdatedTimeInMilliseconds * 0.001f;
                yield return TimeYields.WaitOneFrameX;
            }

            _text.text = "Time Up!";

            for (var i = 0; i < 15; i++)
            {
                _text.enabled = !_text.enabled;
                yield return TimeYields.WaitMilliseconds(GameTimer, 100);
            }

            DefaultMachinery.FinalizeWith(() =>
            {
                SceneManager.LoadScene("Scenes/Results", LoadSceneMode.Single);
            });

            yield return TimeYields.WaitOneFrameX;
        }
    }
}
