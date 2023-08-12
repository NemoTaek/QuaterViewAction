using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public GameObject meshObject;
    public GameObject effectObject;
    public Rigidbody rigid;

    IEnumerator Explosion()
    {
        // ������ 3�ʵڿ� ������ ����(���� �� ��ź�� ������� ȿ���� ��Ÿ����)
        yield return new WaitForSeconds(3f);
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        meshObject.SetActive(false);
        effectObject.SetActive(true);

        // ��ü ����� ����ĳ���ÿ� �ִ� ��� ������ �������� ��
        // �� ��ġ���� �ݰ� 15�ȿ� �ִ� ��� Enemy
        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, 5, Vector3.up, 0, LayerMask.GetMask("Enemy"));

        foreach(RaycastHit hitObject in rayHits)
        {
            hitObject.transform.GetComponent<Enemy>().HitByGrenade(transform.position);
        }

        Destroy(gameObject, 5);
    }


    void Start()
    {
        StartCoroutine(Explosion());
    }

    void Update()
    {
        
    }
}
