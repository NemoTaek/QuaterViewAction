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

    // �����ָӴ� �ɷ�: �� 10���� óġ �� 1�� ���
    public void DropCoin()
    {
        if (GameManager.instance.player.killEnemyCount % 10 == 0)
        {
            Instantiate(GameManager.instance.objectPool.prefabs[1], GameManager.instance.player.familiar.transform);
        }
    }
}
