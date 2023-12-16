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
            // �� �̵�
            if (doorTag == "TopDoor")
            {
                cameraPosition += Vector3.up * 12f;
                GameManager.instance.player.transform.position = playerPosition + Vector3.up * 5f;
                StartCoroutine(GameManager.instance.cam.MoveRoom(cameraPosition));

                Map.instance.currentRoom = Map.instance.currentRoom.upRoom;
                Map.instance.mapPosition -= 1;
            }
            if (doorTag == "BottomDoor")
            {
                cameraPosition += Vector3.down * 12f;
                GameManager.instance.player.transform.position = playerPosition + Vector3.down * 5f;
                StartCoroutine(GameManager.instance.cam.MoveRoom(cameraPosition));

                Map.instance.currentRoom = Map.instance.currentRoom.downRoom;
                Map.instance.mapPosition += 1;
            }
            if (doorTag == "LeftDoor")
            {
                cameraPosition += Vector3.left * 20f;
                GameManager.instance.player.transform.position = playerPosition + Vector3.left * 5f;
                StartCoroutine(GameManager.instance.cam.MoveRoom(cameraPosition));

                Map.instance.currentRoom = Map.instance.currentRoom.leftRoom;
                Map.instance.mapPosition += 9;
            }
            if (doorTag == "RightDoor")
            {
                cameraPosition += Vector3.right * 20f;
                GameManager.instance.player.transform.position = playerPosition + Vector3.right * 5f;
                StartCoroutine(GameManager.instance.cam.MoveRoom(cameraPosition));

                Map.instance.currentRoom = Map.instance.currentRoom.rightRoom;
                Map.instance.mapPosition -= 9;
            }

            // �ö��� ���� �־ ��ֹ��� �浹 ������ �ߴٸ� �� �̵� �� �浹 ����
            if (Physics2D.GetIgnoreLayerCollision(7, 8))
            {
                Physics2D.IgnoreLayerCollision(7, 8, false);
            }

            GameManager.instance.player.isRoomMove = true;
        }
        Debug.Log("�̵� �Ϸ�");
    }

    // isRoomMove(���� �̵��ߴ°��� bool �� ����) �� �������� ������ ���� �� �ݶ��̴� ũ�⸸ŭ �̵����� ���
    // ���� �濡�� ��� triggerEnter ������ ���鼭 �Դٰ��� �ϴ� ������ �־���.
    // �׷��� �̵� �Ŀ��� �ݶ��̴��� ������ ���ƴٸ� ������ �Ͼ�� �ʰ� �ѹ� ���;� �ٽ� �� �� �ִ� �������� �����ߴ�.
    void OnTriggerExit2D(Collider2D collision)
    {
        if (GameManager.instance.player.isRoomMove && collision.CompareTag("Player"))
        {
            GameManager.instance.player.isRoomMove = false;
        }
    }
}
