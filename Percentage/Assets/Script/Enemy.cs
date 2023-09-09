using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator animator;
    public Rigidbody2D target;
    Collider2D collider;

    public float speed;
    public float health;
    public float maxHealth;
    bool isLive;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
    }

    void OnEnable()
    {
        isLive = true;
        health = maxHealth;
    }

    void Start()
    {
        
    }

    void FixedUpdate()
    {
        EnemyMove();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Weapon"))
        {
            Debug.Log("����!");
            health -= collision.GetComponent<Weapon>().damage;

            if(health > 0)
            {
                animator.SetTrigger("Hit");
                StartCoroutine(KnockBack());
            }
            else
            {
                isLive = false;
                collider.enabled = false;
                rigid.simulated = false;
                animator.SetBool("Dead", true);
            }
        }
    }

    void Update()
    {
        
    }

    void LateUpdate()
    {
        // �⺻�� �������� �����ִ�. ���������� Ÿ��(�÷��̾�)�� ������ ���ʿ� �־�� �Ѵ�.
        spriteRenderer.flipX = target.position.x < rigid.position.x ? true : false;
    }

    void EnemyMove()
    {
        // �׾��ų�, �������� ������ �������� ����
        if (!isLive || animator.GetCurrentAnimatorStateInfo(0).IsName("Hit")) return;

        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
        rigid.velocity = Vector2.zero;
    }

    IEnumerator KnockBack()
    {
        // �ϳ��� ���� ������ ������
        yield return new WaitForFixedUpdate();
        Vector3 playerPosition = GameManager.instance.player.transform.position;
        Vector3 dirVec = transform.position - playerPosition;
        rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);
    }
    void DeadAnimation()
    {
        gameObject.SetActive(false);
    }
}
