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
        // ���� ���ڰ� ������� ���� ������ ������� �����Ƿ� �ϴ� �Ⱥ��̰� ����
        gameObject.transform.localScale = Vector3.zero;

        // ���� �ڸ��� ���� ����Ʈ ����
        Bullet explosion = GameManager.instance.bulletPool.Get(10);
        explosion.transform.position = transform.position;

        // 0.5�� �� ����
        yield return new WaitForSeconds(0.5f);

        // �Ѵ� �����
        gameObject.SetActive(false);
        explosion.gameObject.SetActive(false);
    }
}
