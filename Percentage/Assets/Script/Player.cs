using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("----- Component -----")]
    public Camera camera;
    public RoleData roleData;
    public Hand[] hand;

    [Header("----- Player Component -----")]
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator animator;

    [Header("----- Player Property -----")]
    public Vector2 inputVec;
    public Vector2 fireVec;
    bool isSlashing;
    public bool isOpenStatus;

    public int role;
    public string roleName;
    public int roleBasicWeapon;
    public int roleBasicSkill;
    public float speed;

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
        // 키보드 입력
        InputKeyboard();

        // 상태창 오픈하면 아무것도 못하게 할 것
        if (isOpenStatus) return;

        // 플레이어 이동 애니메이션
        animator.SetFloat("speed", inputVec.magnitude);
    }

    void InputKeyboard()
    {
        bool isAttack = Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2");

        if(isAttack && !isOpenStatus && !isSlashing) {
            StartCoroutine(PlayerAttack());
        }

        if(Input.GetKeyDown(KeyCode.I))
        {
            isOpenStatus = !isOpenStatus;
            GameManager.instance.statusPanel.SetActive(isOpenStatus);
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
        if (isOpenStatus) return;

        Vector2 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime;
        //rigid.AddForce(inputVec);
        //rigid.velocity = inputVec;
        rigid.MovePosition(rigid.position + nextVec);
    }

    IEnumerator PlayerAttack()
    {
        isSlashing = true;

        bool isRightAttack = Input.GetKeyDown(KeyCode.RightArrow);
        bool isLeftAttack = Input.GetKeyDown(KeyCode.LeftArrow);
        bool isUpAttack = Input.GetKeyDown(KeyCode.UpArrow);
        bool isDownAttack = Input.GetKeyDown(KeyCode.DownArrow);
        Vector2 dirVec = Vector2.zero;

        // 총은 무기 휘두르는 모션이 없으므로 제외하고 무기 애니메이션 실행
        if (isRightAttack)
        {
            if(role != 3)   hand[role].Attack("Right");
            dirVec = Vector2.right;
        }
        else if (isLeftAttack)
        {
            if (role != 3)  hand[role].Attack("Left");
            dirVec = Vector2.left;
        }
        else if (isUpAttack)
        {
            if (role != 3)   hand[role].Attack("Up");
            dirVec = Vector2.up;
        }
        else if (isDownAttack)
        {
            if (role != 3)  hand[role].Attack("Down");
            dirVec = Vector2.down;
        }

        // 지팡이와 총은 각각 마법과 총알을 발사
        if(role == 1 || role == 3)
        {
            GameManager.instance.weapon[role].Shot(dirVec, transform.position);
        }

        yield return new WaitForSeconds(1.1f);

        isSlashing = false;
    }

    void SetCharacterStatus()
    {
        // 기본 세팅할 것
        // 직업, 스탯
        // 직업 정해지면 그에 맞는 기본 무기와 스킬 세팅
        int roleRandom = Random.Range(0, 4);
        role = roleRandom;

        switch (role)
        {
            case (int)RoleData.RoleType.Knight:
                roleName = "기사";
                roleBasicWeapon = 0;
                roleBasicSkill = 0;
                GameManager.instance.weapon[role].Init("삽", 20);
                hand[role].gameObject.SetActive(true);
                break;
            case (int)RoleData.RoleType.Wizard:
                roleName = "마법사";
                roleBasicWeapon = 1;
                roleBasicSkill = 1;
                GameManager.instance.weapon[role].Init("지팡이", 30);
                hand[role].gameObject.SetActive(true);
                break;
            case (int)RoleData.RoleType.Thief:
                roleName = "도적";
                roleBasicWeapon = 2;
                roleBasicSkill = 2;
                GameManager.instance.weapon[role].Init("단검", 10);
                hand[role].gameObject.SetActive(true);
                break;
            case (int)RoleData.RoleType.Gunner:
                roleName = "총잡이";
                roleBasicWeapon = 3;
                roleBasicSkill = 3;
                GameManager.instance.weapon[role].Init("총", 5);
                hand[role].gameObject.SetActive(true);
                break;
        }
    }
}
