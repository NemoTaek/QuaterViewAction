using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    // override: 부모 스크립트에서 virtual 키워드를 가진 함수를 재정의 할 수 있게 하는 키워드
    // 부모의 것을 사용하려면 base.함수명 이렇게 사용하면 된다.
    protected override void Start()
    {
        base.Start();
    }
}
