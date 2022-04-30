using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HitEvents
{
    OnHit
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