using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public bool isLeft;
    public SpriteRenderer spriteRenderer;
    SpriteRenderer player;

    Quaternion leftRotation = Quaternion.Euler(0, 0, -35);  // ���Ϸ� �Լ��� ���ؼ� ���������� ���ʹϾ����� ����
    Vector3 rightPosition = new Vector3(0.35f, -0.15f, 0);
    Vector3 rightReversePosition = new Vector3(-0.35f, -0.15f, 0);

    void Awake()
    {
        player = GetComponentsInParent<SpriteRenderer>()[1];
    }

    void Update()
    {
        bool isReverse;
        if(GameManager.instance.playerId == 4) isReverse = !player.flipX;
        else isReverse = player.flipX;

        // �̵� �� �¿� ���� ����
        if (isLeft)
        {
            // ���ʹϾ�(ȸ��)�� ���ؾ� ���� ��������.
            transform.localRotation = isReverse ? leftRotation * Quaternion.Euler(0, 0, -100) : leftRotation;
            spriteRenderer.flipY = isReverse;
            spriteRenderer.sortingOrder = isReverse ? 4 : 6;
        }
        else
        {
            transform.localPosition = isReverse ? rightReversePosition : rightPosition;
            spriteRenderer.flipX = isReverse;
            spriteRenderer.sortingOrder = isReverse ? 6 : 4;
        }
    }
}
