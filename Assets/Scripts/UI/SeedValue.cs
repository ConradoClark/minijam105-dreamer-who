using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Events;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class SeedValue : MonoBehaviour
{
    private TMP_Text _text;
    void OnEnable()
    {
        _text = _text != null ? _text : GetComponent<TMP_Text>();
        this.ObserveEvent<RoomEvents, RoomEventArgs>(RoomEvents.OnRoomGenerated, OnEvent);
    }

    private void OnEvent(RoomEventArgs obj)
    {
        _text.text = obj.Seed;
    }

    void OnDisable()
    {
        this.StopObservingEvent<RoomEvents, RoomEventArgs>(RoomEvents.OnRoomGenerated, OnEvent);
    }
}
