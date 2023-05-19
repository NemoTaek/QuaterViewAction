using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardMouse : MonoBehaviour
{
    void Update()
    {
        // Input: ���� �� �Է��� �����ϴ� Ŭ����
        if(Input.anyKeyDown)    // �ƹ� �Է��� ���ʷ� �޴´ٸ� true
        {
            Debug.Log("�÷��̾ �ƹ� Ű�� �������ϴ�.");
        }
        /*
        if (Input.anyKey)    // �ƹ� �Է��� �޴´ٸ� true
        {
            Debug.Log("�÷��̾ �ƹ� Ű�� ������ �ֽ��ϴ�.");
        }
        */
        if (Input.GetKey(KeyCode.LeftArrow))    // �ش� Ű�� ������ ������ true
        {
            Debug.Log("�������� �̵���");
        }
        if (Input.GetKeyDown(KeyCode.Return))    // �ش� Ű�� ���� ������ true
        {
            Debug.Log("Ȯ��!");
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))    // �ش� Ű�� ���� true
        {
            Debug.Log("���������� �̵��� ���߾����ϴ�");
        }

        // ���콺 ����
        if (Input.GetMouseButton(0))    // ���콺�� Ŭ���ϰ� ������ true
        {
            Debug.Log("���콺 Ŭ����");
        }
        if (Input.GetMouseButtonDown(0))    // ���콺�� Ŭ���� ������ true
        {
            Debug.Log("���콺 Ŭ��!");
        }
        if (Input.GetMouseButtonUp(0))    // ���콺�� Ŭ�� �� ���� true
        {
            Debug.Log("���콺 Ŭ���� ���߾����ϴ�");
        }

        // Edit - Project Settings - Input Manager�� �̸� ������ ���� �ҷ����⵵ ����
        if (Input.GetButton("Jump"))    // �̸� ������ Ű�� ������ ������ true
        {
            Debug.Log("�������� ���������...!");
        }
        if (Input.GetButtonDown("Jump"))    // �̸� ������ Ű�� ���� ������ true
        {
            Debug.Log("����!!");
        }
        if (Input.GetButtonUp("Jump"))    // �̸� ������ Ű�� ���� �� ���� true
        {
            Debug.Log("��������!");
        }
    }
}
