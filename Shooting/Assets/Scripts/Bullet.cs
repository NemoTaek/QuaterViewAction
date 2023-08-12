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
        // �Ѿ��� �� ���� ���� �����ǵ��� ����
        if (collision.gameObject.tag == "BorderBullet")
        {
            gameObject.SetActive(false);
        }
    }
}
