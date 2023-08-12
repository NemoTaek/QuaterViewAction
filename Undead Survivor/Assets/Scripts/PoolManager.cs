using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [Header("------- Prefab variable -------")]
    public GameObject[] prefabs;

    [Header("------- Pool list -------")]
    List<GameObject>[] pools;

    void Awake()
    {
        pools = new List<GameObject>[prefabs.Length];
        for(int i=0; i<pools.Length; i++)
        {
            pools[i] = new List<GameObject>();
        }
    }

    public GameObject GetPool(int index)
    {
        GameObject selectedObject = null;

        foreach(GameObject item in pools[index])
        {
            if(!item.activeSelf)
            {
                selectedObject = item;
                selectedObject.SetActive(true);
                break;
            }
        }

        if(selectedObject == null)
        {
            selectedObject = Instantiate(prefabs[index], transform);
            pools[index].Add(selectedObject);
        }

        return selectedObject;
    }
}
