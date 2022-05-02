using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Events;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class LevelsBeatenCounter : MonoBehaviour
{
    private TMP_Text _textComponent;
    private int _levelsBeaten;

    void OnEnable()
    {
        _textComponent = _textComponent != null ? _textComponent : GetComponent<TMP_Text>();
        this.ObserveEvent(CharacterEvents.ExitedMap, OnExitedMap);
    }

    private void OnExitedMap()
    {
        _levelsBeaten++;
        _textComponent.text = _levelsBeaten.ToString().PadLeft(3, '0');
    }

    void OnDisable()
    {
        this.StopObservingEvent(CharacterEvents.ExitedMap, OnExitedMap);
    }
}
