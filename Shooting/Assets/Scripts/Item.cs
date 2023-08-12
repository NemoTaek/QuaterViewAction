using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string itemType;
    Rigidbody2D rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    // 컴포넌트가 활성화 될 때 호출되는 생명주기 함수
    void OnEnable()
    {
        rigid.velocity = Vector2.down * 1.5f;
    }
}
