using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class EnemyPool : WeightedPool
{
    private Enemy _enemy;

    public Enemy Enemy
    {
        get
        {
            if (_enemy == null)
            {
                _enemy = Prefab.GetComponent<Enemy>();
            }

            return _enemy;
        }
    }

    public bool TryGetEnemy(out Enemy enemy)
    {
        if (TryGetFromPool(out var comp) && comp is Enemy e)
        {
            enemy = e;
            return true;
        }

        enemy = null;
        return false;
    }
}

