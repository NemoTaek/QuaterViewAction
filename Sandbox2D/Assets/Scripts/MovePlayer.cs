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
        // 이동 속도
        float horizontal = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * horizontal, ForceMode2D.Impulse);

        // 최대 이동속도 설정
        if(rigid.velocity.x > maxSpeed) // 오른쪽 방향
        {
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        } else if (rigid.velocity.x < (-1) * maxSpeed)  // 왼쪽 방향
        {
            rigid.velocity = new Vector2((-1) * maxSpeed, rigid.velocity.y);
        }
    }

    void Update()
    {
        // 움직임을 멈출 때 바로 멈추도록 설정
        if(Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.00001f, rigid.velocity.y);
        }

        // 방향 전환 시 애니메이션
        if (Input.GetButtonDown("Horizontal"))
        {
            // flipX는 bool 타입. true면 X방향 반전
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") < 0;
        }

        // 가만히 있을때와 움직일때의 애니메이션 전환 변수 설정
        if(rigid.velocity.normalized.x == 0)
        {
            animator.SetBool("isWalking", false);
        } else
        {
            animator.SetBool("isWalking", true);
        }
    }
}
