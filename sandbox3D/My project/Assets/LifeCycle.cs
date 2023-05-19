using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeCycle : MonoBehaviour
{
    // ����������Ŭ�� ũ�� [�ʱ�ȭ - ������ - ����] �ܰ�� �����ȴ�.
    
    // �ʱ�ȭ �ܰ�: Awake - Start
    // ���� ������Ʈ ������ ��, ���� ����
    void Awake()
    {
        Debug.Log("�÷��̾� �����Ͱ� �غ�Ǿ����ϴ�.");
    }

    // ������Ʈ ���� ����, ���� ����
    void Start()
    {
        Debug.Log("��� ��� ì����ϴ�.");
    }

    // ������Ʈ Ȱ��ȭ �ܰ�: OnEnable
    void OnEnable()
    {
        Debug.Log("�α����Ͽ����ϴ�.");
    }

    // �������� �ܰ�: FixedUpdate
    // �������� ������Ʈ. 1�ʿ� ������ ����ȴ�. �׷��� CPU�� ���� ����ϰ� �ȴ�.
    void FixedUpdate()
    {
        Debug.Log("�̵�!");
    }

    // ���ӷ��� �ܰ�: Update - LateUpdate
    // ���ӷ��� ������Ʈ. ���� ȯ�濡 ���� ���� �ֱⰡ �޶��� �� �ִ�.
    void Update()
    {
        Debug.Log("���� ���!");
    }

    // ������Ʈ �Լ��� ���� �� ����. ������Ʈ ���� Ƚ���� ���� Ƚ���� �����
    void LateUpdate()
    {
        Debug.Log("����ġ 10 ȹ��!");
    }

    // ������Ʈ ��Ȱ��ȭ �ܰ�: OnDisable
    void OnDisable()
    {
        Debug.Log("�α׾ƿ��Ͽ����ϴ�.");
    }

    // ��ü �ܰ�: OnDestroy
    // ���� ������Ʈ�� ������ �� ����
    void OnDestroy()
    {
        Debug.Log("�÷��̾� �����͸� �����Ͽ����ϴ�.");
    }
}
