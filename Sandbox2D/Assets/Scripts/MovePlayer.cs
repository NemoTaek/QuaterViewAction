using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    Rigidbody2D rigid;
    public float maxSpeed;
    SpriteRenderer spriteRenderer;
    Animator animator;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
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
        if(rigid.velocity.x > maxSpeed) // ������ ����
        {
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        } else if (rigid.velocity.x < (-1) * maxSpeed)  // ���� ����
        {
            rigid.velocity = new Vector2((-1) * maxSpeed, rigid.velocity.y);
        }
    }

    void Update()
    {
        // �������� ���� �� �ٷ� ���ߵ��� ����
        if(Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.00001f, rigid.velocity.y);
        }

        // ���� ��ȯ �� �ִϸ��̼�
        if (Input.GetButtonDown("Horizontal"))
        {
            // flipX�� bool Ÿ��. true�� X���� ����
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") < 0;
        }

        // ������ �������� �����϶��� �ִϸ��̼� ��ȯ ���� ����
        if(rigid.velocity.normalized.x == 0)
        {
            animator.SetBool("isWalking", false);
        } else
        {
            animator.SetBool("isWalking", true);
        }
    }
}
