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
        /* ��� 1: ������ü�� �߰��ɶ����� ����ó�� �þ�� �ؼ� ������� ���
        // �θ��� ��ġ�� ť�� �ְ�, ���� ��ü ��ġ�� ����
        if(!parentPosition.Contains(parent.position))
        {
            parentPosition.Enqueue(parent.position);
        }

        // ��ü�� �����̰� ������ �����̸�ŭ �Ŀ� ����´�.
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

        // ��� 2: �׳� �ѹ��� ���� ��ü�� ������ �ؼ� ���� ���̴� ���
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
