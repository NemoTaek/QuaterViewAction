using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    string doorTag;

    void Awake()
    {
        doorTag = gameObject.tag;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Vector3 cameraPosition = GameManager.instance.cam.transform.position;
        Vector3 playerPosition = GameManager.instance.player.transform.position;

        if(!GameManager.instance.player.isRoomMove && collision.CompareTag("Player"))
        {
            // 맵 이동
            if (doorTag == "TopDoor")
            {
                GameManager.instance.player.transform.position = playerPosition + Vector3.up * 5f;
                Map.instance.currentRoom = Map.instance.currentRoom.upRoom;
                Map.instance.mapPosition -= 1;
            }
            if (doorTag == "BottomDoor")
            {
                GameManager.instance.player.transform.position = playerPosition + Vector3.down * 5f;
                Map.instance.currentRoom = Map.instance.currentRoom.downRoom;
                Map.instance.mapPosition += 1;
            }
            if (doorTag == "LeftDoor")
            {
                GameManager.instance.player.transform.position = playerPosition + Vector3.left * 5f;
                Map.instance.currentRoom = Map.instance.currentRoom.leftRoom;
                Map.instance.mapPosition += 9;
            }
            if (doorTag == "RightDoor")
            {
                GameManager.instance.player.transform.position = playerPosition + Vector3.right * 5f;
                Map.instance.currentRoom = Map.instance.currentRoom.rightRoom;
                Map.instance.mapPosition -= 9;
            }

            // 이동할 방 인덱스를 찾고 그 위치로 카메라 이동
            RoomProperty roomProperty = Map.instance.roomProperties.Find(room => room.roomIndex == Map.instance.mapPosition);
            StartCoroutine(GameManager.instance.cam.MoveRoom(roomProperty.roomPosition));

            // 플라잉 적이 있어서 장애물과 충돌 해제를 했다면 방 이동 시 충돌 설정
            if (Physics2D.GetIgnoreLayerCollision(7, 8))
            {
                Physics2D.IgnoreLayerCollision(7, 8, false);
            }

            // 이동 완료
            GameManager.instance.player.isRoomMove = true;
            Map.instance.currentRoom.isMapDraw = false;
        }
    }

    // isRoomMove(방을 이동했는가의 bool 형 변수) 를 설정하지 않으면 정말 딱 콜라이더 크기만큼 이동했을 경우
    // 양쪽 방에서 계속 triggerEnter 판정이 나면서 왔다갔다 하는 오류가 있었다.
    // 그래서 이동 후에도 콜라이더가 문에도 겹쳤다면 판정이 일어나지 않고 한번 나와야 다시 들어갈 수 있는 로직으로 수정했다.
    void OnTriggerExit2D(Collider2D collision)
    {
        if (GameManager.instance.player.isRoomMove && collision.CompareTag("Player"))
        {
            GameManager.instance.player.isRoomMove = false;
        }
    }
}
