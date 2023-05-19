using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAsset : MonoBehaviour
{
    void Start()
    {
        // translate: ���Ͱ��� ���� ��ġ�� ���ϴ� �Լ�
        Vector3 vec = new Vector3(5, 0, 0);
        transform.Translate(vec);   // �����ϸ� x������ 5��ŭ ��������
    }

    void Update()
    {
        Vector3 vec = new Vector3(Input.GetAxis("Horizontal"), 0.001f, 0);
        transform.Translate(vec);   // �����Ӹ��� x������ Ű���� �Է¾縸ŭ, y������ 0.001��ŭ ��������
    }
}
