using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    public GameManager gameManager;
    Rigidbody2D rigid;
    public float maxSpeed;
    public float jumpPower;
    SpriteRenderer spriteRenderer;
    Animator animator;
    CapsuleCollider2D capsuleCollider;

    AudioSource audioSource;
    public AudioClip audioJump;
    public AudioClip audioAttack;
    public AudioClip audioDamaged;
    public AudioClip audioItem;
    public AudioClip audioDie;
    public AudioClip audioFinish;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {

    }

    void FixedUpdate()
    {
        // �̵� �ӵ�
        float horizontal = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * horizontal, ForceMode2D.Impulse);

        // �ִ� �̵��ӵ� ����
        if (rigid.velocity.x > maxSpeed) // ������ ����
        {
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        }
        else if (rigid.velocity.x < (-1) * maxSpeed)  // ���� ����
        {
            rigid.velocity = new Vector2((-1) * maxSpeed, rigid.velocity.y);
        }

        // �����ؼ� ������ ���� üũ
        if(rigid.velocity.y < 0)
        {
            // RayCast: ������Ʈ �˻��� ���� Ray�� ��� ���
            // Debug.DrawRay(): ������ �󿡼��� Ray�� �׷��ִ� �Լ�
            // RayCastHit: Ray�� ���� ������Ʈ
            Debug.DrawRay(new Vector2(rigid.position.x - 0.8f, rigid.position.y), Vector3.down, new Color(0, 1, 0));
            RaycastHit2D rayHitLeft = Physics2D.Raycast(new Vector2(rigid.position.x - 0.8f, rigid.position.y), Vector3.down, 2, LayerMask.GetMask("Platform"));
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
            RaycastHit2D rayHitMid = Physics2D.Raycast(rigid.position, Vector3.down, 2, LayerMask.GetMask("Platform"));
            Debug.DrawRay(new Vector2(rigid.position.x + 0.8f, rigid.position.y), Vector3.down, new Color(0, 1, 0));
            RaycastHit2D rayHitRight = Physics2D.Raycast(new Vector2(rigid.position.x + 0.8f, rigid.position.y), Vector3.down, 2, LayerMask.GetMask("Platform"));

            // collider: ray�� ��ǥ ������ �浹�� ������Ʈ
            // distance: ray�� ��ǥ������ ����� ���� �Ÿ�
            if (rayHitMid.collider != null && rayHitMid.distance < 1.0f)
            {
                animator.SetBool("isJumping", false);
            }
            else if(rayHitLeft.collider != null && rayHitLeft.distance < 0.5f)
            {
                animator.SetBool("isJumping", false);
            }
            else if (rayHitRight.collider != null && rayHitRight.distance < 0.5f)
            {
                animator.SetBool("isJumping", false);
            }
        }
    }

    void Update()
    {
        // ����
        if (Input.GetButtonDown("Jump") && !animator.GetBool("isJumping"))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            animator.SetBool("isJumping", true);
            PlaySound("JUMP");
        }

        // �������� ���� �� �ٷ� ���ߵ��� ����
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.00001f, rigid.velocity.y);
        }

        // ���� ��ȯ �� �ִϸ��̼�
        if (Input.GetButton("Horizontal"))
        {
            // flipX�� bool Ÿ��. true�� X���� ����
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") < 0;
        }

        // ������ �������� �����϶��� �ִϸ��̼� ��ȯ ���� ����
        if (rigid.velocity.normalized.x == 0)
        {
            animator.SetBool("isWalking", false);
        }
        else
        {
            animator.SetBool("isWalking", true);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // ���� �浹���� ��
        if(collision.gameObject.tag == "Enemy")
        {
            // �ϰ� �ӵ��� �ְ�, ������ �浹�ߴٸ� ��Ƽ� ����
            if(rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
            {
                OnAttack(collision.transform);
            }
            else
            {
                // �ǰݴ��ߴٸ� ª�� �ð��� ��������
                OnDamaged(collision.transform.position);
            }
        }
    }

    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }

    // ��Ƽ� �����ϴ� ����
    void OnAttack(Transform enemy)
    {
        gameManager.stagePoint += 500;

        // ������ ��¦ ���� �ߴ� ȿ��
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        // ���� �����
        PlaySound("ATTACK");

        // ���� ������ ����
        MoveEnemy moveEnemy = enemy.GetComponent<MoveEnemy>();
        moveEnemy.OnDamaged();
    }

    // ���� ����
    void OnDamaged(Vector2 enemyPosition)
    {
        // ü�� ����
        gameManager.HealthDown(100);

        // ���̾ PlayerDamaged �� ����
        gameObject.layer = 9;

        // �浹 �� ���� ����
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        // �÷��̾� ��ġ�� ���� ��ġ�� ���Ͽ� �ð����� ���� ����
        float collisionDirection = transform.position.x - enemyPosition.x;
        rigid.AddForce(new Vector2(collisionDirection, 1) * 7, ForceMode2D.Impulse);

        // ���� �����
        PlaySound("DAMAGED");

        // ���� �ִϸ��̼�
        animator.SetTrigger("damaged");

        // 1�� �Ŀ� ���� Ǯ��
        Invoke("OffDamaged", 1);
    }

    void OffDamaged()
    {
        gameObject.layer = 8;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    public void OnDie()
    {
        // �浹 �� ���� ����
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        // ������ �Ųٷ�
        spriteRenderer.flipY = true;

        // �浹 ����
        capsuleCollider.enabled = false;

        // �Ʒ��� �������鼭 ���������
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        // ��� �����
        PlaySound("DIE");

        Time.timeScale = 0; // ����!
        gameManager.restartButton.SetActive(true);  // ����� ��ư
    }

    void PlaySound(string action)
    {
        switch(action)
        {
            case "JUMP":
                audioSource.clip = audioJump;
                break;
            case "ATTACK":
                audioSource.clip = audioAttack;
                break;
            case "DAMAGED":
                audioSource.clip = audioDamaged;
                break;
            case "ITEM":
                audioSource.clip = audioItem;
                break;
            case "DIE":
                audioSource.clip = audioDie;
                break;
            case "FINISH":
                audioSource.clip = audioFinish;
                break;
        }
        audioSource.Play();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Coin")
        {
            gameManager.stagePoint += 100;
            PlaySound("ITEM");
            collision.gameObject.SetActive(false);
        }

        if (collision.gameObject.tag == "Finish")
        {
            PlaySound("FINISH");
            gameManager.NextStage();
        }
    }
}
