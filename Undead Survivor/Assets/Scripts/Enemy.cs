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
    WaitForFixedUpdate wait;    // ���� FixedUpdate�� ����� ������ �ѹ� ��ٸ���
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

        // �׾��� ������ ������ ����� ���� �������� �ٽ� �ʱ�ȭ
        isLive = true;
        col.enabled = true;
        rigid.simulated = true;
        spriteRenderer.sortingOrder = 2;
        animator.SetBool("Dead", false);
        health = maxHealth; // �װ� �� ��Ȱ��ȭ �� ������Ʈ�� �ٽ� Ȱ��ȭ �� �� �ִ�ü������ ����
    }

    void FixedUpdate()
    {
        if (!GameManager.instance.isTimePassing) return;

        // �׾��ų�, �¾��� ���� ĵ��
        // �ִϸ������� ���̾� �ε���
        if (!isLive || animator.GetCurrentAnimatorStateInfo(0).IsName("Hit")) return;

        // transform.position: ������Ʈ�� ��ġ�� �ٷ� �̵���Ų��(�����̵� ����).
        // �̴� ��ġ�� �̵��� �� �μӵ� collider���� rigidbody�� ��ġ�� �ٽ� ����ϹǷ� �����ս��� ���ϵȴ�.

        // rigidbody.position: ���� �������� �ܰ迡 ������Ʈ �� �Ŀ� �̵���Ų��.
        // �̹� ���� �Ŀ� �̵���Ű�� ������ �� ���� �ӵ��� �����ս��� �����ش�.
        Vector2 directionVector = target.position - rigid.position;
        Vector2 nextVector = directionVector.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVector);
        rigid.velocity = Vector2.zero;  // ���� �ӵ��� �̵��� ������ ���� �ʵ��� �ӵ� ����
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Bullet") && isLive)
        {
            float damage = collision.GetComponent<Bullet>().damage;

            health -= damage;
            if(damage > 1f)
            {
                // �ӽŰ��� �˹� ���ٰŴ�
                StartCoroutine(KnockBack());
            }
            

            if(health > 0)
            {
                animator.SetTrigger("Hit");
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Hit);
            }
            else
            {
                // �������� ����, collider�� rigidbody �ʱ�ȭ
                isLive = false;
                col.enabled = false;
                rigid.simulated = false;
                spriteRenderer.sortingOrder = 1;
                animator.SetBool("Dead", true);

                // �÷��̾��� ų ���� ����ġ ȹ��
                GameManager.instance.kill++;
                GameManager.instance.GetExp();

                // ���� ���� ����, ���� �״� ȿ���� ���
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

        // �̵� �� �¿� ���� ����
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
