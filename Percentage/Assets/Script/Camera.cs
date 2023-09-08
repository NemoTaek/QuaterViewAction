using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public IEnumerator MoveRoom(Vector3 destination)
    {
        float delta = 0;
        float moveTimer = 1.0f;     // 방 이동할 때 카메라가 이동하는 시간

        // 벡터 이동함수를 이용한 오브젝트 이동은 4가지가 존재한다.
        // 1. movetowoards: 일정한 속도로 직선운동
        // 2. SmoothDamp: 직선운동 하다가 도착시 감속, 이 함수만 3번째 인자의 속도가 낮을수록 빨라진다.
        // 3. Lerp: 선형보간 이동
        // 4. Slerp: 구면 선형보간 이동
        while (delta <= moveTimer)
        {
            float time = delta / moveTimer;
            transform.position = Vector3.Lerp(transform.position, destination, time);
            delta += Time.deltaTime;
            yield return null;
        }
    }
}
