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
        // 플레이어가 비행능력을 얻으면 통과 가능하도록 설정
        // 근데 문제는 적도 같이 통과해버린다는것..
        //col.isTrigger = GameManager.instance.player.isFly;
    }
}
