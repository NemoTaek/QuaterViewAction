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
        // 이동 속도
        float horizontal = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * horizontal, ForceMode2D.Impulse);

        // 최대 이동속도 설정
        if (rigid.velocity.x > maxSpeed) // 오른쪽 방향
        {
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        }
        else if (rigid.velocity.x < (-1) * maxSpeed)  // 왼쪽 방향
        {
            rigid.velocity = new Vector2((-1) * maxSpeed, rigid.velocity.y);
        }

        // 점프해서 내려올 때만 체크
        if(rigid.velocity.y < 0)
        {
            // RayCast: 오브젝트 검색을 위해 Ray를 쏘는 방식
            // Debug.DrawRay(): 에디터 상에서만 Ray를 그려주는 함수
            // RayCastHit: Ray에 닿은 오브젝트
            Debug.DrawRay(new Vector2(rigid.position.x - 0.8f, rigid.position.y), Vector3.down, new Color(0, 1, 0));
            RaycastHit2D rayHitLeft = Physics2D.Raycast(new Vector2(rigid.position.x - 0.8f, rigid.position.y), Vector3.down, 2, LayerMask.GetMask("Platform"));
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
            RaycastHit2D rayHitMid = Physics2D.Raycast(rigid.position, Vector3.down, 2, LayerMask.GetMask("Platform"));
            Debug.DrawRay(new Vector2(rigid.position.x + 0.8f, rigid.position.y), Vector3.down, new Color(0, 1, 0));
            RaycastHit2D rayHitRight = Physics2D.Raycast(new Vector2(rigid.position.x + 0.8f, rigid.position.y), Vector3.down, 2, LayerMask.GetMask("Platform"));

            // collider: ray가 목표 지점에 충돌한 오브젝트
            // distance: ray가 목표지점에 닿았을 때의 거리
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
        // 점프
        if (Input.GetButtonDown("Jump") && !animator.GetBool("isJumping"))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            animator.SetBool("isJumping", true);
            PlaySound("JUMP");
        }

        // 움직임을 멈출 때 바로 멈추도록 설정
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.00001f, rigid.velocity.y);
        }

        // 방향 전환 시 애니메이션
        if (Input.GetButton("Horizontal"))
        {
            // flipX는 bool 타입. true면 X방향 반전
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") < 0;
        }

        // 가만히 있을때와 움직일때의 애니메이션 전환 변수 설정
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
        // 적과 충돌했을 때
        if(collision.gameObject.tag == "Enemy")
        {
            // 하강 속도가 있고, 위에서 충돌했다면 밟아서 제거
            if(rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
            {
                OnAttack(collision.transform);
            }
            else
            {
                // 피격당했다면 짧은 시간의 무적상태
                OnDamaged(collision.transform.position);
            }
        }
    }

    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }

    // 밟아서 제거하는 상태
    void OnAttack(Transform enemy)
    {
        gameManager.stagePoint += 500;

        // 밟으면 살짝 위로 뜨는 효과
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        // 공격 오디오
        PlaySound("ATTACK");

        // 적은 밟혀서 제거
        MoveEnemy moveEnemy = enemy.GetComponent<MoveEnemy>();
        moveEnemy.OnDamaged();
    }

    // 무적 상태
    void OnDamaged(Vector2 enemyPosition)
    {
        // 체력 감소
        gameManager.HealthDown(100);

        // 레이어를 PlayerDamaged 로 변경
        gameObject.layer = 9;

        // 충돌 시 색상 변경
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        // 플레이어 위치와 적의 위치를 비교하여 팅겨지는 방향 설정
        float collisionDirection = transform.position.x - enemyPosition.x;
        rigid.AddForce(new Vector2(collisionDirection, 1) * 7, ForceMode2D.Impulse);

        // 피해 오디오
        PlaySound("DAMAGED");

        // 무적 애니메이션
        animator.SetTrigger("damaged");

        // 1초 후에 무적 풀림
        Invoke("OffDamaged", 1);
    }

    void OffDamaged()
    {
        gameObject.layer = 8;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    public void OnDie()
    {
        // 충돌 시 색상 변경
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        // 밟히면 거꾸로
        spriteRenderer.flipY = true;

        // 충돌 제거
        capsuleCollider.enabled = false;

        // 아래로 내려가면서 사라지도록
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        // 사망 오디오
        PlaySound("DIE");

        Time.timeScale = 0; // 멈춰!
        gameManager.restartButton.SetActive(true);  // 재시작 버튼
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
