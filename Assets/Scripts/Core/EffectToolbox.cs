using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Unity.Pooling;
using UnityEngine;

public class EffectToolbox : MonoBehaviour
{
    [Serializable]
    public struct EffectDefinition
    {
        public string Effect;
        public PrefabPool Pool;
    }

    public EffectDefinition[] Pools;

    public PrefabPool GetPool(string effect)
    {
        return Pools.FirstOrDefault(pool => pool.Effect == effect).Pool;
    }
}
