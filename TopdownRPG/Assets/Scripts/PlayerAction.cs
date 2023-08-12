using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    public GameManager gameManager;

    float horizontal;
    float vertical;
    Rigidbody2D rigid;
    bool isHorizontalMove;
    Animator animator;
    Vector3 directionVector;
    GameObject scanObject;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator= GetComponent<Animator>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        // 수직 수평 이동 값
        // 대화중에는 움직일 수 없다.
        horizontal = gameManager.isTalkAction ? 0 : Input.GetAxisRaw("Horizontal");
        vertical = gameManager.isTalkAction ? 0 : Input.GetAxisRaw("Vertical");

        // 수직 수평 이동 키를 눌렀을 때의 상태 값 적용
        if(Mathf.Abs(horizontal) > Mathf.Abs(vertical))
        {
            // 좌우 키를 누르면 좌우 이동
            isHorizontalMove = true;
            
            if (horizontal == -1) directionVector = Vector3.left;
            else if (horizontal == 1) directionVector = Vector3.right;
        }
        else if(Mathf.Abs(horizontal) < Mathf.Abs(vertical))
        {
            // 상하 키를 누르면 상하 이동
            isHorizontalMove = false;

            if (vertical == 1) directionVector = Vector3.up;
            else if (vertical == -1) directionVector = Vector3.down;
        }
        else if(horizontal != 0 && vertical != 0)
        {   // 두 키가 모두 눌리고 있을 때
            if(Input.GetButtonDown("Horizontal"))
            {
                // 나중에 누른 키가 좌우 키면 좌우 이동
                isHorizontalMove = true;

                if (horizontal == -1) directionVector = Vector3.left;
                else if (horizontal == 1) directionVector = Vector3.right;
            }
            else if(Input.GetButtonDown("Vertical"))
            {
                // 나중에 누른 키가 상하 키면 상하 이동
                isHorizontalMove = false;

                if (vertical == 1) directionVector = Vector3.up;
                else if (vertical == -1) directionVector = Vector3.down;
            }
        }

        // 애니메이션
        if(animator.GetInteger("horizontalAxisRaw") != horizontal)
        {
            animator.SetBool("isChange", true);
            animator.SetInteger("horizontalAxisRaw", (int)horizontal);
        }
        else if (animator.GetInteger("verticalAxisRaw") != vertical)
        {
            animator.SetBool("isChange", true);
            animator.SetInteger("verticalAxisRaw", (int)vertical);
        }
        else
        {
            animator.SetBool("isChange", false);
        }

        // 스캔
        if(Input.GetButtonDown("Jump") && scanObject != null)
        {
            gameManager.Action(scanObject);
        }
    }

    void FixedUpdate()
    {
        // 십자 이동
        Vector2 moveVec = isHorizontalMove ? new Vector2(horizontal, 0) : new Vector2(0, vertical);
        rigid.velocity = moveVec * 5;

        // 조사를 위한 Ray
        Debug.DrawRay(rigid.position, directionVector * 0.5f, new Color(1, 0, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, directionVector, 0.7f, LayerMask.GetMask("Scan Object"));
        if(rayHit.collider != null)
        {
            scanObject = rayHit.collider.gameObject;
        }
        else
        {
            scanObject = null;
        }
    }
}
