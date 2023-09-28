using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public Bullet[] bullets;
    List<Bullet>[] pools;

    void Awake()
    {
        pools = new List<Bullet>[bullets.Length];
        for (int i = 0; i < pools.Length; i++)
        {
            pools[i] = new List<Bullet>();
        }
    }

    public Bullet Get(int index)
    {
        Bullet select = null;

        foreach (Bullet bullet in pools[index])
        {
            if (!bullet.gameObject.activeSelf)
            {
                select = bullet;
                select.gameObject.SetActive(true);
                break;
            }
        }

        if (!select)
        {
            select = Instantiate(bullets[index], transform);
            pools[index].Add(select);
        }

        return select;
    }
}
