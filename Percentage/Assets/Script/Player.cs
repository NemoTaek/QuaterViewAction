using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("----- Component -----")]
    public Camera camera;
    public RoleData roleData;

    [Header("----- Player Component -----")]
    public Vector2 inputVec;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator animator;

    [Header("----- Player Property -----")]
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
        //inputVec.x = Input.GetAxisRaw("Horizontal");
        //inputVec.y = Input.GetAxisRaw("Vertical");
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
                break;
            case (int)RoleData.RoleType.Wizard:
                roleName = "마법사";
                roleBasicWeapon = 1;
                roleBasicSkill = 1;
                break;
            case (int)RoleData.RoleType.Thief:
                roleName = "도적";
                roleBasicWeapon = 2;
                roleBasicSkill = 2;
                break;
            case (int)RoleData.RoleType.Gunner:
                roleName = "총잡이";
                roleBasicWeapon = 3;
                roleBasicSkill = 3;
                break;
        }
    }
}
