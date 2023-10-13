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
        // �� ���� �ʱ�ȭ
        isLive = true;
        health = maxHealth;
    }

    protected virtual void Start()
    {
        // �����ǰ� 1�� �� �̵� ����
        StartCoroutine(MoveOneSecondAfter());
        if (enemyType == EnemyType.Random) StartCoroutine(SetRandomMove());
    }

    void FixedUpdate()
    {
        // �׾��ų�, �������� �˹� ȿ���� ���� ������ �������� ����
        // ���ӿ��� �ÿ��� �������̵��� ����
        if (!isLive || animator.GetCurrentAnimatorStateInfo(0).IsName("Hit") || GameManager.instance.player.isDead) return;

        if (enemyType == EnemyType.Trace && moveStart)   TraceMove();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // ������ ��쿡�� ������ ���� ���� ���������� ������
        if ((collision.CompareTag("Weapon") && GameManager.instance.skill[GameManager.instance.player.currentSkillIndex].isAttack) 
            || collision.CompareTag("Bullet") || collision.CompareTag("SkillBullet"))
        {
            float knockbackAmount = collision.CompareTag("SkillBullet") ? 10f : 3f;
            Skill skill = GameManager.instance.skill[GameManager.instance.player.currentSkillIndex];

            // �⺻������ ���Ϳ� ���� �������� �÷��̾��� ���ݷ¿� ������. ���ݷ� ������ �÷��̾� ��ũ��Ʈ�� �ִ�.
            // ���� �������� �߰��Ǵ� ���ݷ����� �ְ�, ��ų �������� �÷��̾��� ���ݷ¿� x% �������� ����.
            float finalDamage = GameManager.instance.player.power * ((skill.damage + skill.upgradeDamage[skill.level]) / 100);
            health -= finalDamage;

            if (health > 0)
            {
                animator.SetTrigger("Hit");

                // ���� ȿ���� ���� �ʾҴٸ� �˹�ȿ��
                if (!collision.gameObject.GetComponent<Bullet>().isPenetrate || !collision.gameObject.GetComponent<Weapon>().isPenetrate)    StartCoroutine(KnockBack(knockbackAmount));

                // ���ο� ȿ���� �Ծ��ٸ� Ȯ�������� ������ �����̵���
                if ((!collision.gameObject.GetComponent<Bullet>().isSlow || !collision.gameObject.GetComponent<Weapon>().isSlow) && !enemySlow) StartCoroutine(MoveSlow());
            }
            else
            {
                isLive = false;
                col.enabled = false;
                rigid.simulated = false;
                animator.SetBool("Dead", true);
                room.enemyCount--;

                // ������ ������ ���� ü�� UI ��Ȱ��ȭ
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
        // �⺻�� �������� �����ִ�. ���������� Ÿ��(�÷��̾�)�� ������ ���ʿ� �־�� �Ѵ�.
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
        // �ϳ��� ���� ������ ������
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
