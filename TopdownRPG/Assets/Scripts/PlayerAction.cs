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
        // ���� ���� �̵� ��
        // ��ȭ�߿��� ������ �� ����.
        horizontal = gameManager.isTalkAction ? 0 : Input.GetAxisRaw("Horizontal");
        vertical = gameManager.isTalkAction ? 0 : Input.GetAxisRaw("Vertical");

        // ���� ���� �̵� Ű�� ������ ���� ���� �� ����
        if(Mathf.Abs(horizontal) > Mathf.Abs(vertical))
        {
            // �¿� Ű�� ������ �¿� �̵�
            isHorizontalMove = true;
            
            if (horizontal == -1) directionVector = Vector3.left;
            else if (horizontal == 1) directionVector = Vector3.right;
        }
        else if(Mathf.Abs(horizontal) < Mathf.Abs(vertical))
        {
            // ���� Ű�� ������ ���� �̵�
            isHorizontalMove = false;

            if (vertical == 1) directionVector = Vector3.up;
            else if (vertical == -1) directionVector = Vector3.down;
        }
        else if(horizontal != 0 && vertical != 0)
        {   // �� Ű�� ��� ������ ���� ��
            if(Input.GetButtonDown("Horizontal"))
            {
                // ���߿� ���� Ű�� �¿� Ű�� �¿� �̵�
                isHorizontalMove = true;

                if (horizontal == -1) directionVector = Vector3.left;
                else if (horizontal == 1) directionVector = Vector3.right;
            }
            else if(Input.GetButtonDown("Vertical"))
            {
                // ���߿� ���� Ű�� ���� Ű�� ���� �̵�
                isHorizontalMove = false;

                if (vertical == 1) directionVector = Vector3.up;
                else if (vertical == -1) directionVector = Vector3.down;
            }
        }

        // �ִϸ��̼�
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

        // ��ĵ
        if(Input.GetButtonDown("Jump") && scanObject != null)
        {
            gameManager.Action(scanObject);
        }
    }

    void FixedUpdate()
    {
        // ���� �̵�
        Vector2 moveVec = isHorizontalMove ? new Vector2(horizontal, 0) : new Vector2(0, vertical);
        rigid.velocity = moveVec * 5;

        // ���縦 ���� Ray
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
