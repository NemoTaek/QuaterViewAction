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

            // 충돌체의 태그가
            switch(transform.tag)
            {
                // 땅이면 화면에서 보이지 않는 땅을 가는 방향의 다음 타일로 이동
                case "Ground":
                    // 플레이어와 땅 위치의 거리 차이
                    float differenceX = playerPosition.x - currentPosition.x;
                    float differenceY = playerPosition.y - currentPosition.y;

                    // 그 차이가 0보다 작으면 왼쪽, 크면 오른쪽
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

                // 적이면 살아있을때만 랜덤한 위치로 이동
                // 죽어있으면 collider를 비활성화 하므로 활성화 되어있는 것에 한하여 이동
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
