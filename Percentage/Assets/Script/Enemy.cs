using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum EnemyType { Trace, Random, Stand, Boss };

    [Header("----- Component -----")]
    public Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator animator;
    public Rigidbody2D target;
    Collider2D col;
    public Room room;
    public SpriteRenderer statusEffect;
    public Patterns patterns;

    [Header("----- Enemy Property -----")]
    public EnemyData.EnemyType type;
    public int id;
    public float speed;
    public float health;
    public float maxHealth;
    public Sprite image;

    public bool isLive;
    public bool isPatternPlaying;
    public bool isObjectCollision;
    public bool stopRandomMove;
    public bool moveStart;
    public bool enemySlow;
    public Vector2 moveDirection;
    public bool isTrace;
    public bool isFly;

    [Header("----- Boss Property -----")]
    public bool isSpawnSubEnemy;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        room = gameObject.GetComponentInParent<Room>();

        patterns = new Patterns();
    }

    public void Init(EnemyData data)
    {
        id = data.enemyId;
        type = data.enemyType;
        speed = data.enemySpeed;
        maxHealth = data.enemyMaxHealth;
        image = data.enemyImage;
        animator.runtimeAnimatorController = data.enemyAnimator;

        // �� ���� �ʱ�ȭ
        isLive = true;
        health = maxHealth;
        isFly = data.isFly;
    }

    // virtual: ��ӹ޴� �ڽ� ��ũ��Ʈ���� �������̵� �� �� �ֵ��� ������ִ� Ű����
    // ������� ���߿� �ϳ��� abstract Ű�����ε� �̴� �ڽ� ��ũ��Ʈ���� �ݵ�� ������ ���־�� �ϴ� �Լ��� ���δ�.
    protected virtual void Start()
    {
        // �����ǰ� 1�� �� �̵� ����
        StartCoroutine(MoveOneSecondAfter());

        if (type == EnemyData.EnemyType.Trace) isTrace = true;
        if (type == EnemyData.EnemyType.Random) StartCoroutine(SetRandomMove());
    }

    void FixedUpdate()
    {
        // �׾��ų�, �������� �˹� ȿ���� ���� ������ �������� ����
        // ���ӿ��� �ÿ��� �������̵��� ����
        if (!isLive || !moveStart || (type != EnemyData.EnemyType.Boss && animator.GetCurrentAnimatorStateInfo(0).IsName("Hit")) || GameManager.instance.player.isDead) return;

        if (isTrace)   TraceMove();

        if (!isPatternPlaying) StartCoroutine(EnemyPattern());
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // ���� �����ε� ��ֹ��� �浹�� ���� �浹 ����, �� �ܿ��� �浹 ó��
        if (isFly && collision.collider.CompareTag("Object"))
        {
            Physics2D.IgnoreLayerCollision(7, 8, true);
        }
        else
        {
            Physics2D.IgnoreLayerCollision(7, 8, false);
        }
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
            EnemyDamaged(finalDamage);

            // ���� ȿ���� ���� �ʾҴٸ� �˹�ȿ�� (������ �˹� �ȸ�������)
            // ���ο� ȿ���� �Ծ��ٸ� Ȯ�������� ������ �����̵���
            if(isLive)
            {
                if (collision.CompareTag("Bullet") || collision.CompareTag("SkillBullet"))
                {
                    if (type != EnemyData.EnemyType.Boss && type != EnemyData.EnemyType.Stand && !collision.gameObject.GetComponent<Bullet>().isPenetrate) StartCoroutine(KnockBack(knockbackAmount));
                    if (!enemySlow && collision.gameObject.GetComponent<Bullet>().isSlow) SetSlowAttack();
                }
                else if (collision.CompareTag("Weapon"))
                {
                    if (type != EnemyData.EnemyType.Boss && type != EnemyData.EnemyType.Stand && !collision.gameObject.GetComponent<Weapon>().isPenetrate) StartCoroutine(KnockBack(knockbackAmount));
                    if (!enemySlow && collision.gameObject.GetComponent<Weapon>().isSlow) SetSlowAttack();
                }
            }
        }

        //if (col.isTrigger)
        //{
        //    if (collision.CompareTag("Wall"))
        //    {
        //        if (type == EnemyData.EnemyType.Random)
        //        {
        //            // ����!
        //            stopRandomMove = true;

        //            // �浹�� �ݴ�������� �ٽ� ���ư�
        //            moveDirection *= -1;
        //            rigid.AddForce(moveDirection);
        //            rigid.velocity = moveDirection * speed;

        //            // �ٽ� ���� ����
        //            StartCoroutine(SetRandomMove());
        //        }
        //    }
        //}
        
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
        moveDirection = dirVec;
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
        stopRandomMove = false;

        while (!stopRandomMove)
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

            moveDirection = moveDir;
            rigid.AddForce(moveDir);
            rigid.velocity = moveDir * speed;

            yield return new WaitForSeconds(2);
        }
    }

    public void EnemyDamaged(float damage)
    {
        health -= damage;

        if (health > 0)
        {
            animator.SetTrigger("Hit");
        }
        else
        {
            // �� ���
            isLive = false;
            col.enabled = false;
            rigid.simulated = false;
            animator.SetBool("Dead", true);
            room.enemyCount--;
            GameManager.instance.player.killEnemyCount++;

            // ������ ������ ���� ü�� UI ��Ȱ��ȭ
            if (room.roomType == Room.RoomType.Boss)
            {
                GameManager.instance.ui.bossUI.SetActive(false);
            }
        }
    }

    IEnumerator KnockBack(float knockbackAmount)
    {
        // �ϳ��� ���� ������ ������
        yield return new WaitForFixedUpdate();

        Vector3 playerPosition = GameManager.instance.player.transform.position;
        Vector3 dirVec = transform.position - playerPosition;
        rigid.drag = 5;
        rigid.AddForce(dirVec.normalized * knockbackAmount, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.5f);
        rigid.drag = 0;
    }

    void SetSlowAttack()
    {
        // �ϴ� 20% Ȯ���� ���ο� �ɸ���
        int random = Random.Range(0, 10);
        if(random < 2) StartCoroutine(MoveSlow(2.5f));
    }

    public IEnumerator MoveSlow(float time)
    {
        enemySlow = true;

        // �� �Ӹ� ���� ���ο� ���� ǥ�� �߰�
        statusEffect.sprite = GameManager.instance.statusEffectIcon[0];
        speed /= 2;

        yield return new WaitForSeconds(time);

        statusEffect.sprite = null;
        speed *= 2;

        enemySlow = false;
    }

    void DropEnemyReward(Vector3 dropPos)
    {
        int random = Random.Range(0, 10);
        if (random >= 0 && random < 3)
        {
            GameObject coin = Instantiate(GameManager.instance.objectPool.prefabs[1], room.roomReward.transform);
            coin.transform.position = transform.position + dropPos;
        }
    }

    void DeadAnimation()
    {
        // �����ָӴ� �йи�� ������ �ߵ�
        Familiar[] familiars = GameManager.instance.player.familiar.GetComponentsInChildren<Familiar>();
        if(familiars.Length > 0)
        {
            foreach(Familiar fam in familiars)
            {
                if(fam.id == 0) fam.DropCoin();
            }
        }

        gameObject.SetActive(false);

        // ������ ���� ������ 10ȸ ����, �ƴϸ� 1���� ����
        if (type == EnemyData.EnemyType.Boss)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector3 spreadPosition = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                DropEnemyReward(spreadPosition);
            }
        }
        else DropEnemyReward(Vector3.zero);
    }

    IEnumerator EnemyPattern()
    {
        isPatternPlaying = true;
        patterns.enemy = this;

        switch (id)
        {
            case 0:
                // �⺻������ ���� ������� ��
                // ������ ���� ����.
                break;
            case 1:
                // �⺻������ �������� �̵��ϴ� ��
                // ������ ���� ����.
                break;
            case 2:
                // �⺻������ ������ �ִ� ��
                // ������ ���� ����.
                break;
            case 3:
                // 1ź ����
                int patternRandom = Random.Range(0, 3);
                if (patternRandom == 0)
                {
                    // 1�� ����
                    patterns.Rush();
                    yield return new WaitForSeconds(2f);
                }
                else if (patternRandom == 1)
                {
                    // 5ȸ ��Ѹ���
                    for (int i = 0; i < 5; i++)
                    {
                        patterns.SpreadFire();
                        yield return new WaitForSeconds(0.5f);
                    }
                }
                else if (patternRandom == 2)
                {
                    // �������� 3��
                    for (int i = 0; i < 3; i++)
                    {
                        patterns.Sniping(target);
                        yield return new WaitForSeconds(1f);
                    }
                }

                // ���� �� �� 0.5�� ����
                rigid.velocity = Vector3.zero;
                rigid.MovePosition(rigid.position);
                yield return new WaitForSeconds(0.5f);

                isTrace = true;
                yield return new WaitForSeconds(2f);
                isTrace = false;

                break;
            case 4:
                patterns.EnemyShot(moveDirection, 2);
                yield return new WaitForSeconds(2);
                break;
            case 5:
                // �÷��̾� �ֺ��� ���� �������� �ϱ�
                if (moveDirection.magnitude < 1.5)
                {
                    isTrace = false;
                    StartCoroutine(patterns.JumpAttack());
                    yield return new WaitForSeconds(3);
                    isTrace = true;
                }
                break;
            case 6:
                // �߰� ����(?)
                // ����ƺ�: ü�� ���� ü���� ���� ���Ϸ� �������� �ֺ��� ���������� ��ȯ
                // �⺻ ����: �� 100% ~ 50%: ��������, 50% ~ 0%: ¤ ��Ѹ���

                if (health / maxHealth >= 0.5f)
                {
                    patterns.Sniping(target);
                    yield return new WaitForSeconds(3);
                }
                else
                {
                    if (!isSpawnSubEnemy)
                    {
                        // �ֺ��� �������� ��ȯ
                        patterns.SpawnSubEnemy(7);
                        isSpawnSubEnemy = true;
                    }

                    for (int i = 0; i < 100; i++)
                    {
                        patterns.RandomFire();
                        yield return new WaitForSeconds(0.1f);
                    }

                    yield return new WaitForSeconds(5f);
                }
                
                break;
        }

        isPatternPlaying = false;

        yield return null;
    }
}
