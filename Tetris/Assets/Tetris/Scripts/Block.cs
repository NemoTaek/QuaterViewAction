using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public int blockId;
    public Color32 blockColor;
    public bool isCollide;
    public bool isLanded;

    void Awake()
    {
        blockId = -1;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // 블록 리스폰 될 때 초기화 처리
    void OnEnable()
    {
        isCollide = false;
        isLanded = false;
    }

    void Start()
    {
        
    }

    //void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.collider.gameObject.CompareTag("LandingTetromino") || collision.collider.gameObject.CompareTag("Border"))
    //    {
    //        isCollide = true;
    //    }
    //}

    void Update()
    {
        SetBlock();
    }

    void SetBlock()
    {
        // 아이디에 따라 색상 부여
        switch(blockId)
        {
            case 0:
                blockColor = new Color32(115, 251, 253, 255);
                break;
            case 1:
                blockColor = new Color32(0, 33, 245, 255);
                break;
            case 2:
                blockColor = new Color32(243, 168, 59, 255);
                break;
            case 3:
                blockColor = new Color32(255, 253, 84, 255);
                break;
            case 4:
                blockColor = new Color32(117, 250, 76, 255);
                break;
            case 5:
                blockColor = new Color32(155, 47, 246, 255);
                break;
            case 6:
                blockColor = new Color32(235, 51, 35, 255);
                break;
            case 99:
                blockColor = new Color32(161, 165, 185, 255);
                break;
            default:
                blockColor = new Color32(128, 128, 128, 64);
                break;
        }
        spriteRenderer.color = blockColor;
        if(blockId != -1) gameObject.tag = "Tetromino";

        // 착지된 블록이면 태그 변경
        if (isLanded)
        {
            gameObject.layer = 6;
            gameObject.tag = "LandingTetromino";
        }
        else
        {
            gameObject.layer = 0;
            gameObject.tag = "Untagged";
        }
    }
}
