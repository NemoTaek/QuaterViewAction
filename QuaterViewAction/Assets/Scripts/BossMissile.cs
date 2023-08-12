using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossMissile : Bullet   // 플레이어가 발사하는 총알 기능에 유도 기능을 추가로 더해지기 때문에 상속
{
    public Transform target;
    NavMeshAgent nav;

    void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (nav.enabled)
        {
            nav.SetDestination(target.position);
            //nav.isStopped = !isChase;
        }

    }
}
