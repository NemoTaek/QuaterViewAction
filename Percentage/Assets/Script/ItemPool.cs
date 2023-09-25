using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPool : MonoBehaviour
{
    public Item[] items;
    List<Item>[] pools;

    void Awake()
    {
        pools = new List<Item>[items.Length];
        for (int i = 0; i < pools.Length; i++)
        {
            pools[i] = new List<Item>();
        }
    }

    public Item Get(int index)
    {
        Item select = null;

        foreach (Item item in pools[index])
        {
            if (!item.gameObject.activeSelf)
            {
                select = item;
                select.gameObject.SetActive(true);
                break;
            }
        }

        if (!select)
        {
            select = Instantiate(items[index], transform);
            pools[index].Add(select);
        }

        return select;
    }
}
