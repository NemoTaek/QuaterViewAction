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

    // ������Ʈ�� Ȱ��ȭ �� �� ȣ��Ǵ� �����ֱ� �Լ�
    void OnEnable()
    {
        boomCount = 2;
        gameManager.UpdateBoomIcon(boomCount);
    }

    void Update()
    {
        // �����̰�, �ѽ��, ��ź������, �������ϰ�
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

    // �ϴ� �����ǳ� �̵� �Լ�
    public void MoveJoyPanel(int type)
    {
        for(int i=0; i<8; i++)
        {
            joyControl[i] = i == type;  // ���� �ε����� ���� ����� ������?
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
        // �÷��̾� �̵�
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // �����ǳ� �̵� ����
        if (joyControl[0]) { horizontal = -1; vertical = 1;  }
        if (joyControl[1]) { horizontal = 0;  vertical = 1;  }
        if (joyControl[2]) { horizontal = 1;  vertical = 1;  }
        if (joyControl[3]) { horizontal = -1; vertical = 0;  }
        if (joyControl[4]) { horizontal = 1;  vertical = 0;  }
        if (joyControl[5]) { horizontal = -1; vertical = -1; }
        if (joyControl[6]) { horizontal = 0;  vertical = -1; }
        if (joyControl[7]) { horizontal = 1;  vertical = -1; }

        // border�� ��Ұ� �� �������� ��� �̵����̸� ���߱� ���� ���� 0���� ����
        if ((isTouchLeft && horizontal == -1) || (isTouchRight && horizontal == 1) || !isControl)
        {
            horizontal = 0;
        }
        if ((isTouchTop && vertical == 1) || (isTouchBottom && vertical == -1) || !isControl)
        {
            vertical = 0;
        }

        Vector3 currentPosition = transform.position;
        Vector3 nextPosition = new Vector3(horizontal, vertical, 0) * 5 * Time.deltaTime;   // transform �̵� �ÿ��� deltaTime ����ϱ�

        transform.position = currentPosition + nextPosition;

        // �÷��̾� �̵� �ִϸ��̼�
        if (Input.GetButtonDown("Horizontal") || Input.GetButtonUp("Horizontal"))
        {
            animator.SetInteger("Input", (int)horizontal);
        }
    }

    void Fire()
    {
        // ���������̸� �߻� �ȵǵ��� ����
        if (shotDelay < maxShotDelay) return;

        GameObject bullet, bulletLeft, bulletRight;
        Rigidbody2D rigid, rigidLeft, rigidRight;
        
        switch(power)
        {
            // ����� �ѹ�
            case 1:
                bullet = objectManager.GetGameObject("playerNormalBullet");
                bullet.transform.position = transform.position;

                rigid = bullet.GetComponent<Rigidbody2D>();
                rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;

            // �¿쿡�� �ѹ߾�
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

            // �¿�, ������� �ѹ߾�
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

            // �¿쿡���� �Ϲ�, ��������� ��ȭ�� �ѹ߾�
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

        // �� �߻��ϸ� ������ (�ο;ƾƾ� �߻�Ǵ°� ���� ����)
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
        boomAction.SetActive(true); // �ʻ�� �׼� ����

        // �� �� ��� ������ ū ������ �ֱ�
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

        // �׸��� �� �Ѿ˵� ����
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
        // ��輱�� ������ �������� ���ߵ��� ����
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

        // ���� �浹�ϰų�, ���� źȯ�� ���� ���
        // ��� ����
        // �÷��̾� �������� ���ӸŴ����� ����
        if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "EnemyBullet")
        {
            if (isDamaged) return;  // isDamaged: ���� ����. ��, ���������̴� �´� ������ �����ϰڴ�
            else
            {
                GameObject[] bossEnemy = objectManager.GetPool("boss");

                isDamaged = true;

                life--; // ��� ����
                gameManager.UpdateLifeIcon(life);   // ��� ������ ������Ʈ
                power = 1;  // �Ŀ� �ʱ�ȭ

                gameObject.SetActive(false);    // ��ü �����
                followers[0].SetActive(false);
                followers[1].SetActive(false);  // ������ü �����

                gameManager.CallExplosion(transform.position, "Player");    // �ǰ� �� ���� �ִϸ��̼�

                // ������ �����ϰ� ��ü�� �浹�� ������Ʈ�� ���� ����
                if (collision.gameObject != bossEnemy[0])
                {
                    collision.gameObject.SetActive(false);
                }

                // ����� 0�̸� ���ӿ���, �ƴϸ� ������
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

        // ������ ������
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

            // ���� �������� ����
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
