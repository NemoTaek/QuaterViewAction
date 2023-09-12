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
    Room room;

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
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        room = gameObject.GetComponentInParent<Room>();
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
        StartCoroutine(EnemyMove());
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Weapon") || collision.CompareTag("Bullet"))
        {
            int role = GameManager.instance.player.role;
            Weapon weapon = GameManager.instance.weapon[role];
            health -= weapon.damage;

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
                room.enemyCount--;
            }
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

    IEnumerator EnemyMove()
    {
        // 소환 후 1초는 여유시간
        yield return new WaitForSeconds(1);

        // 죽었거나, 맞을때는 앞으로 못가도록 설정
        if (!isLive || animator.GetCurrentAnimatorStateInfo(0).IsName("Hit")) yield break;

        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
        rigid.velocity = Vector2.zero;
    }

    IEnumerator KnockBack()
    {
        // 하나의 물리 프레임 딜레이
        yield return new WaitForFixedUpdate();
        Vector3 playerPosition = GameManager.instance.player.transform.position;
        Vector3 dirVec = transform.position - playerPosition;
        rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);
    }

    void DeadAnimation()
    {
        gameObject.SetActive(false);

        int random = Random.Range(0, 10);
        if(random >= 0 && random < 3)
        {
            GameObject coin = GameManager.instance.ObjectPool.Get(2);
            coin.transform.position = transform.position;
        }
        
    }
}
