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
    public Collider2D col;
    Animator animator;

    [Header("----- Player Property -----")]
    public Vector2 inputVec;
    public Vector2 fireVec;
    public bool isSlashing;
    public float attackDelay;
    public int currentWeaponIndex;
    public int currentSkillIndex;
    public bool isDarkSight;

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
        Vector3 cameraPosition = Vector3.zero;
        Vector3 playerPosition = Vector3.zero;

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

    void Update()
    {
        // 상태창 오픈하면 아무것도 못하게 할 것
        if (GameManager.instance.isOpenStatus || GameManager.instance.isOpenBox) return;

        // 공격 딜레이
        if (GameManager.instance.isAttack && !isSlashing)
        {
            StartCoroutine(PlayerAttack());
        }

        // 플레이어 이동 애니메이션
        animator.SetFloat("speed", inputVec.magnitude);

        // 은신 상태면 적과의 몸빵 데미지를 무시하고 투과할 수 있음
        if(isDarkSight)
        {

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

    public IEnumerator PlayerAttack()
    {
        isSlashing = true;
        Vector2 dirVec = Vector2.zero;

        // 총은 무기 휘두르는 모션이 없으므로 제외하고 무기 애니메이션 실행
        if (GameManager.instance.isRightAttack)
        {
            dirVec = Vector2.right;
            hand[role].Attack("Right", dirVec, currentSkillIndex);
        }
        else if (GameManager.instance.isLeftAttack)
        {
            dirVec = Vector2.left;
            hand[role].Attack("Left", dirVec, currentSkillIndex);
        }
        else if (GameManager.instance.isUpAttack)
        {
            dirVec = Vector2.up;
            hand[role].Attack("Up", dirVec, currentSkillIndex);
        }
        else if (GameManager.instance.isDownAttack)
        {
            dirVec = Vector2.down;
            hand[role].Attack("Down", dirVec, currentSkillIndex);
        }

        yield return new WaitForSeconds(attackDelay + 0.1f);

        isSlashing = false;
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
        acquireWeapons.Add(0);

        // 스킬 생성 후 세팅
        GameManager.instance.skill[getSkillCount] = newSkill.AddComponent<Skill>();
        GameManager.instance.skill[getSkillCount].Init(GameManager.instance.skillData[role * 5]);
        GameManager.instance.skill[getSkillCount].name = GameManager.instance.skill[getSkillCount].skillNname;
        hand[role].isChanged = true;
        getSkillCount++;
        currentSkillIndex = 0;
        acquireSkills.Add(0);

        // ui 갱신
        GameManager.instance.ui.gameObject.SetActive(true);
        GameManager.instance.ui.isChanged = true;
    }
}
