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

    // 장애물과 플레이어가 트리거 하면 트리거된 장애물 카운트 세팅
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
        // 플레이어가 비행능력을 얻으면 통과 가능하도록 설정
        // 근데 문제는 적도 같이 통과해버린다는것..
        //col.isTrigger = GameManager.instance.player.isFly;
    }
}
