using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool isTouchTop;
    public bool isTouchBottom;
    public bool isTouchLeft;
    public bool isTouchRight;
    Animator animator;
    public SpriteRenderer spriteRenderer;

    public GameObject normalBullet;
    public GameObject powerBullet;
    public int power;
    public int maxPower;
    public int boomCount;
    public int maxBoomCount;
    public float shotDelay;
    public float maxShotDelay;

    public GameManager gameManager;
    public ObjectManager objectManager;
    public int life;
    public int score;
    public bool isDamaged;

    public GameObject boomAction;

    public GameObject[] followers;

    public bool[] joyControl;
    public bool isControl;
    public bool isButtonA;

    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        gameManager.UpdateBoomIcon(boomCount);
    }

    // 컴포넌트가 활성화 될 때 호출되는 생명주기 함수
    void OnEnable()
    {
        boomCount = 2;
        gameManager.UpdateBoomIcon(boomCount);
    }

    void Update()
    {
        // 움직이고, 총쏘고, 폭탄날리고, 재장전하고
        Move();
        if (Input.GetButtonDown("Fire1") || isButtonA)
        {
            Fire();
        }
        if (Input.GetButtonDown("Fire2") && boomCount > 0)
        {
            Boom();
        }
        Reload();
    }

    // 하단 조이판넬 이동 함수
    public void MoveJoyPanel(int type)
    {
        for(int i=0; i<8; i++)
        {
            joyControl[i] = i == type;  // 현재 인덱스가 누른 방향과 같은가?
        }
    }

    public void JoyDown()
    {
        isControl = true;
    }
    public void JoyUp()
    {
        isControl = false;
    }
    public void ButtonADown()
    {
        isButtonA = true;
    }
    public void ButtonAUp()
    {
        isButtonA = false;
    }
    public void ButtonBDown()
    {
        if(boomCount > 0)
        {
            Boom();
        }
    }

    void Move()
    {
        // 플레이어 이동
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // 조이판넬 이동 설정
        if (joyControl[0]) { horizontal = -1; vertical = 1;  }
        if (joyControl[1]) { horizontal = 0;  vertical = 1;  }
        if (joyControl[2]) { horizontal = 1;  vertical = 1;  }
        if (joyControl[3]) { horizontal = -1; vertical = 0;  }
        if (joyControl[4]) { horizontal = 1;  vertical = 0;  }
        if (joyControl[5]) { horizontal = -1; vertical = -1; }
        if (joyControl[6]) { horizontal = 0;  vertical = -1; }
        if (joyControl[7]) { horizontal = 1;  vertical = -1; }

        // border에 닿았고 그 방향으로 계속 이동중이면 멈추기 위해 값을 0으로 고정
        if ((isTouchLeft && horizontal == -1) || (isTouchRight && horizontal == 1) || !isControl)
        {
            horizontal = 0;
        }
        if ((isTouchTop && vertical == 1) || (isTouchBottom && vertical == -1) || !isControl)
        {
            vertical = 0;
        }

        Vector3 currentPosition = transform.position;
        Vector3 nextPosition = new Vector3(horizontal, vertical, 0) * 5 * Time.deltaTime;   // transform 이동 시에는 deltaTime 사용하기

        transform.position = currentPosition + nextPosition;

        // 플레이어 이동 애니메이션
        if (Input.GetButtonDown("Horizontal") || Input.GetButtonUp("Horizontal"))
        {
            animator.SetInteger("Input", (int)horizontal);
        }
    }

    void Fire()
    {
        // 재장전중이면 발사 안되도록 설정
        if (shotDelay < maxShotDelay) return;

        GameObject bullet, bulletLeft, bulletRight;
        Rigidbody2D rigid, rigidLeft, rigidRight;
        
        switch(power)
        {
            // 가운데서 한발
            case 1:
                bullet = objectManager.GetGameObject("playerNormalBullet");
                bullet.transform.position = transform.position;

                rigid = bullet.GetComponent<Rigidbody2D>();
                rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;

            // 좌우에서 한발씩
            case 2:
                bulletLeft = objectManager.GetGameObject("playerNormalBullet");
                bulletLeft.transform.position = transform.position + Vector3.left * 0.3f;
                bulletRight = objectManager.GetGameObject("playerNormalBullet");
                bulletRight.transform.position = transform.position + Vector3.right * 0.3f;

                rigidLeft = bulletLeft.GetComponent<Rigidbody2D>();
                rigidRight = bulletRight.GetComponent<Rigidbody2D>();
                rigidLeft.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidRight.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;

            // 좌우, 가운데에서 한발씩
            case 3:
                bulletLeft = objectManager.GetGameObject("playerNormalBullet");
                bulletLeft.transform.position = transform.position + Vector3.left * 0.3f;
                bulletRight = objectManager.GetGameObject("playerNormalBullet");
                bulletRight.transform.position = transform.position + Vector3.right * 0.3f;
                bullet = objectManager.GetGameObject("playerNormalBullet");
                bullet.transform.position = transform.position;

                rigidLeft = bulletLeft.GetComponent<Rigidbody2D>();
                rigidRight = bulletRight.GetComponent<Rigidbody2D>();
                rigid = bullet.GetComponent<Rigidbody2D>();
                rigidLeft.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidRight.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;

            // 좌우에서는 일반, 가운데에서는 강화로 한발씩
            default:
                bulletLeft = objectManager.GetGameObject("playerNormalBullet");
                bulletLeft.transform.position = transform.position + Vector3.left * 0.3f;
                bulletRight = objectManager.GetGameObject("playerNormalBullet");
                bulletRight.transform.position = transform.position + Vector3.right * 0.3f;
                bullet = objectManager.GetGameObject("playerPowerBullet");
                bullet.transform.position = transform.position;

                rigidLeft = bulletLeft.GetComponent<Rigidbody2D>();
                rigidRight = bulletRight.GetComponent<Rigidbody2D>();
                rigid = bullet.GetComponent<Rigidbody2D>();
                rigidLeft.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidRight.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
        }

        // 총 발사하면 재장전 (부와아아악 발사되는걸 막기 위함)
        shotDelay = 0;
    }

    void Reload()
    {
        shotDelay += Time.deltaTime;
    }

    void AddFollower()
    {
        followers[0].SetActive(true);
        followers[1].SetActive(true);
    }

    void Boom()
    {
        boomCount--;
        gameManager.UpdateBoomIcon(boomCount);
        boomAction.SetActive(true); // 필살기 액션 생성

        // 그 후 모든 적에게 큰 데미지 주기
        GameObject[] largeEnemies = objectManager.GetPool("enemyLarge");
        GameObject[] mediumEnemies = objectManager.GetPool("enemyMedium");
        GameObject[] smallEnemies = objectManager.GetPool("enemySmall");
        GameObject[] bossEnemy = objectManager.GetPool("boss");

        for (int i = 0; i < largeEnemies.Length; i++)
        {
            if (largeEnemies[i].activeSelf)
            {
                Enemy enemyComponents = largeEnemies[i].GetComponent<Enemy>();
                enemyComponents.Damaged(1000);
            }
        }
        for (int i = 0; i < mediumEnemies.Length; i++)
        {
            if (mediumEnemies[i].activeSelf)
            {
                Enemy enemyComponents = mediumEnemies[i].GetComponent<Enemy>();
                enemyComponents.Damaged(1000);
            }
        }
        for (int i = 0; i < smallEnemies.Length; i++)
        {
            if (smallEnemies[i].activeSelf)
            {
                Enemy enemyComponents = smallEnemies[i].GetComponent<Enemy>();
                enemyComponents.Damaged(1000);
            }
        }
        for (int i = 0; i < bossEnemy.Length; i++)
        {
            if (bossEnemy[i].activeSelf)
            {
                Enemy enemyComponents = bossEnemy[i].GetComponent<Enemy>();
                enemyComponents.Damaged(1000);
            }
        }

        // 그리고 적 총알도 삭제
        GameObject[] enemyNormalBullets = objectManager.GetPool("enemyNormalBullet");
        GameObject[] enemyLargeBullets = objectManager.GetPool("enemyLargeBullet");
        GameObject[] bossNormalBullets = objectManager.GetPool("bossNormalBullet");
        GameObject[] bossPowerBullets = objectManager.GetPool("bossPowerBullet");
        for (int i = 0; i < enemyNormalBullets.Length; i++)
        {
            if (enemyNormalBullets[i].activeSelf)
            {
                enemyNormalBullets[i].SetActive(false);
            }
        }
        for (int i = 0; i < enemyLargeBullets.Length; i++)
        {
            if (enemyLargeBullets[i].activeSelf)
            {
                enemyLargeBullets[i].SetActive(false);
            }
        }
        for (int i = 0; i < bossNormalBullets.Length; i++)
        {
            if (bossNormalBullets[i].activeSelf)
            {
                bossNormalBullets[i].SetActive(false);
            }
        }
        for (int i = 0; i < bossPowerBullets.Length; i++)
        {
            if (bossPowerBullets[i].activeSelf)
            {
                bossPowerBullets[i].SetActive(false);
            }
        }

        Invoke("DisappearBoomAction", 3f);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // 경계선에 닿으면 움직임을 멈추도록 설정
        if(collision.gameObject.tag == "Border")
        {
            switch(collision.gameObject.name)
            {
                case "Top":
                    isTouchTop = true;
                    break;
                case "Bottom":
                    isTouchBottom = true;
                    break;
                case "Left":
                    isTouchLeft = true;
                    break;
                case "Right":
                    isTouchRight = true;
                    break;
            }
        }

        // 적과 충돌하거나, 적의 탄환에 맞은 경우
        // 목숨 감소
        // 플레이어 리스폰은 게임매니저가 관리
        if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "EnemyBullet")
        {
            if (isDamaged) return;  // isDamaged: 맞은 상태. 즉, 무적상태이니 맞는 로직을 무시하겠다
            else
            {
                GameObject[] bossEnemy = objectManager.GetPool("boss");

                isDamaged = true;

                life--; // 목숨 감소
                gameManager.UpdateLifeIcon(life);   // 목숨 아이콘 업데이트
                power = 1;  // 파워 초기화

                gameObject.SetActive(false);    // 기체 사라짐
                followers[0].SetActive(false);
                followers[1].SetActive(false);  // 보조기체 사라짐

                gameManager.CallExplosion(transform.position, "Player");    // 피격 시 폭발 애니메이션

                // 보스를 제외하고 기체와 충돌한 오브젝트도 같이 삭제
                if (collision.gameObject != bossEnemy[0])
                {
                    collision.gameObject.SetActive(false);
                }

                // 목숨이 0이면 게임오버, 아니면 리스폰
                if (life == 0)
                {
                    gameManager.GameOver();
                }
                else
                {
                    gameManager.InvokeRespawnPlayer();
                }
            }
        }

        // 아이템 먹으면
        if (collision.gameObject.tag == "Item")
        {
            Item item = collision.gameObject.GetComponent<Item>();
            switch (item.itemType)
            {
                case "Coin":
                    score += 1000;
                    break;
                case "Power":
                    if(power < maxPower)
                    {
                        power++;
                        if(power == 5)
                        {
                            AddFollower();
                        }
                    }
                    else
                    {
                        score += 3000;
                    }
                    break;
                case "Boom":
                    if (boomCount < maxBoomCount)
                    {
                        boomCount++;
                        gameManager.UpdateBoomIcon(boomCount);
                    }
                    else
                    {
                        score += 5000;
                    }
                    break;
            }

            // 먹은 아이템은 삭제
            collision.gameObject.SetActive(false);
        }
    }

    void DisappearBoomAction()
    {
        boomAction.SetActive(false);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Border")
        {
            switch (collision.gameObject.name)
            {
                case "Top":
                    isTouchTop = false;
                    break;
                case "Bottom":
                    isTouchBottom = false;
                    break;
                case "Left":
                    isTouchLeft = false;
                    break;
                case "Right":
                    isTouchRight = false;
                    break;
            }
        }
    }


}
