using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Familiar : MonoBehaviour
{
    public int id;

    void Start()
    {
        
    }

    void Update()
    {

    }

    void DoActionFamiliar(int id)
    {

    }

    // 동전주머니 능력: 적 10마리 처치 시 1원 드랍
    public void DropCoin()
    {
        if (GameManager.instance.player.killEnemyCount % 10 == 0)
        {
            Instantiate(GameManager.instance.objectPool.prefabs[1], GameManager.instance.player.familiar.transform);
        }
    }
}
