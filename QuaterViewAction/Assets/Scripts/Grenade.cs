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
        // 던지고 3초뒤에 터지는 로직(멈춘 후 폭탄은 사라지고 효과는 나타나고)
        yield return new WaitForSeconds(3f);
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        meshObject.SetActive(false);
        effectObject.SetActive(true);

        // 구체 모양의 레이캐스팅에 있는 모든 적에게 데미지를 줌
        // 현 위치에서 반경 15안에 있는 모든 Enemy
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
