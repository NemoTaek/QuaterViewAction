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
        // �÷��̾ ����ɷ��� ������ ��� �����ϵ��� ����
        col.isTrigger = GameManager.instance.player.isFly;
    }
}
