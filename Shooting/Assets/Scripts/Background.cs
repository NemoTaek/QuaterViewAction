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
        // Parallax: �Ÿ��� ���� ����� �ӵ��� Ȱ���� ���
        // ����� ���ĳ��� ������ �ӵ��� �ٸ��� �ָ鼭 ���ٰ��� �־�̰� �ϴ� ���
        Vector3 currentPosition = transform.position;
        Vector3 nextPosition = Vector3.down * mapSpeed * Time.deltaTime;
        transform.position = currentPosition + nextPosition;
    }

    void ScrollingMap()
    {
        // Scrolling: ���� ����� �����ϴ� ���
        // ī�޶��� �þ߿��� ��� ���� ������ ���� ��� �������� ��ũ�� �ؼ� ���̴� ���
        if (sprites[endIndex].position.y < -viewHeight)
        {
            // �������� ������ ��� ��Ȱ��
            // 0, 1, 2�� �ݺ������� ���������� ����
            Vector3 backSpritePosition = sprites[startIndex].localPosition;
            sprites[endIndex].localPosition = backSpritePosition + Vector3.up * viewHeight;

            int startIndexSave = startIndex;
            startIndex = endIndex;
            endIndex = (startIndexSave - 1 == -1) ? sprites.Length - 1 : startIndexSave - 1;
        }
    }
}
