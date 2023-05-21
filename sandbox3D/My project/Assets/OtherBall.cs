using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherBall : MonoBehaviour
{
    // 오브젝트의 재질 접근은 MeshRenderer을 통해서 접근
    MeshRenderer mesh;
    Material mat;

    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        mat = mesh.material;
    }

    // 물리적인 충돌이 가해졌을 때 호출되는 함수
    // Collision: 충돌 정보 클래스
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "MyBall")
        {
            mat.color = new Color(0, 0, 0);
        }
    }

    // 물리적인 충돌이 진행되는 중에 호출되는 함수
    private void OnCollisionStay(Collision collision)
    {
        
    }

    // 물리적인 충돌이 끝났을 때 호출되는 함수
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.name == "MyBall")
        {
            mat.color = new Color(1, 1, 1);
        }
    }
}
