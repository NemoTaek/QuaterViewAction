using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reposition : MonoBehaviour
{
    Collider2D col;

    void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Area"))
        {
            Vector3 playerPosition = GameManager.instance.player.transform.position;
            Vector3 currentPosition = transform.position;

            // �浹ü�� �±װ�
            switch(transform.tag)
            {
                // ���̸� ȭ�鿡�� ������ �ʴ� ���� ���� ������ ���� Ÿ�Ϸ� �̵�
                case "Ground":
                    // �÷��̾�� �� ��ġ�� �Ÿ� ����
                    float differenceX = playerPosition.x - currentPosition.x;
                    float differenceY = playerPosition.y - currentPosition.y;

                    // �� ���̰� 0���� ������ ����, ũ�� ������
                    float directionX = differenceX < 0 ? -1 : 1;
                    float directionY = differenceY < 0 ? -1 : 1;

                    differenceX = Mathf.Abs(differenceX);
                    differenceY = Mathf.Abs(differenceY);

                    if (differenceX > differenceY) {
                        transform.Translate(Vector3.right * directionX * 40);
                    }
                    else if (differenceX < differenceY)
                    {
                        transform.Translate(Vector3.up * directionY * 40);
                    }
                    else
                    {
                        transform.Translate(Vector3.right * directionX * 40 + Vector3.up * directionY * 40);
                    }
                    break;

                // ���̸� ����������� ������ ��ġ�� �̵�
                // �׾������� collider�� ��Ȱ��ȭ �ϹǷ� Ȱ��ȭ �Ǿ��ִ� �Ϳ� ���Ͽ� �̵�
                case "Enemy":
                    if (col.enabled)
                    {
                        Vector3 distance = playerPosition - currentPosition;
                        Vector3 random = new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0);
                        transform.Translate(distance * 2 + random);
                    }
                    break;
            }

        }
    }
}
