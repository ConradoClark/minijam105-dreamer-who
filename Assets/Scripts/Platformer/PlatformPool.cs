using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class PlatformPool : WeightedPool
{
    public bool TryGetPlatform(out Platform platform)
    {
        if (TryGetFromPool(out var comp) && comp is Platform plat)
        {
            platform = plat;
            return true;
        }

        platform = null;
        return false;
    }
}

