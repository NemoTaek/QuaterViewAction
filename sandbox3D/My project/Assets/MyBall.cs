using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyBall : MonoBehaviour
{
    Rigidbody rigid;
    
    void Start()
    {
        rigid = GetComponent<Rigidbody>();

        //rigid.velocity = Vector3.right; // ������ �������� �ӵ��� �ְڴ�
        //rigid.velocity = new Vector3(2, 4, 3); // ������ �������� �ӵ��� �ְڴ�.

        
    }

    void FixedUpdate()
    {
        // rigid ���� �ڵ�� FixedUpdate�� �ۼ��ϴ� ���� ����
        if(Input.GetButtonDown("Jump"))
        {
            rigid.AddForce(Vector3.up * 50, ForceMode.Impulse);  // ������ ����� ũ��� ForceMode�� ������� ���� �ְڴ�.
            Debug.Log(rigid.velocity);
        }

        Vector3 vec = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        rigid.AddForce(vec, ForceMode.Impulse);

        rigid.AddTorque(Vector3.down, ForceMode.Impulse);    // ������ ������ ������ �Ͽ� ȸ������ ����
    }

    // �ݶ��̴��� ��� �浹�ϰ� ���� �� ȣ��
    private void OnTriggerStay(Collider other)
    {
        if(other.name == "Cube")
        {
            rigid.AddForce(Vector3.up * 2, ForceMode.Impulse);
        }
    }

    public void Jump()
    {
        rigid.AddForce(Vector3.up * 20, ForceMode.Impulse);
    }


    void Update()
    {
        
    }
}
