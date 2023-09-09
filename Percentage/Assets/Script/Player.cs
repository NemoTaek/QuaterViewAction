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

        // 플레이어 이동 애니메이션
        animator.SetFloat("speed", inputVec.magnitude);
    }

    void InputKeyboard()
    {
        bool isAttack = Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2");

        if(isAttack && !isSlashing) {
            StartCoroutine(PlayerAttack());
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

    IEnumerator PlayerAttack()
    {
        isSlashing = true;

        bool isRightAttack = Input.GetKeyDown(KeyCode.RightArrow);
        bool isLeftAttack = Input.GetKeyDown(KeyCode.LeftArrow);
        bool isUpAttack = Input.GetKeyDown(KeyCode.UpArrow);
        bool isDownAttack = Input.GetKeyDown(KeyCode.DownArrow);
        int handIndex = 0;
        if (role == 0 || role == 1) handIndex = 0;
        if (role == 2 || role == 3) handIndex = 1;

        if (isRightAttack)
        {
            hand[handIndex].Attack("Right");
        }
        else if (isLeftAttack)
        {
            hand[handIndex].Attack("Left");
        }
        else if (isUpAttack)
        {
            hand[handIndex].Attack("Up");
        }
        else if (isDownAttack)
        {
            hand[handIndex].Attack("Down");
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
                GameManager.instance.weapon[0].Init("삽", 20);
                hand[0].gameObject.SetActive(true);
                break;
            case (int)RoleData.RoleType.Wizard:
                roleName = "마법사";
                roleBasicWeapon = 1;
                roleBasicSkill = 1;
                GameManager.instance.weapon[1].Init("지팡이", 30);
                hand[0].gameObject.SetActive(true);
                break;
            case (int)RoleData.RoleType.Thief:
                roleName = "도적";
                roleBasicWeapon = 2;
                roleBasicSkill = 2;
                GameManager.instance.weapon[2].Init("단검", 10);
                hand[1].gameObject.SetActive(true);
                break;
            case (int)RoleData.RoleType.Gunner:
                roleName = "총잡이";
                roleBasicWeapon = 3;
                roleBasicSkill = 3;
                GameManager.instance.weapon[2].Init("총", 5);
                hand[1].gameObject.SetActive(true);
                break;
        }
    }
}
