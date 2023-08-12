using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public bool isLeft;
    public SpriteRenderer spriteRenderer;
    SpriteRenderer player;

    Quaternion leftRotation = Quaternion.Euler(0, 0, -35);  // 오일러 함수를 통해서 오릴러각을 쿼터니언으로 변경
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

        // 이동 시 좌우 방향 설정
        if (isLeft)
        {
            // 쿼터니언(회전)은 곱해야 값이 더해진다.
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
