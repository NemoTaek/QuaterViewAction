using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum ItemType { Ammo, Coin, Grenade, Heart, Weapon}; // 아이템의 타입
    public ItemType type;   // 아이템의 종류
    public int value;       // 아이템의 값
    Rigidbody rigid;
    SphereCollider sphereCollider;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        // 아이템에는 sphereCollider가 2개 있는데 이렇게 작성하면 1개를 가져오게 되는데 무엇을 가져올까?
        // 유니티에서 설정한 순서대로 제일 위에있는 콜라이더를 가져오게 된다.
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
