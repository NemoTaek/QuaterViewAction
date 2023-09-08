using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator animator;
    public Rigidbody2D target;

    public float speed;
    public float health;
    public float maxHealth;
    bool isLive;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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
        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
        rigid.velocity = Vector2.zero;
    }

    void Dead()
    {

    }
}
