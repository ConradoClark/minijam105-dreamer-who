using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Interfaces.Generation;
using Licht.Unity.Pooling;

public class WeightedPool : PrefabPool, IWeighted<float>
{
    public float Weight => 1;
}

