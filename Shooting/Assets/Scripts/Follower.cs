using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEngine;

public class Follower : MonoBehaviour
{
    public ObjectManager objectManager;

    public Vector3 followerPosition;
    public int followerDelay;
    public Transform parent;
    public Queue<Vector3> parentPosition;
    public GameObject[] followers;

    void Awake()
    {
        parentPosition = new Queue<Vector3>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        Watch();
        FollowerMove();
        if (Input.GetButtonDown("Fire1"))
        {
            FollowerFire();
        }
    }

    void Watch()
    {
        /* 방법 1: 보조기체가 추가될때마다 꼬리처럼 늘어나게 해서 따라오는 방법
        // 부모의 위치를 큐에 넣고, 보조 기체 위치를 설정
        if(!parentPosition.Contains(parent.position))
        {
            parentPosition.Enqueue(parent.position);
        }

        // 본체가 움직이고 설정한 딜레이만큼 후에 따라온다.
        if(parentPosition.Count > followerDelay)
        {
            followerPosition = parentPosition.Dequeue();
        }
        else
        {
            followerPosition = parent.position;
        }
        */
    }

    void FollowerMove()
    {
        //transform.position = followerPosition;

        // 방법 2: 그냥 한번에 보조 기체를 나오게 해서 옆에 붙이는 방법
        followers[0].transform.position = parent.position + Vector3.left * 0.8f + Vector3.down * 0.3f;
        followers[1].transform.position = parent.position + Vector3.right * 0.8f + Vector3.down * 0.3f;
    }

    void FollowerFire()
    {
        GameObject bullet;
        Rigidbody2D rigid;

        bullet = objectManager.GetGameObject("followerBullet");
        bullet.transform.position = transform.position;

        rigid = bullet.GetComponent<Rigidbody2D>();
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
    }
}
