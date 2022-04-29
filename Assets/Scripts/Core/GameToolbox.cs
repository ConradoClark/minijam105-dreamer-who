using System.Collections;
using System.Collections.Generic;
using Licht.Unity.Objects;
using UnityEngine;

[CreateAssetMenu(fileName = "DreamerWho", menuName = "DreamerWho/GameToolbox", order = 1)]
public class GameToolbox : ScriptableObject
{
    public TimerScriptable UITimer;
    public TimerScriptable GameTimer;
    public BasicMachineryScriptable MainMachinery;
}
