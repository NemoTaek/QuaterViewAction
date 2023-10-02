using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    bool isPatternPlaying;
    bool isTrace;

    void Start()
    {
        
    }

    void FixedUpdate()
    {
        if (GameManager.instance.player.isDead) return;
        if (isTrace) TraceMove();
        if (!isPatternPlaying)  StartCoroutine(BossPattern());
    }

    void Update()
    {
        
    }

    IEnumerator BossPattern()
    {
        isPatternPlaying = true;
        int patternRandom = Random.Range(0, 3);
        switch(patternRandom)
        {
            // 1초 돌진
            case 0:
                Rush();
                yield return new WaitForSeconds(2f);
                break;

            // 5회 흩뿌리기
            case 1:
                for (int i = 0; i < 5; i++)
                {
                    SpreadFire();
                    yield return new WaitForSeconds(0.5f);
                }
                break;

            // 스나이핑 3번
            case 2:
                for (int i = 0; i < 3; i++)
                {
                    Sniping();
                    yield return new WaitForSeconds(1f);
                }
                break;
        }

        // 패턴 본 후 0.5초 정지
        rigid.velocity = Vector3.zero;
        rigid.MovePosition(rigid.position);
        yield return new WaitForSeconds(0.5f);

        isTrace = true;
        yield return new WaitForSeconds(2f);
        isTrace = false;


        isPatternPlaying = false;
    }

    void Rush()
    {
        int dirRandom = Random.Range(0, 4);
        Vector2 rushDir = Vector2.zero;
        switch (dirRandom)
        {
            case 0:
                rushDir = Vector2.up;
                break;
            case 1:
                rushDir = Vector2.right;
                break;
            case 2:
                rushDir = Vector2.down;
                break;
            case 3:
                rushDir = Vector2.left;
                break;
        }

        rigid.velocity = rushDir * 5f;
    }

    void SpreadFire()
    {
        int countPerCycle = 10;
        for (int i = 0; i < countPerCycle; i++)
        {
            EnemyBullet bossBullet = GameManager.instance.bulletPool.Get(11).GetComponent<EnemyBullet>();
            Rigidbody2D bulletRigid = bossBullet.GetComponent<Rigidbody2D>();
            bossBullet.transform.position = transform.position;

            // 해당 총알이 원 둘레에서 어느 위치에 있는가
            float bulletIndex = Mathf.PI * 2 * i / countPerCycle;

            // x좌표는 cos, y좌표는 sin
            Vector2 spreadDir = new Vector2(Mathf.Cos(bulletIndex), Mathf.Sin(bulletIndex));
            spreadDir.Normalize();

            bulletRigid.velocity = spreadDir * 3f;
        }
    }

    void Sniping()
    {
        EnemyBullet bossBullet = GameManager.instance.bulletPool.Get(11).GetComponent<EnemyBullet>();
        Rigidbody2D bulletRigid = bossBullet.GetComponent<Rigidbody2D>();
        bossBullet.transform.position = transform.position;

        Vector2 dirVec = target.position - rigid.position;
        bulletRigid.velocity = dirVec.normalized * 10f;
    }
}
