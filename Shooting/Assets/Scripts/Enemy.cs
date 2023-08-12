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

    // 컴포넌트가 활성화 될 때 호출되는 생명주기 함수
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
        // 보스 사망 시 패턴 발사하면 안됨
        if(health <= 0) return;

        // 다음 패턴 진입 시 전 패턴 초기화 및 다음 패턴 준비
        patternCount = 0;
        patternIndex = patternIndex == 3 ? 0 : patternIndex + 1;

        // 패턴 진행
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
            randomVector = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(0, 2f)); // 총알이 겹치지 않게 랜덤값을 부여
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

        // sin 그래프를 보면 [0, 2pi] 구간에서 값이 왔다갔다 하는 것에서 착안하여 총알 발사
        // 파이 값을 조절하여 와리가리 하는 속도를 조절
        // 발사 횟수를 홀수로 하면 나누었을 때 딱 떨어지지 않기 때문에 고정된 자리에서 피할 수 없게 됨
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
        int roundNum1 = 50;  // 범위공격 할 때 발사할 총알 개수
        int roundNum2 = 40;  // 범위공격 할 때 발사할 총알 개수
        int roundNum = patternCount % 2 == 0 ? roundNum1 : roundNum2;  // 범위공격 할 때 발사할 총알 개수

        for (int i = 0; i < roundNum; i++)
        {
            bullet = objectManager.GetGameObject("bossPowerBullet");
            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.identity;

            // 원형태러 뿜어져 나오는 총알의 번호를 부여하여 sin/cos 함수에 대입
            float aroundBulletIndex = Mathf.PI * 2 * i / roundNum;
            directionVector = new Vector2(Mathf.Sin(aroundBulletIndex), Mathf.Cos(aroundBulletIndex));
            directionVector.Normalize();

            rigid = bullet.GetComponent<Rigidbody2D>();
            rigid.AddForce(directionVector * 2, ForceMode2D.Impulse);

            // 범위공격 할 때 총알의 방향을 맞추기 위하여 총알 회전
            rotationVector = Vector3.back * 360 * i / roundNum;
            bullet.transform.Rotate(rotationVector);
        }

        NextPattern("FireAround", 0.7f);
    }

    void NextPattern(string patternName, float interval)
    {
        // 패턴을 했으면 카운팅 하고, 지정 횟수가 끝나면 다음 패턴 발사 준비
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
        if (health <= 0) return;    // 체력이 0이되었는데 총을 너무 빨리쏴서 여러개 맞은 처리 되는 예외처리

        health -= damage;

        // 맞았을 때 체력이 남아있으면 반짝반짝, 아니면 파괴
        if(health > 0)
        {
            if (enemyName == "Boss")
            {
                animator.SetTrigger("OnHit");
            }
            else
            {
                // 일반은 0번 인덱스, 맞았을 때 하얀색은 1번 인덱스
                spriteRenderer.sprite = sprites[1];
                Invoke("ReturnSprite", 0.1f);
            }
        }
        else
        {
            // 점수 올리고 적 파괴
            Player playerComponent = player.GetComponent<Player>();
            playerComponent.score += enemyScore;
            gameObject.SetActive(false);
            gameManager.CallExplosion(transform.position, enemyName);    // 피격 시 폭발 애니메이션
            CancelInvoke();

            // 파괴 후 아이템을 떨굴건지, 떨군다면 어떤걸 떨굴건지 설정
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

            // 보스를 파괴했으면 3초 후 스테이지 클리어 로직 실행
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
        // 적이 화면 밖으로 나가면 사라지도록
        if (collision.gameObject.tag == "BorderBullet" && enemyName != "Boss")
        {
            gameObject.SetActive(false);
        }
        // 촣알에 맞으면 총알이 사라지고, 데미지를 입도록
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
        // 적이 탄환을 일정 주기로 발사
        Fire();
        Reload();
    }

    void Fire()
    {
        // 주기가 되지 않으면 돌아가
        if (currentShotDelay < maxShotDelay) return;

        GameObject bullet, bulletLeft, bulletRight;
        Rigidbody2D rigid, rigidLeft, rigidRight;

        // 목표의 방향 = 목표의 위치 - 자신의 위치
        // 그후 크기를 1로 만들기 위해 단위벡터화
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
