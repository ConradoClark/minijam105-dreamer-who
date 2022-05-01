using System.Collections;
using System.Collections.Generic;
using Licht.Interfaces.Generation;
using Licht.Unity.Pooling;
using UnityEngine;

public class Platform : EffectPoolable
{
    public float GridWidth;
    public float GridHeight;

    public int Width;
    public int Height;

    public SpriteRenderer SpriteRenderer;

    public EnemyTag[] AllowsEnemies;

    public override void OnActivation()
    {
    }

    public override bool IsEffectOver { get; protected set; }

    public void Destroy()
    {
        IsEffectOver = true;
    }
}
