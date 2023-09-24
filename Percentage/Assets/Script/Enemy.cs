using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum EnemyType { Trace, Random, Boss };

    [Header("----- Component -----")]
    public Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator animator;
    public Rigidbody2D target;
    Collider2D col;
    Room room;

    [Header("----- Enemy Property -----")]
    public EnemyType enemyType;
    public float speed;
    public float health;
    public float maxHealth;
    bool isLive;
    bool isObjectCollision;
    Vector2 moveDir;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        room = gameObject.GetComponentInParent<Room>();
    }

    void OnEnable()
    {
        isLive = true;
        health = maxHealth;
        StartCoroutine(SetRandomMove());
    }

    void Start()
    {

    }

    void FixedUpdate()
    {
        // 죽었거나, 맞을때는 앞으로 못가도록 설정
        // 스탯창을 열었을 때는 멈추도록 설정
        if (!isLive || animator.GetCurrentAnimatorStateInfo(0).IsName("Hit")) return;
        if (GameManager.instance.isOpenStatus) return;

        EnemyMove();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // 무기의 경우에는 공격을 했을 때만 공격판정이 들어가도록
        if ((collision.CompareTag("Weapon") && GameManager.instance.skill[GameManager.instance.player.currentSkillIndex].isAttack) 
            || collision.CompareTag("Bullet") || collision.CompareTag("SkillBullet"))
        {
            float knockbackAmount = collision.CompareTag("SkillBullet") ? 10f : 3f;

            int role = GameManager.instance.player.role;
            Weapon weapon = GameManager.instance.weapon[GameManager.instance.player.currentWeaponIndex];
            health -= weapon.damage;

            if(health > 0)
            {
                animator.SetTrigger("Hit");
                StartCoroutine(KnockBack(knockbackAmount));
            }
            else
            {
                isLive = false;
                col.enabled = false;
                rigid.simulated = false;
                animator.SetBool("Dead", true);
                room.enemyCount--;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Object") || collision.collider.CompareTag("Wall"))
        {
            isObjectCollision = true;
        }
    }

    void Update()
    {
        
    }

    void LateUpdate()
    {
        // 기본은 오른쪽을 보고있다. 뒤집으려면 타겟(플레이어)가 적보다 왼쪽에 있어야 한다.
        spriteRenderer.flipX = target.position.x < rigid.position.x ? true : false;
    }

    void EnemyMove()
    {
        switch(enemyType)
        {
            case EnemyType.Trace:
                TraceMove();
                break;

            case EnemyType.Random:
                RandomMove();
                break;
        }
    }

    public void TraceMove()
    {
        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
        rigid.velocity = Vector2.zero;
    }

    public void RandomMove()
    {
        // 오브젝트에 부딪치면 방향 전환
        if (isObjectCollision)
        {
            StopCoroutine(SetRandomMove());
            StartCoroutine(SetRandomMove());
            isObjectCollision = false;
        }

        rigid.MovePosition(rigid.position + moveDir * speed * Time.fixedDeltaTime);
        rigid.velocity = Vector2.zero;
    }

    public void StopMove()
    {
        rigid.MovePosition(rigid.position);
    }

    IEnumerator SetRandomMove()
    {
        while (true)
        {
            int dirRandom = Random.Range(0, 4);
            switch (dirRandom)
            {
                case 0:
                    moveDir = Vector2.up;
                    break;
                case 1:
                    moveDir = Vector2.right;
                    break;
                case 2:
                    moveDir = Vector2.down;
                    break;
                case 3:
                    moveDir = Vector2.left;
                    break;
            }

            yield return new WaitForSeconds(3);
        }
    }

    IEnumerator KnockBack(float knockbackAmount)
    {
        // 하나의 물리 프레임 딜레이
        yield return new WaitForFixedUpdate();

        Vector3 playerPosition = GameManager.instance.player.transform.position;
        Vector3 dirVec = transform.position - playerPosition;
        rigid.AddForce(dirVec.normalized * knockbackAmount, ForceMode2D.Impulse);
    }

    void DeadAnimation()
    {
        gameObject.SetActive(false);

        int random = Random.Range(0, 10);
        if(random >= 0 && random < 3)
        {
            GameObject coin = GameManager.instance.objectPool.Get(0);
            coin.transform.position = transform.position;
        }
    }
}
