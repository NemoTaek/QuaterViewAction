using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeCycle : MonoBehaviour
{
    // 라이프사이클은 크게 [초기화 - 프레임 - 해제] 단계로 구성된다.
    
    // 초기화 단계: Awake - Start
    // 게임 오브젝트 생성할 때, 최초 실행
    void Awake()
    {
        Debug.Log("플레이어 데이터가 준비되었습니다.");
    }

    // 업데이트 시작 직전, 최초 실행
    void Start()
    {
        Debug.Log("사냥 장비를 챙겼습니다.");
    }

    // 오브젝트 활성화 단계: OnEnable
    void OnEnable()
    {
        Debug.Log("로그인하였습니다.");
    }

    // 물리연산 단계: FixedUpdate
    // 물리연산 업데이트. 1초에 여러번 실행된다. 그래서 CPU를 많이 사용하게 된다.
    void FixedUpdate()
    {
        Debug.Log("이동!");
    }

    // 게임로직 단계: Update - LateUpdate
    // 게임로직 업데이트. 실행 환경에 따라서 실행 주기가 달라질 수 있다.
    void Update()
    {
        Debug.Log("몬스터 사냥!");
    }

    // 업데이트 함수가 끝난 후 실행. 업데이트 실행 횟수와 같은 횟수로 실행됨
    void LateUpdate()
    {
        Debug.Log("경험치 10 획득!");
    }

    // 오브젝트 비활성화 단계: OnDisable
    void OnDisable()
    {
        Debug.Log("로그아웃하였습니다.");
    }

    // 해체 단계: OnDestroy
    // 게임 오브젝트가 삭제될 때 실행
    void OnDestroy()
    {
        Debug.Log("플레이어 데이터를 해제하였습니다.");
    }
}
