using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

public class Enemy : MonoBehaviour
{
    [Header("------- Component -------")]
    public Rigidbody2D target;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    public RuntimeAnimatorController[] animatorController;
    Animator animator;
    WaitForFixedUpdate wait;    // 다음 FixedUpdate가 실행될 때까지 한번 기다린다
    Collider2D col;

    [Header("------- logic -------")]
    public float speed;
    public float health;
    public float maxHealth;
    bool isLive;

    public void Init(SpawnData data)
    {
        animator.runtimeAnimatorController = animatorController[data.spriteType];
        speed = data.speed;
        maxHealth = data.health;
        health = data.health;
    }

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        wait = new WaitForFixedUpdate();
        col = GetComponent<Collider2D>();
    }

    void OnEnable()
    {
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();

        // 죽었던 상태의 설정을 살았을 때의 설정으로 다시 초기화
        isLive = true;
        col.enabled = true;
        rigid.simulated = true;
        spriteRenderer.sortingOrder = 2;
        animator.SetBool("Dead", false);
        health = maxHealth; // 죽고난 후 비활성화 된 오브젝트를 다시 활성화 할 때 최대체력으로 생성
    }

    void FixedUpdate()
    {
        if (!GameManager.instance.isTimePassing) return;

        // 죽었거나, 맞았을 때는 캔슬
        // 애니메이터의 레이어 인덱스
        if (!isLive || animator.GetCurrentAnimatorStateInfo(0).IsName("Hit")) return;

        // transform.position: 오브젝트의 위치를 바로 이동시킨다(순간이동 개념).
        // 이는 위치가 이동된 후 부속된 collider들이 rigidbody의 위치를 다시 계산하므로 퍼포먼스가 저하된다.

        // rigidbody.position: 다음 물리연산 단계에 업데이트 된 후에 이동시킨다.
        // 이미 계산된 후에 이동시키기 때문에 더 나은 속도와 퍼포먼스를 보여준다.
        Vector2 directionVector = target.position - rigid.position;
        Vector2 nextVector = directionVector.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVector);
        rigid.velocity = Vector2.zero;  // 물리 속도가 이동에 영향을 주지 않도록 속도 제거
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Bullet") && isLive)
        {
            float damage = collision.GetComponent<Bullet>().damage;

            health -= damage;
            if(damage > 1f)
            {
                // 머신건은 넉백 안줄거다
                StartCoroutine(KnockBack());
            }
            

            if(health > 0)
            {
                animator.SetTrigger("Hit");
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Hit);
            }
            else
            {
                // 죽은상태 설정, collider와 rigidbody 초기화
                isLive = false;
                col.enabled = false;
                rigid.simulated = false;
                spriteRenderer.sortingOrder = 1;
                animator.SetBool("Dead", true);

                // 플레이어의 킬 수와 경험치 획득
                GameManager.instance.kill++;
                GameManager.instance.GetExp();

                // 게임 중일 때만, 적이 죽는 효과음 출력
                if (GameManager.instance.isTimePassing)
                {
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead);
                }
            }
        }
    }

    void Update()
    {
        if (!GameManager.instance.isTimePassing) return;
        if (!isLive) return;

        // 이동 시 좌우 방향 설정
        if (target.position.x < rigid.position.x)
        {
            spriteRenderer.flipX = true;
        }
        else if (target.position.x > rigid.position.x)
        {
            spriteRenderer.flipX = false;
        }
    }

    IEnumerator KnockBack()
    {
        yield return wait;
        Vector3 playerPosition = GameManager.instance.player.transform.position;
        Vector3 knockbackVector = transform.position - playerPosition;
        knockbackVector.Normalize();
        rigid.AddForce(knockbackVector * 3, ForceMode2D.Impulse);
    }

    void Dead()
    {
        gameObject.SetActive(false);
        
        GameObject coin = GameManager.instance.pool.GetPool(3);
        coin.transform.position = transform.position;
    }
}
