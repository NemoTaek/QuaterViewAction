using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public IEnumerator MoveRoom(Vector3 destination)
    {
        float delta = 0;
        float moveTimer = 1.0f;     // �� �̵��� �� ī�޶� �̵��ϴ� �ð�

        // ���� �̵��Լ��� �̿��� ������Ʈ �̵��� 4������ �����Ѵ�.
        // 1. movetowoards: ������ �ӵ��� �����
        // 2. SmoothDamp: ����� �ϴٰ� ������ ����, �� �Լ��� 3��° ������ �ӵ��� �������� ��������.
        // 3. Lerp: �������� �̵�
        // 4. Slerp: ���� �������� �̵�
        while (delta <= moveTimer)
        {
            float time = delta / moveTimer;
            transform.position = Vector3.Lerp(transform.position, destination, time);
            delta += Time.deltaTime;
            yield return null;
        }
    }
}
