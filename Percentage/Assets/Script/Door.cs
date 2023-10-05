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

        if(collision.CompareTag("Player"))
        {
            // ∏  ¿Ãµø
            if (doorTag == "TopDoor")
            {
                cameraPosition += Vector3.up * 12f;
                GameManager.instance.player.transform.position = playerPosition + Vector3.up * 6f;
                StartCoroutine(GameManager.instance.cam.MoveRoom(cameraPosition));

                Map.instance.currentRoom = Map.instance.currentRoom.upRoom;
                Map.instance.currentRoom.isVisited = true;
                Map.instance.mapPosition -= 1;
            }
            if (doorTag == "BottomDoor")
            {
                cameraPosition += Vector3.down * 12f;
                GameManager.instance.player.transform.position = playerPosition + Vector3.down * 6f;
                StartCoroutine(GameManager.instance.cam.MoveRoom(cameraPosition));

                Map.instance.currentRoom = Map.instance.currentRoom.downRoom;
                Map.instance.currentRoom.isVisited = true;
                Map.instance.mapPosition += 1;
            }
            if (doorTag == "LeftDoor")
            {
                cameraPosition += Vector3.left * 20f;
                GameManager.instance.player.transform.position = playerPosition + Vector3.left * 6f;
                StartCoroutine(GameManager.instance.cam.MoveRoom(cameraPosition));

                Map.instance.currentRoom = Map.instance.currentRoom.leftRoom;
                Map.instance.currentRoom.isVisited = true;
                Map.instance.mapPosition += 9;
            }
            if (doorTag == "RightDoor")
            {
                cameraPosition += Vector3.right * 20f;
                GameManager.instance.player.transform.position = playerPosition + Vector3.right * 6f;
                StartCoroutine(GameManager.instance.cam.MoveRoom(cameraPosition));

                Map.instance.currentRoom = Map.instance.currentRoom.rightRoom;
                Map.instance.currentRoom.isVisited = true;
                Map.instance.mapPosition -= 9;
            }
        }
    }
}
