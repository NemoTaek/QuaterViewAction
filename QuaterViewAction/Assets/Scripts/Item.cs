using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum ItemType { Ammo, Coin, Grenade, Heart, Weapon}; // �������� Ÿ��
    public ItemType type;   // �������� ����
    public int value;       // �������� ��
    Rigidbody rigid;
    SphereCollider sphereCollider;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        // �����ۿ��� sphereCollider�� 2�� �ִµ� �̷��� �ۼ��ϸ� 1���� �������� �Ǵµ� ������ �����ñ�?
        // ����Ƽ���� ������ ������� ���� �����ִ� �ݶ��̴��� �������� �ȴ�.
        sphereCollider = GetComponent<SphereCollider>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        transform.Rotate(Vector3.up * 20 * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            rigid.isKinematic = true;
            sphereCollider.enabled = false;
        }
    }
}
