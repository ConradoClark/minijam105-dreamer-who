using System.Collections;
using System.Collections.Generic;
using Licht.Interfaces.Generation;
using Licht.Unity.Pooling;
using UnityEngine;

public class Platform : EffectPoolable
{
    public float GridWidth;
    public float GridHeight;

    public float Width;
    public float Height;

    public SpriteRenderer SpriteRenderer;

    public override void OnActivation()
    {
    }

    public override bool IsEffectOver { get; protected set; }

    public void Destroy()
    {
        IsEffectOver = true;
    }
}
