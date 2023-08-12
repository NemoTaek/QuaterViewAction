using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossMissile : Bullet   // �÷��̾ �߻��ϴ� �Ѿ� ��ɿ� ���� ����� �߰��� �������� ������ ���
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
