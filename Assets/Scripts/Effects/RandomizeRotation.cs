using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeRotation : MonoBehaviour
{
    void OnEnable()
    {
        transform.rotation = Quaternion.FromToRotation(Vector2.right, Random.insideUnitCircle);
    }
}
