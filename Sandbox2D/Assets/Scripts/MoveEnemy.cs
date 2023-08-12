using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveEnemy : MonoBehaviour
{
    Rigidbody2D rigid;
    public int nextMove;
    SpriteRenderer spriteRenderer;
    Animator animator;
    CapsuleCollider2D capsuleCollider;
    bool isDead;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        isDead = false;

        Think();
    }

    void Start()
    {

    }

    void FixedUpdate()
    {
        // 적의 움직임
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

        // 적의 앞 벡터
        Vector2 frontVec = new Vector2(rigid.position.x + (nextMove * 0.5f), rigid.position.y);

        // 낭떠러지 체크
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHitCliff = Physics2D.Raycast(frontVec, Vector3.down, 2, LayerMask.GetMask("Platform"));

        // 벽 체크
        Debug.DrawRay(frontVec, Vector3.right * nextMove, new Color(0, 1, 0));
        RaycastHit2D rayHitWall = Physics2D.Raycast(frontVec, Vector3.right * nextMove, 0.5f, LayerMask.GetMask("Platform"));

        if (!isDead && (rayHitCliff.collider == null || rayHitWall.collider != null))
        {
            Turn();
        }
    }

    void Update()
    {
    }

    // 어디로 움직일까
    // 죽으면 생각할수 없지
    void Think()
    {
        if(!isDead)
        {
            nextMove = Random.Range(-1, 2); // [min, max) 범위의 랜덤 값
            float randomTime = Random.Range(2.0f, 5.0f);

            animator.SetInteger("walkSpeed", nextMove);
            if (nextMove != 0)
            {
                spriteRenderer.flipX = nextMove == 1;
            }

            // Invoke(func, time): func 함수를 time 후에 실행
            Invoke("Think", randomTime);
        }
    }

    // 방향전환
    void Turn()
    {
        nextMove *= -1;
        spriteRenderer.flipX = nextMove == 1;

        CancelInvoke(); // 실행중인 Invoke 함수를 해제
        Think();
    }

    // 밟혀서 제거
    // 밟히면 위로 잠깐 올라갔다가 쭈욱 내려가면서 사라지는 효과
    public void OnDamaged()
    {
        // 죽은 상태
        isDead = true;

        // 충돌 시 색상 변경
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        // 밟히면 거꾸로
        spriteRenderer.flipY = true;

        // 충돌 제거
        capsuleCollider.enabled = false;

        // 아래로 내려가면서 사라지도록
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        // 적을 맵에서 비활성화
        Invoke("DeActive", 5);
    }

    void DeActive()
    {
        gameObject.SetActive(false);
    }
}
