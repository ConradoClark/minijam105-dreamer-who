using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public static class AudioSourceExtensions
{
    public static void PlayWithRandomPitch(this AudioSource audioSource, float min, float variance)
    {
        audioSource.pitch = min + Random.value * variance;
        audioSource.Play();
    }
}
