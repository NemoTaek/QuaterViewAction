using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAsset : MonoBehaviour
{
    void Start()
    {
        // translate: 벡터값을 현재 위치에 더하는 함수
        Vector3 vec = new Vector3(5, 0, 0);
        transform.Translate(vec);   // 시작하면 x축으로 5만큼 움직여라
    }

    void Update()
    {
        Vector3 vec = new Vector3(Input.GetAxis("Horizontal"), 0.001f, 0);
        transform.Translate(vec);   // 프레임마다 x축으로 키보드 입력양만큼, y축으로 0.001만큼 움직여라
    }
}
