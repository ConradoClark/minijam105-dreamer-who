using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomMusic : BaseGameObject
{
    public AudioSource Source;
    public AudioClip[] Songs;
    void OnEnable()
    {
        DefaultMachinery.AddBasicMachine(Play());
    }

    IEnumerable<IEnumerable<Action>> Play()
    {
        while (isActiveAndEnabled)
        {
            var song = Random.Range(0, Songs.Length);
            Source.clip = Songs[song];
            Source.PlayWithRandomPitch(0.9f, 0.2f);
            while (Source.isPlaying)
            {
                yield return TimeYields.WaitOneFrameX;
            }
        }
    }
}
