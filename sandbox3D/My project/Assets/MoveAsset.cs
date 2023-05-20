using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAsset : MonoBehaviour
{
    Vector3 target = new Vector3(3, 0, 0);
    Vector3 zeroVec = Vector3.zero;

    void Start()
    {
        // translate: 벡터값을 현재 위치에 더하는 함수
        //Vector3 vec = new Vector3(5, 0, 0);
        //transform.Translate(vec);   // 시작하면 x축으로 5만큼 움직여라
    }

    void Update()
    {
        // Time.deltaTime: 이전 프레임의 완료까지 걸린 시간
        // deltaTime 값은 프레임이 적으면 크고, 프레임이 많으면 작다.
        // 이를 사용함으로서 실행 환경(사양)이 달라도 같은 결과 값을 받을 수 있다.
        Vector3 vec = new Vector3(Input.GetAxis("Horizontal"), 0.001f, 0) * Time.deltaTime;
        transform.Translate(vec);   // 프레임마다 x축으로 키보드 입력만큼, y축으로 0.001만큼 움직여라

        // 1. MoveTowards(현재위치, 목표위치, 속도): 목표 위치까지의 등속이동
        transform.position = Vector3.MoveTowards(transform.position, target, 0.01f * Time.deltaTime);

        // 2. SmoothDamp(현재위치, 목표위치, 참조속도, 속도): 부드러운 감속이동
        // 마지막 매개변수에 반비례하여 속도 증가
        // 참조 속도는 추가하고자 하는 방향이 있다면 넣지만, 보통은 영벡터를 넣음
        //transform.position = Vector3.SmoothDamp(transform.position, target, ref zeroVec, 1.0f);

        // 3. Lerp(현재위치, 목표위치, 속도): 선형 보간, SmoothDamp보다 감속시간이 길다.
        // 마지막 매개변수에 비례하여 속도 증가
        //transform.position = Vector3.Lerp(transform.position, target, 0.01f);

        // 4. Slerp(현재위치, 목표위치, 속도): 구면 선형 보간, 호를 그리면서 이동
        //transform.position = Vector3.Slerp(transform.position, target, 0.01f);
    }
}
