using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public bool isRotate;

    void Update()
    {
        if(isRotate)
        {
            transform.Rotate(Vector3.forward * 10);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // 총알이 저 위로 가면 삭제되도록 설정
        if (collision.gameObject.tag == "BorderBullet")
        {
            gameObject.SetActive(false);
        }
    }
}
