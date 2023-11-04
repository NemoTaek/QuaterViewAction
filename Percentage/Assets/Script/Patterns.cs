using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patterns
{
    public Rigidbody2D rigid;
    public Transform transform;

    public void Rush()
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

    public void SpreadFire()
    {
        int countPerCycle = 10;
        for (int i = 0; i < countPerCycle; i++)
        {
            EnemyBullet bossBullet = GameManager.instance.bulletPool.Get(1, 0).GetComponent<EnemyBullet>();
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

    public void Sniping(Rigidbody2D target)
    {
        EnemyBullet bossBullet = GameManager.instance.bulletPool.Get(1, 0).GetComponent<EnemyBullet>();
        Rigidbody2D bulletRigid = bossBullet.GetComponent<Rigidbody2D>();
        bossBullet.transform.position = transform.position;

        Vector2 dirVec = target.position - rigid.position;
        bulletRigid.velocity = dirVec.normalized * 10f;
    }

    public void EnemyShot(Vector2 shotDir, float shotSpeed)
    {
        EnemyBullet enemyBullet = GameManager.instance.bulletPool.Get(1, 0).GetComponent<EnemyBullet>();
        Rigidbody2D bulletRigid = enemyBullet.GetComponent<Rigidbody2D>();
        enemyBullet.transform.position = transform.position;

        bulletRigid.velocity = shotDir.normalized * shotSpeed;
    }
}
