using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Builders;
using Licht.Unity.Objects;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class TextBlink : BaseUIObject
{
    private TMP_Text _textComponent;

    private void OnEnable()
    {
        _textComponent = GetComponent<TMP_Text>();
        DefaultMachinery.AddBasicMachine(Blink());
    }

    private IEnumerable<IEnumerable<Action>> Blink()
    {
        while (isActiveAndEnabled)
        {
            yield return TimeYields.WaitMilliseconds(UITimer, 200);
            _textComponent.color = _textComponent.color == Color.white ? new Color(1, 1, 1, 0) : Color.white;
        }
    }
}
