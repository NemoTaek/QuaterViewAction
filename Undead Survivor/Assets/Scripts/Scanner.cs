using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour
{
    public float scanRange;
    public LayerMask targetLayer;
    public RaycastHit2D[] targets;
    public Transform nearestTarget;

    void FixedUpdate()
    {
        // CircleCastAll(시작 위치, 반지름, 캐스팅 방향, 캐스팅 길이, 대상 레이어): 원형의 캐스트를 쏘고 모든 결과를 반환하는 함수
        targets = Physics2D.CircleCastAll(transform.position, scanRange, Vector2.zero, 0, targetLayer);
        nearestTarget = GetNearestTarget();
    }

    public Transform GetNearestTarget()
    {
        Transform result = null;
        float difference = 100f;    // 인식할 최소 범위

        foreach(RaycastHit2D target in targets)
        {
            Vector3 playerPosition = transform.position;
            Vector3 targetPosition = target.transform.position;
            float currentDifference = Vector3.Distance(playerPosition, targetPosition);
            if(currentDifference < difference)
            {
                difference = currentDifference;
                result = target.transform;
            }
        }
        return result;
    }
}
