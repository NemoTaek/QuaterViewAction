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
            // 1�� ����
            case 0:
                Rush();
                yield return new WaitForSeconds(2f);
                break;

            // 5ȸ ��Ѹ���
            case 1:
                for (int i = 0; i < 5; i++)
                {
                    SpreadFire();
                    yield return new WaitForSeconds(0.5f);
                }
                break;

            // �������� 3��
            case 2:
                for (int i = 0; i < 3; i++)
                {
                    Sniping();
                    yield return new WaitForSeconds(1f);
                }
                break;
        }

        // ���� �� �� 0.5�� ����
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

            // �ش� �Ѿ��� �� �ѷ����� ��� ��ġ�� �ִ°�
            float bulletIndex = Mathf.PI * 2 * i / countPerCycle;

            // x��ǥ�� cos, y��ǥ�� sin
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
