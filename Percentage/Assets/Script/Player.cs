using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [Header("----- Component -----")]
    public Hand[] hand;
    public UserInterface ui;

    [Header("----- Player Component -----")]
    public Rigidbody2D rigid;
    public SpriteRenderer spriteRenderer;
    public SpriteRenderer buffSprite;
    public Collider2D col;
    Animator animator;
    public Transform keydownGuage;
    public Transform guage;
    public GameObject familiar;

    [Header("----- Player Property -----")]
    public Vector2 inputVec;
    public Vector2 fireVec;
    public bool isSlashing;
    public int currentWeaponIndex;
    public int currentSkillIndex;
    public bool isOnObject;
    public bool isDamaged;
    public bool isDead;
    public float keydownTimer;
    public bool isKeydown;
    public bool isChargeComplete = false;
    public bool isGameClear;
    public int killEnemyCount;

    public int role;
    public string roleName;
    public int getWeaponCount;
    public int getSkillCount;
    
    public float maxHealth = 10;
    public float currentHealth;
    public float health;
    public float speed;
    public float attackSpeed;
    public float attackSpeedUp = 0;
    public float basePower;
    public float power;
    public float powerUp = 0;
    public float staticPower = 0;

    public bool isFly;
    public bool isScapular;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        hand = GetComponentsInChildren<Hand>(true);
    }

    void Start()
    {
        SetCharacterStatus();
    }

    void FixedUpdate()
    {
        // 플레이어 못움직이게하는 이유 참 많다 그죠?
        // 로딩중, 스탯창, 상자 오픈, 아이템 습득, 사망, 게임 클리어
        if (GameManager.instance.isLoading || GameManager.instance.isOpenStatus || GameManager.instance.isOpenBox || GameManager.instance.isOpenItemPanel || isDead || isGameClear) return;
        PlayerMove();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // 적의 총알에 맞으면 체력 감소
        if (collision.CompareTag("EnemyBullet") && !isDamaged)
        {
            // 적의 총알 삭제
            collision.gameObject.SetActive(false);

            PlayerCollide();
        }

        // 피격시 무적 1초와 비행효과 있을 경우를 제외하면 피격
        if (collision.CompareTag("Spike") && !isDamaged && !isFly)
        {
            PlayerCollide();
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (isOnObject && collision.CompareTag("Object"))
        {
            // collision 태그를 검사 안하고 하면 collision이 발사체 혹은 무기가 되는데 공격하면 이들이 플레이어를 떠나서 가게되므로 트리거 엑싯 판정이 났던것이었다.
            // 그래서 피충돌체를 장애물(Object)로 한정시켜 장애물에서 나갈 때만 트리거를 원상복귀 시키는 것으로 했다.
            col.isTrigger = false;
            isOnObject = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 적과 닿으면 체력 감소
        if (collision.collider.CompareTag("Enemy") && !isDamaged)
        {
            PlayerCollide();
        }

        // 비행 능력 획득 시 오브젝트와 충돌 시, 플레이어의 트리거가 해제되어 그 위를 지나다닐 수 있음
        if (isFly && collision.collider.CompareTag("Object"))
        {
            isOnObject = true;
            col.isTrigger = true;
        }
    }

    void Update()
    {
        // 플레이어 스탯 실시간 계산
        CalculateStatus();

        // 플레이어 공격
        PlayerAttack();

        // 플레이어 이동 애니메이션
        animator.SetFloat("speed", inputVec.magnitude);
    }

    void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>();
        if(inputVec.x != 0)
        {
            spriteRenderer.flipX = inputVec.x > 0 ? false : true;
        }
    }

    void PlayerMove()
    {
        Vector2 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime;
        //rigid.AddForce(inputVec);
        //rigid.velocity = inputVec;
        rigid.MovePosition(rigid.position + nextVec);
    }

    void PlayerCollide()
    {
        // 플레이어의 최대 체력 or 현재 체력이 0 이하가 되면 사망
        if (currentHealth <= 0)
        {
            isDead = true;
            animator.SetTrigger("dead");
            StartCoroutine(GameManager.instance.GameResult());
        }
        else StartCoroutine(PlayerDamaged());
    }

    IEnumerator PlayerDamaged()
    {
        // 성의 아이템을 먹으면 30% 확률로 노피격 판정
        if (isScapular)
        {
            int random = Random.Range(0, 10);
            if (random > 3) currentHealth -= 0.5f;
        }
        else currentHealth -= 0.5f;

        // 피격 효과
        isDamaged = true;
        GameManager.instance.ui.isChanged = true;

        // 투명도 50% 후 1초 후 복구
        Color color = spriteRenderer.color;
        color.a = 0.5f;
        spriteRenderer.color = color;

        yield return new WaitForSeconds(1f);

        isDamaged = false;

        color.a = 1f;
        spriteRenderer.color = color;
    }

    public void PlayerAttack()
    {
        Vector2 dirVec = Vector2.zero;

        // 공격
        if (GameManager.instance.isRightAttack)
        {
            dirVec = Vector2.right;
            SetAttackDirection(GameManager.instance.isRightAttack, dirVec);
        }
        else if (GameManager.instance.isLeftAttack)
        {
            dirVec = Vector2.left;
            SetAttackDirection(GameManager.instance.isLeftAttack, dirVec);
        }
        else if (GameManager.instance.isUpAttack)
        {
            dirVec = Vector2.up;
            SetAttackDirection(GameManager.instance.isUpAttack, dirVec);
        }
        else if (GameManager.instance.isDownAttack)
        {
            dirVec = Vector2.down;
            SetAttackDirection(GameManager.instance.isDownAttack, dirVec);
        }

        // 궁극기 키다운에서 키업
        // 게이지가 완충되었을 때, 키를 떼면 스킬 발동
        if (GameManager.instance.isUltimateRightAttack)
        {
            if (keydownTimer >= 3f)
            {
                hand[role].Attack(Vector2.right, currentSkillIndex);
            }
            ChargingCancel();
        }
        else if (GameManager.instance.isUltimateLeftAttack)
        {
            if (keydownTimer >= 3f)
            {
                hand[role].Attack(Vector2.left, currentSkillIndex);
            }
            ChargingCancel();
        }
        else if (GameManager.instance.isUltimateUpAttack)
        {
            if (keydownTimer >= 3f)
            {
                hand[role].Attack(Vector2.up, currentSkillIndex);
            }
            ChargingCancel();
        }
        else if (GameManager.instance.isUltimateDownAttack)
        {
            if (keydownTimer >= 3f)
            {
                hand[role].Attack(Vector2.down, currentSkillIndex);
            }
            ChargingCancel();
        }
    }

    void SetAttackDirection(bool directionKey, Vector2 dirVec)
    {
        if (GameManager.instance.skill[currentSkillIndex].id == role * 5 + 4 && GameManager.instance.skill[currentSkillIndex].coolTimeTimer <= 0)
        {
            ChargingWeapon(directionKey);
        }
        else
        {
            hand[role].Attack(dirVec, currentSkillIndex);
        }
    }

    void ChargingWeapon(bool directionKey)
    {
        // 스킬을 활성화 하고 키다운 하면 게이지가 활성화
        keydownGuage.localScale = new Vector3(1.5f, 0.5f, 1);

        if (directionKey)
        {
            // 게이지 충전시간 증가 및 UI 갱신
            isKeydown = true;
            keydownTimer += Time.deltaTime;
            guage.localScale = Vector3.right * (keydownTimer * 0.9f) + Vector3.up * 2.64f;

            if (keydownTimer >= 3f)
            {
                keydownTimer = 3f;
            }
        }
    }

    void ChargingCancel()
    {
        isKeydown = false;
        keydownTimer = 0;
        keydownGuage.localScale = Vector3.zero;
    }

    void SetCharacterStatus()
    {
        // 기본 세팅할 것
        // 직업, 스탯
        // 직업 정해지면 그에 맞는 기본 무기와 스킬 세팅
        int roleRandom = Random.Range(0, 4);
        role = roleRandom;
        GameObject newWeapon = GameManager.instance.GenerateWeapon();
        GameObject newSkill = GameManager.instance.GenerateSkill();

        switch (role)
        {
            case 0:
                roleName = "기사";
                hand[role].gameObject.SetActive(true);

                // 기본 스탯 설정
                health = 4;
                speed = 2.5f;
                attackSpeed = 0;
                basePower = 3.5f;
                break;
            case 1:
                roleName = "마법사";
                hand[role].gameObject.SetActive(true);

                // 기본 스탯 설정
                health = 3;
                speed = 3;
                attackSpeed = 0;
                basePower = 3f;
                break;
            case 2:
                roleName = "도적";
                hand[role].gameObject.SetActive(true);

                // 기본 스탯 설정
                health = 2;
                speed = 3.5f;
                attackSpeed = 0.5f;
                basePower = 2.5f;
                break;
            case 3:
                roleName = "거너";
                hand[role].gameObject.SetActive(true);

                // 기본 스탯 설정
                health = 3;
                speed = 3;
                attackSpeed = 0;
                basePower = 3f;
                break;
        }
        currentHealth = health;

        // 무기 생성 후 세팅
        // 무기의 데미지는 아이템으로 먹은 추가 데미지와 같게 들어간다. 무기 교체할 때도 동일하게 적용한다.
        GameManager.instance.weapon[getWeaponCount] = newWeapon.AddComponent<Weapon>();
        GameManager.instance.weapon[getWeaponCount].Init(GameManager.instance.weaponData[role * 5]);
        GameManager.instance.weapon[getWeaponCount].name = GameManager.instance.weapon[getWeaponCount].weaponName;
        hand[role].isChanged = true;
        getWeaponCount++;
        currentWeaponIndex = 0;
        powerUp += (GameManager.instance.weapon[currentWeaponIndex].damage + GameManager.instance.weapon[currentWeaponIndex].upgradeDamage[GameManager.instance.weapon[currentWeaponIndex].level]);

        // 스킬 생성 후 세팅
        GameManager.instance.skill[getSkillCount] = newSkill.AddComponent<Skill>();
        GameManager.instance.skill[getSkillCount].Init(GameManager.instance.skillData[role * 5]);
        GameManager.instance.skill[getSkillCount].name = GameManager.instance.skill[getSkillCount].skillName;
        hand[role].isChanged = true;
        getSkillCount++;
        currentSkillIndex = 0;

        // ui 갱신
        GameManager.instance.ui.gameObject.SetActive(true);
        GameManager.instance.ui.isChanged = true;
    }

    public void CalculateStatus(int a=0)
    {
        // 스탯에 관련한 공식은 아이작을 따른다.
        // 공격력: 플레이어 기본 공격력 * sqrt(게임 중 획득한 공격력 수치 * 1.2 + 1) + 고정으로 올려주는 데미지
        power = basePower * Mathf.Sqrt((powerUp * 1.2f) + 1) + staticPower;

        // 공격속도: 16 - 6 * sqrt(게임 중 획득한 공격속도 수치 * 1.3 + 1). 만약 루트 값이 음수가 나오면 16 - 6 * 게임 중 획득한 공격속도 수치 로 설정
        // ex) 공속이 3인 캐릭터는 16 - 6 * (sqrt(4.9)) = 16 - 6 * 2.2 = 2.8
        // 플레이어는 초당 30 / 공속+1 번 공격한다. -> 초당 30 / 3.8번 -> 1번 공격하는데 3.8 / 30 초
        float finalAttackSpeed = 0;
        if (attackSpeedUp >= -0.75f) finalAttackSpeed = 16 - 6 * Mathf.Sqrt(attackSpeedUp * 1.3f + 1);
        else finalAttackSpeed = 16 - 6 * attackSpeedUp;
        attackSpeed = (finalAttackSpeed + 1) / 30;

        // 게임에서의 최대 체력은 10
        // 플레이어의 최대 체력은 처음 세팅된 체력. 추후에 아이템으로 변동될 수 있음
        
        // 플레이어의 최대 체력(health)은 게임의 최대 체력(10)을 넘을 수 없다.
        if (maxHealth < health) health = maxHealth;

        // 현재 체력(currentHealth)은 플레이어의 최대 체력(health)을 넘을 수 없다.
        if (health < currentHealth) currentHealth = health;
    }
}
