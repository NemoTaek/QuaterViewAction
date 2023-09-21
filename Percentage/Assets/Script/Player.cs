using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("----- Component -----")]
    public Camera cam;
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

    [Header("----- Player Property -----")]
    public Vector2 inputVec;
    public Vector2 fireVec;
    public bool isSlashing;
    public float attackDelay;
    public int currentWeaponIndex;
    public int currentSkillIndex;
    public bool isDarkSight;
    public bool isDamaged;
    public float keydownTimer;
    public bool isKeydown;
    public bool isChargeComplete = false;

    public int role;
    public string roleName;
    public List<int> acquireWeapons;
    public List<int> acquireSkills;
    public int getWeaponCount;
    public int getSkillCount;
    
    public float maxHealth = 10;
    public float health;
    public float speed;
    public float attackSpeed;
    public float power;

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
        if (GameManager.instance.isOpenStatus || GameManager.instance.isOpenBox) return;
        PlayerMove();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Vector3 cameraPosition = cam.transform.position;
        Vector3 playerPosition = transform.position;

        // 맵 이동
        if (collision.CompareTag("TopDoor"))
        {
            cameraPosition = new Vector3(cam.transform.position.x, cam.transform.position.y + 12, cam.transform.position.z);
            playerPosition = new Vector3(transform.position.x, transform.position.y + 5.5f, transform.position.z);
            transform.position = playerPosition;
            StartCoroutine(cam.MoveRoom(cameraPosition));
        }
        if (collision.CompareTag("BottomDoor"))
        {
            cameraPosition = new Vector3(cam.transform.position.x, cam.transform.position.y - 12, cam.transform.position.z);
            playerPosition = new Vector3(transform.position.x, transform.position.y - 5.5f, transform.position.z);
            transform.position = playerPosition;
            StartCoroutine(cam.MoveRoom(cameraPosition));
        }
        if (collision.CompareTag("LeftDoor"))
        {
            cameraPosition = new Vector3(cam.transform.position.x - 20, cam.transform.position.y, cam.transform.position.z);
            playerPosition = new Vector3(transform.position.x - 5.5f, transform.position.y, transform.position.z);
            transform.position = playerPosition;
            StartCoroutine(cam.MoveRoom(cameraPosition));
        }
        if (collision.CompareTag("RightDoor"))
        {
            cameraPosition = new Vector3(cam.transform.position.x + 20, cam.transform.position.y, cam.transform.position.z);
            playerPosition = new Vector3(transform.position.x + 5.5f, transform.position.y, transform.position.z);
            transform.position = playerPosition;
            StartCoroutine(cam.MoveRoom(cameraPosition));
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 적과 닿으면 체력 감소
        if (collision.collider.CompareTag("Enemy"))
        {
            isDamaged = true;
            health -= 0.5f;
        }
    }

    void Update()
    {
        // 상태창 오픈하면 아무것도 못하게 할 것
        if (GameManager.instance.isOpenStatus || GameManager.instance.isOpenBox) return;

        // 플레이어 공격
        PlayerAttack();

        // 플레이어 이동 애니메이션
        animator.SetFloat("speed", inputVec.magnitude);

        // 피격 시 1초 무적
        if(isDamaged)
        {
            StartCoroutine(PlayerDamaged());
        }
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

    IEnumerator PlayerDamaged()
    {
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
                speed = 2;
                attackSpeed = 2;
                power = 4;
                break;
            case 1:
                roleName = "마법사";
                hand[role].gameObject.SetActive(true);

                // 기본 스탯 설정
                health = 3;
                speed = 3;
                attackSpeed = 2;
                power = 4;
                break;
            case 2:
                roleName = "도적";
                hand[role].gameObject.SetActive(true);

                // 기본 스탯 설정
                health = 2;
                speed = 4;
                attackSpeed = 4;
                power = 2;
                break;
            case 3:
                roleName = "거너";
                hand[role].gameObject.SetActive(true);

                // 기본 스탯 설정
                health = 3;
                speed = 3;
                attackSpeed = 3;
                power = 3;
                break;
        }

        // 무기 생성 후 세팅
        GameManager.instance.weapon[getWeaponCount] = newWeapon.AddComponent<Weapon>();
        GameManager.instance.weapon[getWeaponCount].Init(GameManager.instance.weaponData[role * 5]);
        GameManager.instance.weapon[getWeaponCount].name = GameManager.instance.weapon[getWeaponCount].weaponNname;
        hand[role].isChanged = true;
        getWeaponCount++;
        currentWeaponIndex = 0;
        acquireWeapons.Add(role * 5);

        // 스킬 생성 후 세팅
        GameManager.instance.skill[getSkillCount] = newSkill.AddComponent<Skill>();
        GameManager.instance.skill[getSkillCount].Init(GameManager.instance.skillData[role * 5]);
        GameManager.instance.skill[getSkillCount].name = GameManager.instance.skill[getSkillCount].skillNname;
        hand[role].isChanged = true;
        getSkillCount++;
        currentSkillIndex = 0;
        acquireSkills.Add(role * 5);

        // ui 갱신
        GameManager.instance.ui.gameObject.SetActive(true);
        GameManager.instance.ui.isChanged = true;
    }
}
