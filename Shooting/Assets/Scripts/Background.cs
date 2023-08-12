using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    public int mapSpeed;
    public int startIndex;  // 2
    public int endIndex;    // 0
    public Transform[] sprites;
    float viewHeight;

    void Awake()
    {
        viewHeight = Camera.main.orthographicSize * 2;
    }

    void Start()
    {
        
    }

    void Update()
    {
        MovingMap();
        ScrollingMap();
    }

    void MovingMap()
    {
        // Parallax: 거리에 따른 상대적 속도를 활용한 기술
        // 배경을 겹쳐놓고 각각의 속도를 다르게 주면서 원근감을 있어보이게 하는 방식
        Vector3 currentPosition = transform.position;
        Vector3 nextPosition = Vector3.down * mapSpeed * Time.deltaTime;
        transform.position = currentPosition + nextPosition;
    }

    void ScrollingMap()
    {
        // Scrolling: 무한 배경을 구현하는 방법
        // 카메라의 시야에서 벗어난 맵을 앞으로 나올 배경 다음으로 스크롤 해서 붙이는 방식
        if (sprites[endIndex].position.y < -viewHeight)
        {
            // 뒤쪽으로 내려간 배경 재활용
            // 0, 1, 2를 반복적으로 돌려가도록 세팅
            Vector3 backSpritePosition = sprites[startIndex].localPosition;
            sprites[endIndex].localPosition = backSpritePosition + Vector3.up * viewHeight;

            int startIndexSave = startIndex;
            startIndex = endIndex;
            endIndex = (startIndexSave - 1 == -1) ? sprites.Length - 1 : startIndexSave - 1;
        }
    }
}
