using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAsset : MonoBehaviour
{
    Vector3 target = new Vector3(3, 0, 0);
    Vector3 zeroVec = Vector3.zero;

    void Start()
    {
        // translate: ���Ͱ��� ���� ��ġ�� ���ϴ� �Լ�
        //Vector3 vec = new Vector3(5, 0, 0);
        //transform.Translate(vec);   // �����ϸ� x������ 5��ŭ ��������
    }

    void Update()
    {
        // Time.deltaTime: ���� �������� �Ϸ���� �ɸ� �ð�
        // deltaTime ���� �������� ������ ũ��, �������� ������ �۴�.
        // �̸� ��������μ� ���� ȯ��(���)�� �޶� ���� ��� ���� ���� �� �ִ�.
        Vector3 vec = new Vector3(Input.GetAxis("Horizontal"), 0.001f, 0) * Time.deltaTime;
        transform.Translate(vec);   // �����Ӹ��� x������ Ű���� �Է¸�ŭ, y������ 0.001��ŭ ��������

        // 1. MoveTowards(������ġ, ��ǥ��ġ, �ӵ�): ��ǥ ��ġ������ ����̵�
        transform.position = Vector3.MoveTowards(transform.position, target, 0.01f * Time.deltaTime);

        // 2. SmoothDamp(������ġ, ��ǥ��ġ, �����ӵ�, �ӵ�): �ε巯�� �����̵�
        // ������ �Ű������� �ݺ���Ͽ� �ӵ� ����
        // ���� �ӵ��� �߰��ϰ��� �ϴ� ������ �ִٸ� ������, ������ �����͸� ����
        //transform.position = Vector3.SmoothDamp(transform.position, target, ref zeroVec, 1.0f);

        // 3. Lerp(������ġ, ��ǥ��ġ, �ӵ�): ���� ����, SmoothDamp���� ���ӽð��� ���.
        // ������ �Ű������� ����Ͽ� �ӵ� ����
        //transform.position = Vector3.Lerp(transform.position, target, 0.01f);

        // 4. Slerp(������ġ, ��ǥ��ġ, �ӵ�): ���� ���� ����, ȣ�� �׸��鼭 �̵�
        //transform.position = Vector3.Slerp(transform.position, target, 0.01f);
    }
}
