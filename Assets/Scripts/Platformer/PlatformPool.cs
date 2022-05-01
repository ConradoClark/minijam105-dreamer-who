using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class PlatformPool : WeightedPool
{
    private Platform _platform;

    public Platform Platform
    {
        get
        {
            if (_platform == null)
            {
                _platform = Prefab.GetComponent<Platform>();
            }

            return _platform;
        }
    }

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

