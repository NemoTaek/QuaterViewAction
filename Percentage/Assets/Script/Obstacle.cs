using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Obstacle : MonoBehaviour
{
    public Collider2D col;

    void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    void Start()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //if (collision.collider.CompareTag("Player") && GameManager.instance.player.isFly) col.isTrigger = true;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        //col.isTrigger = false;
    }

    void Update()
    {
        // �÷��̾ ����ɷ��� ������ ��� �����ϵ��� ����
        // �ٵ� ������ ���� ���� ����ع����ٴ°�..
        //col.isTrigger = GameManager.instance.player.isFly;
    }
}
