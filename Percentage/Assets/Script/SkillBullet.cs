using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBullet : Bullet
{
    void Awake()
    {

    }

    void OnEnable()
    {
        if (id == 8)
        {
            gameObject.transform.localScale = Vector3.one;
            StartCoroutine(LandMine());
        }
    }

    void Start()
    {

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if(id == 8)
            {
                StartCoroutine(ExplodeMine());
            }

            gameObject.SetActive(false);
            transform.position = transform.parent.position;
        }
    }

    void Update()
    {

    }

    IEnumerator LandMine()
    {
        yield return new WaitForSeconds(5);
        if(gameObject.activeSelf) StartCoroutine(ExplodeMine());
    }

    IEnumerator ExplodeMine()
    {
        // 먼저 지뢰가 사라지면 다음 로직이 실행되지 않으므로 일단 안보이게 설정
        gameObject.transform.localScale = Vector3.zero;

        // 지뢰 자리에 폭발 이펙트 생성
        Bullet explosion = GameManager.instance.bulletPool.Get(10);
        explosion.transform.position = transform.position;

        // 0.5초 후 폭발
        yield return new WaitForSeconds(0.5f);

        // 둘다 사라져
        gameObject.SetActive(false);
        explosion.gameObject.SetActive(false);
    }
}
