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

        // 적 상태 초기화
        isLive = true;
        health = maxHealth;
        isFly = data.isFly;
    }

    // virtual: 상속받는 자식 스크립트에서 오버라이딩 할 수 있도록 허락해주는 키워드
    // 따라오는 것중에 하나가 abstract 키워드인데 이는 자식 스크립트에서 반드시 재정의 해주어야 하는 함수에 쓰인다.
    protected virtual void Start()
    {
        // 스폰되고 1초 후 이동 시작
        StartCoroutine(MoveOneSecondAfter());

        if (type == EnemyData.EnemyType.Trace) isTrace = true;
        if (type == EnemyData.EnemyType.Random) StartCoroutine(SetRandomMove());
    }

    void FixedUpdate()
    {
        // 죽었거나, 맞을때는 넉백 효과를 위해 앞으로 못가도록 설정
        // 게임오버 시에도 못움직이도록 설정
        if (!isLive || !moveStart || (type != EnemyData.EnemyType.Boss && animator.GetCurrentAnimatorStateInfo(0).IsName("Hit")) || GameManager.instance.player.isDead) return;

        if (isTrace)   TraceMove();

        if (!isPatternPlaying) StartCoroutine(EnemyPattern());
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 비행 몬스터인데 장애물과 충돌할 때는 충돌 무시, 그 외에는 충돌 처리
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
        // 무기의 경우에는 공격을 했을 때만 공격판정이 들어가도록
        if ((collision.CompareTag("Weapon") && GameManager.instance.skill[GameManager.instance.player.currentSkillIndex].isAttack) 
            || collision.CompareTag("Bullet") || collision.CompareTag("SkillBullet"))
        {
            float knockbackAmount = collision.CompareTag("SkillBullet") ? 10f : 3f;
            Skill skill = GameManager.instance.skill[GameManager.instance.player.currentSkillIndex];

            // 기본적으로 몬스터에 들어가는 데미지는 플레이어의 공격력에 따른다. 공격력 공식은 플레이어 스크립트에 있다.
            // 무기 데미지를 추가되는 공격력으로 넣고, 스킬 데미지는 플레이어의 공격력에 x% 데미지로 들어간다.
            float finalDamage = GameManager.instance.player.power * ((skill.damage + skill.upgradeDamage[skill.level]) / 100);
            EnemyDamaged(finalDamage);

            // 투과 효과를 먹지 않았다면 넉백효과 (보스는 넉백 안먹히도록)
            // 슬로우 효과를 먹었다면 확률적으로 느리게 움직이도록
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
        //            // 멈춰!
        //            stopRandomMove = true;

        //            // 충돌한 반대방향으로 다시 돌아가
        //            moveDirection *= -1;
        //            rigid.AddForce(moveDirection);
        //            rigid.velocity = moveDirection * speed;

        //            // 다시 랜덤 시작
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
            // 몹 사망
            isLive = false;
            col.enabled = false;
            rigid.simulated = false;
            animator.SetBool("Dead", true);
            room.enemyCount--;
            GameManager.instance.player.killEnemyCount++;

            // 보스가 죽으면 보스 체력 UI 비활성화
            if (room.roomType == Room.RoomType.Boss)
            {
                GameManager.instance.ui.bossUI.SetActive(false);
            }
        }
    }

    IEnumerator KnockBack(float knockbackAmount)
    {
        // 하나의 물리 프레임 딜레이
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
        // 일단 20% 확률로 슬로우 걸리기
        int random = Random.Range(0, 10);
        if(random < 2) StartCoroutine(MoveSlow(2.5f));
    }

    public IEnumerator MoveSlow(float time)
    {
        enemySlow = true;

        // 적 머리 위에 슬로우 버프 표식 추가
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
        // 동전주머니 패밀리어가 있으면 발동
        Familiar[] familiars = GameManager.instance.player.familiar.GetComponentsInChildren<Familiar>();
        if(familiars.Length > 0)
        {
            foreach(Familiar fam in familiars)
            {
                if(fam.id == 0) fam.DropCoin();
            }
        }

        gameObject.SetActive(false);

        // 보스면 보스 리워드 10회 수행, 아니면 1번만 수행
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
                // 기본적으로 나를 따라오는 몹
                // 패턴은 따로 없다.
                break;
            case 1:
                // 기본적으로 랜덤으로 이동하는 몹
                // 패턴은 따로 없다.
                break;
            case 2:
                // 기본적으로 가만히 있는 몹
                // 패턴은 따로 없다.
                break;
            case 3:
                // 1탄 보스
                int patternRandom = Random.Range(0, 3);
                if (patternRandom == 0)
                {
                    // 1초 돌진
                    patterns.Rush();
                    yield return new WaitForSeconds(2f);
                }
                else if (patternRandom == 1)
                {
                    // 5회 흩뿌리기
                    for (int i = 0; i < 5; i++)
                    {
                        patterns.SpreadFire();
                        yield return new WaitForSeconds(0.5f);
                    }
                }
                else if (patternRandom == 2)
                {
                    // 스나이핑 3번
                    for (int i = 0; i < 3; i++)
                    {
                        patterns.Sniping(target);
                        yield return new WaitForSeconds(1f);
                    }
                }

                // 패턴 본 후 0.5초 정지
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
                // 플레이어 주변에 오면 점프공격 하기
                if (moveDirection.magnitude < 1.5)
                {
                    isTrace = false;
                    StartCoroutine(patterns.JumpAttack());
                    yield return new WaitForSeconds(3);
                    isTrace = true;
                }
                break;
            case 6:
                // 중간 보스(?)
                // 허수아비: 체력 많고 체력이 반피 이하로 내려가면 주변에 나무인형을 소환
                // 기본 패턴: 피 100% ~ 50%: 스나이핑, 50% ~ 0%: 짚 흩뿌리기

                if (health / maxHealth >= 0.5f)
                {
                    patterns.Sniping(target);
                    yield return new WaitForSeconds(3);
                }
                else
                {
                    if (!isSpawnSubEnemy)
                    {
                        // 주변에 나무인형 소환
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
