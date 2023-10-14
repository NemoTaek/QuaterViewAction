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

    void Update()
    {
        // 플레이어가 비행능력을 얻으면 통과 가능하도록 설정
        col.isTrigger = GameManager.instance.player.isFly;
    }
}
