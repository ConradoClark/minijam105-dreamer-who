using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Orchestration;
using UnityEngine;

public class DreamCharacterCollisionDetector : MonoBehaviour
{
    public enum CharacterColliders
    {
        Horizontal,
        Vertical
    }

    public Collider2D HorizontalCollider;
    public Collider2D VerticalCollider;

    public RaycastHit2D HandleCollision(CharacterColliders charCollider, Vector2 speed, float magnitudeMultiplier = 0.1f)
    {
        var col = charCollider == CharacterColliders.Horizontal ? HorizontalCollider : VerticalCollider;
        var results = new RaycastHit2D[16];

        col.Cast(speed.normalized, results, speed.magnitude * magnitudeMultiplier);
        return results.FirstOrDefault(r => r != default);
    }
}
