using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HitEvents
{
    OnHit,
    OnDamage,
}

public enum HitEventType
{
    Fall,
    Touch,
}

public class HitEventArgs
{
    public HitEventType HitType;
    public RaycastHit2D Hit;
}

public class DamageEventArgs : HitEventArgs
{
    public Component Source;
}