using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    bool isTrace;

    // override: �θ� ��ũ��Ʈ���� virtual Ű���带 ���� �Լ��� ������ �� �� �ְ� �ϴ� Ű����
    // �θ��� ���� ����Ϸ��� base.�Լ��� �̷��� ����ϸ� �ȴ�.
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
            // 1�� ����
            case 0:
                patterns.Rush();
                yield return new WaitForSeconds(2f);
                break;

            // 5ȸ ��Ѹ���
            case 1:
                for (int i = 0; i < 5; i++)
                {
                    patterns.SpreadFire();
                    yield return new WaitForSeconds(0.5f);
                }
                break;

            // �������� 3��
            case 2:
                for (int i = 0; i < 3; i++)
                {
                    patterns.Sniping(target);
                    yield return new WaitForSeconds(1f);
                }
                break;
        }

        // ���� �� �� 0.5�� ����
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
