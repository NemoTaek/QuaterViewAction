using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("----- Component -----")]
    public Camera camera;
    public Hand[] hand;
    public UserInterface ui;

    [Header("----- Player Component -----")]
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator animator;

    [Header("----- Player Property -----")]
    public Vector2 inputVec;
    public Vector2 fireVec;
    bool isSlashing;
    public float attackDelay;
    public int currentWeaponIndex;

    public int role;
    public string roleName;
    public List<int> acquireWeapons;
    public List<int> acquireSkills;
    public int getWeaponCount;
    
    public float maxHealth = 10;
    public float health;
    public float speed;
    public float attackSpeed;
    public float power;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        hand = GetComponentsInChildren<Hand>(true);
    }

    void Start()
    {
        SetCharacterStatus();
    }

    void FixedUpdate()
    {
        PlayerMove();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Vector3 cameraPosition = Vector3.zero;
        Vector3 playerPosition = Vector3.zero;

        if (collision.CompareTag("TopDoor"))
        {
            cameraPosition = new Vector3(camera.transform.position.x, camera.transform.position.y + 12, camera.transform.position.z);
            playerPosition = new Vector3(transform.position.x, transform.position.y + 5.5f, transform.position.z);
            transform.position = playerPosition;
            StartCoroutine(camera.MoveRoom(cameraPosition));
        }
        if (collision.CompareTag("BottomDoor"))
        {
            cameraPosition = new Vector3(camera.transform.position.x, camera.transform.position.y - 12, camera.transform.position.z);
            playerPosition = new Vector3(transform.position.x, transform.position.y - 5.5f, transform.position.z);
            transform.position = playerPosition;
            StartCoroutine(camera.MoveRoom(cameraPosition));
        }
        if (collision.CompareTag("LeftDoor"))
        {
            cameraPosition = new Vector3(camera.transform.position.x - 20, camera.transform.position.y, camera.transform.position.z);
            playerPosition = new Vector3(transform.position.x - 5.5f, transform.position.y, transform.position.z);
            transform.position = playerPosition;
            StartCoroutine(camera.MoveRoom(cameraPosition));
        }
        if (collision.CompareTag("RightDoor"))
        {
            cameraPosition = new Vector3(camera.transform.position.x + 20, camera.transform.position.y, camera.transform.position.z);
            playerPosition = new Vector3(transform.position.x + 5.5f, transform.position.y, transform.position.z);
            transform.position = playerPosition;
            StartCoroutine(camera.MoveRoom(cameraPosition));
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
        if (GameManager.instance.isOpenStatus || GameManager.instance.isOpenBox) return;

        Vector2 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime;
        //rigid.AddForce(inputVec);
        //rigid.velocity = inputVec;
        rigid.MovePosition(rigid.position + nextVec);
    }

    IEnumerator PlayerAttack()
    {
        isSlashing = true;


        Vector2 dirVec = Vector2.zero;

        // 총은 무기 휘두르는 모션이 없으므로 제외하고 무기 애니메이션 실행
        if (GameManager.instance.isRightAttack)
        {
            if(role != 3)   hand[role].Attack("Right");
            dirVec = Vector2.right;
        }
        else if (GameManager.instance.isLeftAttack)
        {
            if (role != 3)  hand[role].Attack("Left");
            dirVec = Vector2.left;
        }
        else if (GameManager.instance.isUpAttack)
        {
            if (role != 3)   hand[role].Attack("Up");
            dirVec = Vector2.up;
        }
        else if (GameManager.instance.isDownAttack)
        {
            if (role != 3)  hand[role].Attack("Down");
            dirVec = Vector2.down;
        }

        // 지팡이와 총은 각각 마법과 총알을 발사
        if(role == 1 || role == 3)
        {
            GameManager.instance.weapon[role].Shot(dirVec, transform.position);
        }

        yield return new WaitForSeconds(attackDelay + 0.1f);

        isSlashing = false;
    }

    void SetCharacterStatus()
    {
        // 기본 세팅할 것
        // 직업, 스탯
        // 직업 정해지면 그에 맞는 기본 무기와 스킬 세팅
        int roleRandom = Random.Range(0, 1);
        role = roleRandom;

        switch (role)
        {
            case 0:
                roleName = "기사";
                hand[role].gameObject.SetActive(true);

                // 무기 생성
                GameObject newWeapon = GameManager.instance.GenerateWeapon();
                GameManager.instance.weapon[getWeaponCount] = newWeapon.AddComponent<Weapon>();
                GameManager.instance.weapon[getWeaponCount].Init(GameManager.instance.weaponData[0]);
                hand[role].isChanged = true;
                getWeaponCount++;
                currentWeaponIndex = 0;
                acquireWeapons.Add(0);
                acquireSkills.Add(0);

                // ui 갱신
                GameManager.instance.ui.isChanged = true;

                // 기본 스탯 설정
                health = 4;
                speed = 2;
                attackSpeed = 2;
                power = 4;
                break;
            case 1:
                roleName = "마법사";
                //GameManager.instance.weapon[role].Init("스태프", 30, 1);
                hand[role].gameObject.SetActive(true);
                acquireWeapons.Add(5);
                acquireSkills.Add(5);

                health = 3;
                speed = 3;
                attackSpeed = 2;
                power = 4;
                break;
            case 2:
                roleName = "도적";
                //GameManager.instance.weapon[role].Init("단검", 10, 1);
                hand[role].gameObject.SetActive(true);
                acquireWeapons.Add(10);
                acquireSkills.Add(10);

                health = 2;
                speed = 4;
                attackSpeed =42;
                power = 2;
                break;
            case 3:
                roleName = "총잡이";
                //GameManager.instance.weapon[role].Init("총", 5, 1);
                hand[role].gameObject.SetActive(true);
                acquireWeapons.Add(15);
                acquireSkills.Add(15);

                health = 3;
                speed = 3;
                attackSpeed = 3;
                power = 3;
                break;
        }

        ui.gameObject.SetActive(true);
    }
}
