using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    bool isTrace;

    // override: 부모 스크립트에서 virtual 키워드를 가진 함수를 재정의 할 수 있게 하는 키워드
    // 부모의 것을 사용하려면 base.함수명 이렇게 사용하면 된다.
    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void Start()
    {
        base.Start();
    }

    void FixedUpdate()
    {
        if (!moveStart || GameManager.instance.player.isDead) return;
        if (isTrace) TraceMove();
        if (!isPatternPlaying)  StartCoroutine(BossPattern());
    }

    void Update()
    {
        
    }

    IEnumerator BossPattern()
    {
        isPatternPlaying = true;
        int patternRandom = Random.Range(0, 3);
        switch(patternRandom)
        {
            // 1초 돌진
            case 0:
                patterns.Rush();
                yield return new WaitForSeconds(2f);
                break;

            // 5회 흩뿌리기
            case 1:
                for (int i = 0; i < 5; i++)
                {
                    patterns.SpreadFire();
                    yield return new WaitForSeconds(0.5f);
                }
                break;

            // 스나이핑 3번
            case 2:
                for (int i = 0; i < 3; i++)
                {
                    patterns.Sniping(target);
                    yield return new WaitForSeconds(1f);
                }
                break;
        }

        // 패턴 본 후 0.5초 정지
        rigid.velocity = Vector3.zero;
        rigid.MovePosition(rigid.position);
        yield return new WaitForSeconds(0.5f);

        isTrace = true;
        yield return new WaitForSeconds(2f);
        isTrace = false;


        isPatternPlaying = false;
    }

    void StopPattern()
    {
        isPatternPlaying = true;
        StopCoroutine(BossPattern());
    }
}
