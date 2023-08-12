using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    public Sprite[] sprites;
    Animator animator;

    public int health;
    public string enemyName;
    public int enemyScore;

    public GameObject normalBullet;
    public GameObject largeBullet;
    public float maxShotDelay;
    public float currentShotDelay;

    public GameObject player;
    public GameObject itemPower;
    public GameObject itemBoom;
    public GameObject itemCoin;

    public GameManager gameManager;
    public ObjectManager objectManager;

    public int patternIndex;
    public int patternCount;
    public int[] maxPatternCount;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if(enemyName == "Boss")
        {
            animator = GetComponent<Animator>();
        }
    }

    // ������Ʈ�� Ȱ��ȭ �� �� ȣ��Ǵ� �����ֱ� �Լ�
    void OnEnable()
    {
        if (enemyName == "EnemySmall")
        {
            health = 3;
        }

        if (enemyName == "EnemyMedium")
        {
            health = 10;
        }

        if (enemyName == "EnemyLarge")
        {
            health = 30;
        }

        if (enemyName == "Boss")
        {
            health = 3000;
            Invoke("Stop", 2);
        }
    }

    void Stop()
    {
        if (!gameObject.activeSelf) return;

        Rigidbody2D rigid = GetComponent<Rigidbody2D>();
        rigid.velocity = Vector2.zero;

        Invoke("Pattern", 2);
    }

    void Pattern()
    {
        // ���� ��� �� ���� �߻��ϸ� �ȵ�
        if(health <= 0) return;

        // ���� ���� ���� �� �� ���� �ʱ�ȭ �� ���� ���� �غ�
        patternCount = 0;
        patternIndex = patternIndex == 3 ? 0 : patternIndex + 1;

        // ���� ����
        switch (patternIndex)
        {
            case 0:
                FireForward();
                break;
            case 1:
                FireShot();
                break;
            case 2:
                FireArc();
                break;
            case 3:
                FireAround();
                break;
        }
    }

    void FireForward() {
        GameObject bulletL, bulletLL, bulletR, bulletRR;
        Rigidbody2D rigidL, rigidLL, rigidR, rigidRR;

        bulletLL = objectManager.GetGameObject("bossNormalBullet");
        bulletLL.transform.position = transform.position + new Vector3(-0.83f, -0.9f, 0);
        bulletL = objectManager.GetGameObject("bossNormalBullet");
        bulletL.transform.position = transform.position + new Vector3(-0.63f, -0.9f, 0);
        bulletR = objectManager.GetGameObject("bossNormalBullet");
        bulletR.transform.position = transform.position + new Vector3(0.63f, -0.9f, 0);
        bulletRR = objectManager.GetGameObject("bossNormalBullet");
        bulletRR.transform.position = transform.position + new Vector3(0.83f, -0.9f, 0);

        rigidL = bulletL.GetComponent<Rigidbody2D>();
        rigidR = bulletR.GetComponent<Rigidbody2D>();
        rigidLL = bulletLL.GetComponent<Rigidbody2D>();
        rigidRR = bulletRR.GetComponent<Rigidbody2D>();
        rigidL.AddForce(Vector2.down * 8, ForceMode2D.Impulse);
        rigidR.AddForce(Vector2.down * 8, ForceMode2D.Impulse);
        rigidLL.AddForce(Vector2.down * 8, ForceMode2D.Impulse);
        rigidRR.AddForce(Vector2.down * 8, ForceMode2D.Impulse);

        NextPattern("FireForward", 2);
    }

    void FireShot() {
        GameObject bullet;
        Rigidbody2D rigid;
        Vector2 directionVector, randomVector;

        for (int i=0; i<5; i++)
        {
            bullet = objectManager.GetGameObject("bossPowerBullet");
            bullet.transform.position = transform.position;

            directionVector = player.transform.position - transform.position;
            randomVector = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(0, 2f)); // �Ѿ��� ��ġ�� �ʰ� �������� �ο�
            directionVector += randomVector;
            directionVector.Normalize();

            rigid = bullet.GetComponent<Rigidbody2D>();
            rigid.AddForce(directionVector * 12, ForceMode2D.Impulse);
        }

        NextPattern("FireShot", 3.5f);
    }

    void FireArc() {
        GameObject bullet;
        Rigidbody2D rigid;
        Vector2 directionVector;

        bullet = objectManager.GetGameObject("bossNormalBullet");
        bullet.transform.position = transform.position;

        // sin �׷����� ���� [0, 2pi] �������� ���� �Դٰ��� �ϴ� �Ϳ��� �����Ͽ� �Ѿ� �߻�
        // ���� ���� �����Ͽ� �͸����� �ϴ� �ӵ��� ����
        // �߻� Ƚ���� Ȧ���� �ϸ� �������� �� �� �������� �ʱ� ������ ������ �ڸ����� ���� �� ���� ��
        float arcSpeed = Mathf.PI * 10 * patternCount;
        directionVector = new Vector2(Mathf.Sin(arcSpeed / maxPatternCount[patternIndex]), -1);
        directionVector.Normalize();

        rigid = bullet.GetComponent<Rigidbody2D>();
        rigid.AddForce(directionVector * 5, ForceMode2D.Impulse);

        NextPattern("FireArc", 0.2f);
    }

    void FireAround() {
        GameObject bullet;
        Rigidbody2D rigid;
        Vector2 directionVector;
        Vector3 rotationVector;
        int roundNum1 = 50;  // �������� �� �� �߻��� �Ѿ� ����
        int roundNum2 = 40;  // �������� �� �� �߻��� �Ѿ� ����
        int roundNum = patternCount % 2 == 0 ? roundNum1 : roundNum2;  // �������� �� �� �߻��� �Ѿ� ����

        for (int i = 0; i < roundNum; i++)
        {
            bullet = objectManager.GetGameObject("bossPowerBullet");
            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.identity;

            // �����·� �վ��� ������ �Ѿ��� ��ȣ�� �ο��Ͽ� sin/cos �Լ��� ����
            float aroundBulletIndex = Mathf.PI * 2 * i / roundNum;
            directionVector = new Vector2(Mathf.Sin(aroundBulletIndex), Mathf.Cos(aroundBulletIndex));
            directionVector.Normalize();

            rigid = bullet.GetComponent<Rigidbody2D>();
            rigid.AddForce(directionVector * 2, ForceMode2D.Impulse);

            // �������� �� �� �Ѿ��� ������ ���߱� ���Ͽ� �Ѿ� ȸ��
            rotationVector = Vector3.back * 360 * i / roundNum;
            bullet.transform.Rotate(rotationVector);
        }

        NextPattern("FireAround", 0.7f);
    }

    void NextPattern(string patternName, float interval)
    {
        // ������ ������ ī���� �ϰ�, ���� Ƚ���� ������ ���� ���� �߻� �غ�
        patternCount++;
        if (patternCount < maxPatternCount[patternIndex])
        {
            Invoke(patternName, interval);
        }
        else
        {
            Invoke("Pattern", 3);
        }
    }

    public void Damaged(int damage)
    {
        if (health <= 0) return;    // ü���� 0�̵Ǿ��µ� ���� �ʹ� �������� ������ ���� ó�� �Ǵ� ����ó��

        health -= damage;

        // �¾��� �� ü���� ���������� ��¦��¦, �ƴϸ� �ı�
        if(health > 0)
        {
            if (enemyName == "Boss")
            {
                animator.SetTrigger("OnHit");
            }
            else
            {
                // �Ϲ��� 0�� �ε���, �¾��� �� �Ͼ���� 1�� �ε���
                spriteRenderer.sprite = sprites[1];
                Invoke("ReturnSprite", 0.1f);
            }
        }
        else
        {
            // ���� �ø��� �� �ı�
            Player playerComponent = player.GetComponent<Player>();
            playerComponent.score += enemyScore;
            gameObject.SetActive(false);
            gameManager.CallExplosion(transform.position, enemyName);    // �ǰ� �� ���� �ִϸ��̼�
            CancelInvoke();

            // �ı� �� �������� ��������, �����ٸ� ��� �������� ����
            int itemRandom = Random.Range(0, 100);
            if(itemRandom >= 50 && itemRandom < 90)
            {
                itemCoin = objectManager.GetGameObject("itemCoin");
                itemCoin.transform.position = transform.position;
            }
            else if (itemRandom >= 90 && itemRandom < 97)
            {
                itemPower = objectManager.GetGameObject("itemPower");
                itemPower.transform.position = transform.position;
            }
            else if (itemRandom >= 97 && itemRandom < 100)
            {
                itemBoom = objectManager.GetGameObject("itemBoom");
                itemBoom.transform.position = transform.position;
            }

            // ������ �ı������� 3�� �� �������� Ŭ���� ���� ����
            if(enemyName == "Boss")
            {
                patternIndex = -1;
                Invoke("StageClear", 3);
            }
        }
    }

    void StageClear()
    {
        gameManager.StageEnd();
    }

    void ReturnSprite()
    {
        spriteRenderer.sprite = sprites[0];
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // ���� ȭ�� ������ ������ ���������
        if (collision.gameObject.tag == "BorderBullet" && enemyName != "Boss")
        {
            gameObject.SetActive(false);
        }
        // �c�˿� ������ �Ѿ��� �������, �������� �Ե���
        else if(collision.gameObject.tag == "PlayerBullet")
        {
            collision.gameObject.SetActive(false);
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            Damaged(bullet.damage);
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (enemyName == "Boss") return;
        // ���� źȯ�� ���� �ֱ�� �߻�
        Fire();
        Reload();
    }

    void Fire()
    {
        // �ֱⰡ ���� ������ ���ư�
        if (currentShotDelay < maxShotDelay) return;

        GameObject bullet, bulletLeft, bulletRight;
        Rigidbody2D rigid, rigidLeft, rigidRight;

        // ��ǥ�� ���� = ��ǥ�� ��ġ - �ڽ��� ��ġ
        // ���� ũ�⸦ 1�� ����� ���� ��������ȭ
        Vector3 directionVector = player.transform.position - transform.position;
        directionVector.Normalize();

        if (enemyName == "EnemySmall")
        {
            bullet = objectManager.GetGameObject("enemyNormalBullet");
            bullet.transform.position = transform.position;

            rigid = bullet.GetComponent<Rigidbody2D>();
            rigid.AddForce(directionVector * 6, ForceMode2D.Impulse);
        }

        if (enemyName == "EnemyMedium")
        {
            bulletLeft = objectManager.GetGameObject("enemyNormalBullet");
            bulletLeft.transform.position = transform.position + Vector3.left * 0.5f;
            bulletRight = objectManager.GetGameObject("enemyNormalBullet");
            bulletRight.transform.position = transform.position + Vector3.right * 0.5f;

            rigidLeft = bulletLeft.GetComponent<Rigidbody2D>();
            rigidRight = bulletRight.GetComponent<Rigidbody2D>();
            rigidLeft.AddForce(directionVector * 6, ForceMode2D.Impulse);
            rigidRight.AddForce(directionVector * 6, ForceMode2D.Impulse);
        }

        if (enemyName == "EnemyLarge")
        {
            bulletLeft = objectManager.GetGameObject("enemyNormalBullet");
            bulletLeft.transform.position = transform.position + Vector3.left * 0.5f;
            bulletRight = objectManager.GetGameObject("enemyNormalBullet");
            bulletRight.transform.position = transform.position + Vector3.right * 0.5f;
            bullet = objectManager.GetGameObject("enemyLargeBullet");
            bullet.transform.position = transform.position;

            rigidLeft = bulletLeft.GetComponent<Rigidbody2D>();
            rigidRight = bulletRight.GetComponent<Rigidbody2D>();
            rigid = bullet.GetComponent<Rigidbody2D>();
            rigidLeft.AddForce(directionVector * 6, ForceMode2D.Impulse);
            rigidRight.AddForce(directionVector * 6, ForceMode2D.Impulse);
            rigid.AddForce(directionVector * 6, ForceMode2D.Impulse);
        }

        currentShotDelay = 0;
    }

    void Reload()
    {
        currentShotDelay += Time.deltaTime;
    }
}
