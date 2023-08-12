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
        // ���� ������
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

        // ���� �� ����
        Vector2 frontVec = new Vector2(rigid.position.x + (nextMove * 0.5f), rigid.position.y);

        // �������� üũ
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHitCliff = Physics2D.Raycast(frontVec, Vector3.down, 2, LayerMask.GetMask("Platform"));

        // �� üũ
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

    // ���� �����ϱ�
    // ������ �����Ҽ� ����
    void Think()
    {
        if(!isDead)
        {
            nextMove = Random.Range(-1, 2); // [min, max) ������ ���� ��
            float randomTime = Random.Range(2.0f, 5.0f);

            animator.SetInteger("walkSpeed", nextMove);
            if (nextMove != 0)
            {
                spriteRenderer.flipX = nextMove == 1;
            }

            // Invoke(func, time): func �Լ��� time �Ŀ� ����
            Invoke("Think", randomTime);
        }
    }

    // ������ȯ
    void Turn()
    {
        nextMove *= -1;
        spriteRenderer.flipX = nextMove == 1;

        CancelInvoke(); // �������� Invoke �Լ��� ����
        Think();
    }

    // ������ ����
    // ������ ���� ��� �ö󰬴ٰ� �޿� �������鼭 ������� ȿ��
    public void OnDamaged()
    {
        // ���� ����
        isDead = true;

        // �浹 �� ���� ����
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        // ������ �Ųٷ�
        spriteRenderer.flipY = true;

        // �浹 ����
        capsuleCollider.enabled = false;

        // �Ʒ��� �������鼭 ���������
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        // ���� �ʿ��� ��Ȱ��ȭ
        Invoke("DeActive", 5);
    }

    void DeActive()
    {
        gameObject.SetActive(false);
    }
}
