using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    RectTransform rect;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    void Start()
    {
        
    }

    void FixedUpdate()
    {
        // Camera.main.WorldToScreenPoint(): ���� ī�޶󿡼� ���� ���� ������Ʈ ��ġ�� ��ũ�� ��ǥ�� ��ȯ
        rect.position = Camera.main.WorldToScreenPoint(GameManager.instance.player.transform.position);
    }
}
