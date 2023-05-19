using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardMouse : MonoBehaviour
{
    void Update()
    {
        // Input: 게임 내 입력을 관리하는 클래스
        if(Input.anyKeyDown)    // 아무 입력을 최초로 받는다면 true
        {
            Debug.Log("플레이어가 아무 키를 눌렀습니다.");
        }
        /*
        if (Input.anyKey)    // 아무 입력을 받는다면 true
        {
            Debug.Log("플레이어가 아무 키를 누르고 있습니다.");
        }
        */
        if (Input.GetKey(KeyCode.LeftArrow))    // 해당 키를 누르고 있으면 true
        {
            Debug.Log("왼쪽으로 이동중");
        }
        if (Input.GetKeyDown(KeyCode.Return))    // 해당 키를 누른 순간에 true
        {
            Debug.Log("확인!");
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))    // 해당 키를 떼면 true
        {
            Debug.Log("오른쪽으로 이동을 멈추었습니다");
        }

        // 마우스 버전
        if (Input.GetMouseButton(0))    // 마우스를 클릭하고 있으면 true
        {
            Debug.Log("마우스 클릭중");
        }
        if (Input.GetMouseButtonDown(0))    // 마우스를 클릭한 순간에 true
        {
            Debug.Log("마우스 클릭!");
        }
        if (Input.GetMouseButtonUp(0))    // 마우스를 클릭 후 떼면 true
        {
            Debug.Log("마우스 클릭을 멈추었습니다");
        }

        // Edit - Project Settings - Input Manager에 미리 세팅한 값을 불러오기도 가능
        if (Input.GetButton("Jump"))    // 미리 설정한 키를 누르고 있으면 true
        {
            Debug.Log("슈퍼점프 기모으는중...!");
        }
        if (Input.GetButtonDown("Jump"))    // 미리 설정한 키를 누른 순간에 true
        {
            Debug.Log("점프!!");
        }
        if (Input.GetButtonUp("Jump"))    // 미리 설정한 키를 누른 후 떼면 true
        {
            Debug.Log("슈퍼점프!");
        }
    }
}
