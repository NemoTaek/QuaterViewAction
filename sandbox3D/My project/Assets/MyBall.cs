using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyBall : MonoBehaviour
{
    Rigidbody rigid;
    
    void Start()
    {
        rigid = GetComponent<Rigidbody>();

        //rigid.velocity = Vector3.right; // 오른쪽 방향으로 속도를 주겠다
        //rigid.velocity = new Vector3(2, 4, 3); // 지정한 방향으로 속도를 주겠다.

        
    }

    void FixedUpdate()
    {
        // rigid 관련 코드는 FixedUpdate에 작성하는 것을 권장
        if(Input.GetButtonDown("Jump"))
        {
            rigid.AddForce(Vector3.up * 50, ForceMode.Impulse);  // 지정한 방향과 크기로 ForceMode의 방식으로 힘을 주겠다.
            Debug.Log(rigid.velocity);
        }

        Vector3 vec = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        rigid.AddForce(vec, ForceMode.Impulse);

        rigid.AddTorque(Vector3.down, ForceMode.Impulse);    // 지정한 방향을 축으로 하여 회전력을 생성
    }

    // 콜라이더가 계속 충돌하고 있을 때 호출
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
