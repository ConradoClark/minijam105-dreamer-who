using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DreamerWho", menuName = "DreamerWho/Score", order = 1)]
public class ScriptableHighScore : ScriptableObject
{
    public int LevelsBeaten;
    public int EnemiesKilled;
}
