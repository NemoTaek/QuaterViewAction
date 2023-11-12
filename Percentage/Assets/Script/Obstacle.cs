using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    void Awake()
    {

    }

    void Start()
    {
        
    }

    // ��ֹ��� �÷��̾ Ʈ���� �ϸ� Ʈ���ŵ� ��ֹ� ī��Ʈ ����
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.instance.player.isOnObjectCount++;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.instance.player.isOnObjectCount--;
        }
    }

    void Update()
    {
        // �÷��̾ ����ɷ��� ������ ��� �����ϵ��� ����
        // �ٵ� ������ ���� ���� ����ع����ٴ°�..
        //col.isTrigger = GameManager.instance.player.isFly;
    }
}
