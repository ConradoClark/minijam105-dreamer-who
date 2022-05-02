using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomMusic : MonoBehaviour
{
    public GameToolbox Toolbox;
    public AudioSource Source;
    public AudioClip[] Songs;
    void OnEnable()
    {
        Toolbox.MainMachinery.Machinery.AddBasicMachine(Play());
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
