using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherBall : MonoBehaviour
{
    // ������Ʈ�� ���� ������ MeshRenderer�� ���ؼ� ����
    MeshRenderer mesh;
    Material mat;

    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        mat = mesh.material;
    }

    // �������� �浹�� �������� �� ȣ��Ǵ� �Լ�
    // Collision: �浹 ���� Ŭ����
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "MyBall")
        {
            mat.color = new Color(0, 0, 0);
        }
    }

    // �������� �浹�� ����Ǵ� �߿� ȣ��Ǵ� �Լ�
    private void OnCollisionStay(Collision collision)
    {
        
    }

    // �������� �浹�� ������ �� ȣ��Ǵ� �Լ�
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.name == "MyBall")
        {
            mat.color = new Color(1, 1, 1);
        }
    }
}
