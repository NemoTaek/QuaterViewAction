using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public Bullet[] prefabs;
    List<Bullet>[] pools;

    void Awake()
    {
        pools = new List<Bullet>[prefabs.Length];
        for(int i=0; i<pools.Length; i++)
        {
            pools[i] = new List<Bullet>();
        }
    }

    public Bullet Get(int index)
    {
        Bullet select = null;
        Debug.Log(pools.Length);
        foreach(Bullet item in pools[index])
        {
            if(!item.gameObject.activeSelf)
            {
                select = item;
                select.gameObject.SetActive(true);
                break;
            }
        }

        if(!select)
        {
            select = Instantiate(prefabs[index], transform);
            pools[index].Add(select);
        }

        return select;
    }
}
