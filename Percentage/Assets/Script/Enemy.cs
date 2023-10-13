using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

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
    public bool isLive;
    public bool isObjectCollision;
    public bool moveStart;

    public bool enemySlow;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        room = gameObject.GetComponentInParent<Room>();
    }

    protected virtual void OnEnable()
    {
        // 적 상태 초기화
        isLive = true;
        health = maxHealth;
    }

    protected virtual void Start()
    {
        // 스폰되고 1초 후 이동 시작
        StartCoroutine(MoveOneSecondAfter());
        if (enemyType == EnemyType.Random) StartCoroutine(SetRandomMove());
    }

    void FixedUpdate()
    {
        // 죽었거나, 맞을때는 넉백 효과를 위해 앞으로 못가도록 설정
        // 게임오버 시에도 못움직이도록 설정
        if (!isLive || animator.GetCurrentAnimatorStateInfo(0).IsName("Hit") || GameManager.instance.player.isDead) return;

        if (enemyType == EnemyType.Trace && moveStart)   TraceMove();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // 무기의 경우에는 공격을 했을 때만 공격판정이 들어가도록
        if ((collision.CompareTag("Weapon") && GameManager.instance.skill[GameManager.instance.player.currentSkillIndex].isAttack) 
            || collision.CompareTag("Bullet") || collision.CompareTag("SkillBullet"))
        {
            float knockbackAmount = collision.CompareTag("SkillBullet") ? 10f : 3f;
            Skill skill = GameManager.instance.skill[GameManager.instance.player.currentSkillIndex];

            // 기본적으로 몬스터에 들어가는 데미지는 플레이어의 공격력에 따른다. 공격력 공식은 플레이어 스크립트에 있다.
            // 무기 데미지를 추가되는 공격력으로 넣고, 스킬 데미지는 플레이어의 공격력에 x% 데미지로 들어간다.
            float finalDamage = GameManager.instance.player.power * ((skill.damage + skill.upgradeDamage[skill.level]) / 100);
            health -= finalDamage;

            if (health > 0)
            {
                animator.SetTrigger("Hit");

                // 투과 효과를 먹지 않았다면 넉백효과
                if (!collision.gameObject.GetComponent<Bullet>().isPenetrate || !collision.gameObject.GetComponent<Weapon>().isPenetrate)    StartCoroutine(KnockBack(knockbackAmount));

                // 슬로우 효과를 먹었다면 확률적으로 느리게 움직이도록
                if ((!collision.gameObject.GetComponent<Bullet>().isSlow || !collision.gameObject.GetComponent<Weapon>().isSlow) && !enemySlow) StartCoroutine(MoveSlow());
            }
            else
            {
                isLive = false;
                col.enabled = false;
                rigid.simulated = false;
                animator.SetBool("Dead", true);
                room.enemyCount--;

                // 보스가 죽으면 보스 체력 UI 비활성화
                if(room.roomType == Room.RoomType.Boss)
                {
                    GameManager.instance.ui.bossUI.SetActive(false);
                }
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Object") || collision.collider.CompareTag("Wall"))
        {
            if (enemyType == EnemyType.Random)
            {
                StopCoroutine(SetRandomMove());
                StartCoroutine(SetRandomMove());
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

    IEnumerator MoveOneSecondAfter()
    {
        yield return new WaitForSeconds(1);
        moveStart = true;
    }

    public void TraceMove()
    {
        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
        rigid.velocity = Vector2.zero;
    }

    public void StopMove()
    {
        rigid.MovePosition(rigid.position);
    }

    IEnumerator SetRandomMove()
    {
        yield return new WaitForSeconds(1);
        Vector2 moveDir = Vector2.zero;

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

            rigid.AddForce(moveDir);
            rigid.velocity = moveDir * speed;

            yield return new WaitForSeconds(2);
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

    IEnumerator MoveSlow()
    {
        enemySlow = true;

        speed /= 2;
        yield return new WaitForSeconds(2.5f);
        speed *= 2;

        enemySlow = false;
    }

    void DeadAnimation()
    {
        gameObject.SetActive(false);

        int random = Random.Range(0, 10);
        if(random >= 0 && random < 3)
        {
            GameObject coin = Instantiate(GameManager.instance.objectPool.prefabs[1], room.roomReward.transform);
            coin.transform.position = transform.position;
        }
    }
}
