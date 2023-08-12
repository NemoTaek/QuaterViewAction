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
        hands = GetComponentsInChildren<Hand>(true);    // �ʱ�ȭ �� �Ű����� �ڸ��� true, false�� ������ ������Ʈ�� Ȱ��ȭ, ��Ȱ��ȭ �����ش�
    }

    void OnEnable()
    {
        speed *= GameManager.instance.gameCharacters[GameManager.instance.playerId].characterData.baseSpeed;
        animator.runtimeAnimatorController = animatorController[GameManager.instance.playerId];
    }

    // ���� ������ �ϴ� update �Լ�
    // �����ϰ� fixed�� ������ �������꿡 ���õ� �Լ��� ��������
    // ���� ����Ŭ�� update time���� fixed time�� ������ �����Ӵ� ������ ����� �� �ִ�.
    // �� fixedDeltaTime�� deltaTime���� ������ physics loop�� ������ �ݺ� ����� �� �ִ�.
    // �����ֱ⿡�� fixedUpdate�� ���� ����Ŭ�� ���Կ� ����Ǹ�,
    // �����ֱ�ó�� ������ �ֱ⸶�� ȣ��Ǵ� ���� �ƴ϶� game loop ������ physics loop�� ���� ȣ��ȴ�.
    // [����] https://rito15.github.io/posts/unity-fixed-update-and-physics-loop/
    void FixedUpdate()
    {
        if (!GameManager.instance.isTimePassing) return;

        // ��ġ �̵�
        // deltaTime: 1������ �� �帣�� �ð�
        // fixedDeltaTime: ���� ������ �� �帣�� �ð�
        Vector2 nextVector = inputVector.normalized * speed * Time.fixedDeltaTime;  
        rigid.MovePosition(rigid.position + nextVector);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if(!GameManager.instance.isTimePassing) return;

        GameManager.instance.health -= Time.deltaTime * 10;  // ������ ���ϱ� 10

        if(GameManager.instance.health <= 0)
        {
            // transform.childCount: �ڽ��� ����
            for(int i=2; i<transform.childCount; i++)
            {
                // transform.GetChild(n): n��° �ڽ� ��������
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

        // �̵� �� �¿� ���� ����
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

        // �̵� �� �ִϸ��̼�
        animator.SetFloat("speed", inputVector.magnitude);
    }

    // Input System�� ����Ͽ� �÷��̾� �̵� ���� ����
    void OnMove(InputValue value)
    {
        inputVector = value.Get<Vector2>();
    }
}
