using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("------- Component -------")]
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator animator;
    public Scanner scanner;
    public Hand[] hands;
    public RuntimeAnimatorController[] animatorController;

    [Header("------- Input -------")]
    public Vector2 inputVector;

    [Header("------- logic -------")]
    public float speed;


    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        scanner = GetComponent<Scanner>();
        hands = GetComponentsInChildren<Hand>(true);    // 초기화 시 매개변수 자리에 true, false를 넣으면 오브젝트를 활성화, 비활성화 시켜준다
    }

    void OnEnable()
    {
        speed *= GameManager.instance.gameCharacters[GameManager.instance.playerId].characterData.baseSpeed;
        animator.runtimeAnimatorController = animatorController[GameManager.instance.playerId];
    }

    // 물리 연산을 하는 update 함수
    // 간편하게 fixed가 붙으면 물리연산에 관련된 함수라 생각하자
    // 물리 사이클은 update time보다 fixed time이 작으면 프레임당 여러번 실행될 수 있다.
    // 즉 fixedDeltaTime이 deltaTime보다 작으면 physics loop가 여러번 반복 실행될 수 있다.
    // 생명주기에서 fixedUpdate는 물리 사이클의 초입에 실행되며,
    // 생명주기처럼 일정한 주기마다 호출되는 것이 아니라 game loop 내에서 physics loop를 통해 호출된다.
    // [참고] https://rito15.github.io/posts/unity-fixed-update-and-physics-loop/
    void FixedUpdate()
    {
        if (!GameManager.instance.isTimePassing) return;

        // 위치 이동
        // deltaTime: 1프레임 당 흐르는 시간
        // fixedDeltaTime: 물리 프레임 당 흐르는 시간
        Vector2 nextVector = inputVector.normalized * speed * Time.fixedDeltaTime;  
        rigid.MovePosition(rigid.position + nextVector);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if(!GameManager.instance.isTimePassing) return;

        GameManager.instance.health -= Time.deltaTime * 10;  // 원래는 곱하기 10

        if(GameManager.instance.health <= 0)
        {
            // transform.childCount: 자식의 개수
            for(int i=2; i<transform.childCount; i++)
            {
                // transform.GetChild(n): n번째 자식 가져오기
                transform.GetChild(i).gameObject.SetActive(false);
            }
            animator.SetTrigger("dead");
            GameManager.instance.GameOver();
        }
    }

    void Update()
    {
        if (!GameManager.instance.isTimePassing) return;
        /*
        inputVector.x = Input.GetAxisRaw("Horizontal");
        inputVector.y = Input.GetAxisRaw("Vertical");
        */

        // 이동 시 좌우 방향 설정
        if (GameManager.instance.playerId == 4)
        {
            if (inputVector.x < 0)
            {
                spriteRenderer.flipX = false;
            }
            else if (inputVector.x > 0)
            {
                spriteRenderer.flipX = true;
            }
        }
        else
        {
            if (inputVector.x < 0)
            {
                spriteRenderer.flipX = true;
            }
            else if (inputVector.x > 0)
            {
                spriteRenderer.flipX = false;
            }
        }

        // 이동 시 애니메이션
        animator.SetFloat("speed", inputVector.magnitude);
    }

    // Input System을 사용하여 플레이어 이동 로직 생성
    void OnMove(InputValue value)
    {
        inputVector = value.Get<Vector2>();
    }
}
