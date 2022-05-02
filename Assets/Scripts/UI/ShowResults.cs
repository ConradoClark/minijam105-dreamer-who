using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShowResults : MonoBehaviour
{
    public TMP_Text LevelsText;
    public TMP_Text EnemiesText;
    public ScriptableHighScore Score;

    void Start()
    {
        LevelsText.text = Score.LevelsBeaten.ToString().PadLeft(3, '0');
        EnemiesText.text = Score.EnemiesKilled.ToString().PadLeft(3, '0');
    }
}
