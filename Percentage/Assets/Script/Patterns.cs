using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patterns
{
    public Enemy enemy;

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

        enemy.rigid.velocity = rushDir * 5f;
    }

    public void SpreadFire()
    {
        int countPerCycle = 10;
        for (int i = 0; i < countPerCycle; i++)
        {
            EnemyBullet bossBullet = GameManager.instance.bulletPool.Get(1, 0).GetComponent<EnemyBullet>();
            Rigidbody2D bulletRigid = bossBullet.GetComponent<Rigidbody2D>();
            bossBullet.transform.position = enemy.transform.position;

            // �ش� �Ѿ��� �� �ѷ����� ��� ��ġ�� �ִ°�
            float bulletIndex = Mathf.PI * 2 * i / countPerCycle;

            // x��ǥ�� cos, y��ǥ�� sin
            Vector2 spreadDir = new Vector2(Mathf.Cos(bulletIndex), Mathf.Sin(bulletIndex));
            spreadDir.Normalize();

            bulletRigid.velocity = spreadDir * 3f;
        }
    }

    public void Sniping(Rigidbody2D target)
    {
        EnemyBullet bossBullet = GameManager.instance.bulletPool.Get(1, 0).GetComponent<EnemyBullet>();
        Rigidbody2D bulletRigid = bossBullet.GetComponent<Rigidbody2D>();
        bossBullet.transform.position = enemy.transform.position;

        Vector2 dirVec = target.position - enemy.rigid.position;
        bulletRigid.velocity = dirVec.normalized * 10f;
    }

    public void EnemyShot(Vector2 shotDir, float shotSpeed)
    {
        EnemyBullet enemyBullet = GameManager.instance.bulletPool.Get(1, 0).GetComponent<EnemyBullet>();
        Rigidbody2D bulletRigid = enemyBullet.GetComponent<Rigidbody2D>();
        enemyBullet.transform.position = enemy.transform.position;

        bulletRigid.velocity = shotDir.normalized * shotSpeed;
    }

    public IEnumerator JumpAttack()
    {
        Vector3 currentPosition = enemy.transform.position;
        Vector3 jumpPosition = currentPosition;
        while (jumpPosition.y < currentPosition.y + 1f)
        {
            jumpPosition += Vector3.up * 0.05f;
            enemy.transform.position = jumpPosition;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }

        while (jumpPosition.y > currentPosition.y)
        {
            jumpPosition -= Vector3.up * 0.05f;
            enemy.transform.position = jumpPosition;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }

        // ������ ������ �� �Ÿ��� 2 �̳��� ������ ������ �ֱ�
        Vector3 distance = enemy.target.position - enemy.rigid.position;
        if (distance.magnitude < 1.5) enemy.StartCoroutine(GameManager.instance.player.PlayerDamaged());
    }

    public void RandomFire()
    {
        EnemyBullet bossBullet = GameManager.instance.bulletPool.Get(1, 0).GetComponent<EnemyBullet>();

        // �߻� ���� ���� ����
        float shotDirX = Random.Range(-1.0f, 1.0f);
        float shotDirY = Random.Range(-1.0f, 1.0f);
        Vector2 shotDir = new Vector2(shotDirX, shotDirY).normalized;

        // ���� ���� �߻�
        // Atan2(y, x): y / x ����Ͽ� ���� ���� ����. �� �� ������ ��ȯ
        float shotDeg = Mathf.Atan2(shotDir.y, shotDir.x) * Mathf.Rad2Deg - 90;
        bossBullet.transform.position = enemy.transform.position;
        bossBullet.transform.rotation = Quaternion.Euler(0, 0, shotDeg);
        bossBullet.GetComponent<Rigidbody2D>().velocity = shotDir * 2;
    }

    public void SpawnSubEnemy(int spawnEnemyId)
    {
        // �÷��̾� �ֺ� 8���� ����
        Vector3[] subEnemyPosition = new Vector3[8];
        int index = 0;
        for (float dx = -1f; dx <= 1; dx += 1)
        {
            for (float dy = -1f; dy <= 1; dy += 1)
            {
                if (dx == 0 && dy == 0) continue;
                subEnemyPosition[index] = new Vector3(enemy.transform.position.x + dx, enemy.transform.position.y + dy, 1);
                index++;
            }
        }

        // ��������Ʈ 8���⿡ ��ġ
        for (int i = 0; i < 8; i++)
        {
            enemy.room.InstallSpawnPoint(enemy.room.transform, subEnemyPosition[i]);
        }

        // �� ��ȯ
        enemy.room.InitEnemies(spawnEnemyId);
    }
}
