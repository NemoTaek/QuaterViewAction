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
        // CircleCastAll(���� ��ġ, ������, ĳ���� ����, ĳ���� ����, ��� ���̾�): ������ ĳ��Ʈ�� ��� ��� ����� ��ȯ�ϴ� �Լ�
        targets = Physics2D.CircleCastAll(transform.position, scanRange, Vector2.zero, 0, targetLayer);
        nearestTarget = GetNearestTarget();
    }

    public Transform GetNearestTarget()
    {
        Transform result = null;
        float difference = 100f;    // �ν��� �ּ� ����

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
