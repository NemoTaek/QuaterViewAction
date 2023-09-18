using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guage : MonoBehaviour
{
    public float keydownTimer;
    public bool isKeydown;
    public bool isChargeComplete = false;

    void Start()
    {
        
    }

    void Update()
    {
        transform.localScale = Vector3.right * (keydownTimer * 0.9f) + Vector3.up * 2.64f;
    }

    public void IsKeydown(bool directionKey)
    {
        if (directionKey)
        {
            // ������ �����ð� ���� �� UI ����
            isKeydown = true;
            keydownTimer += Time.deltaTime;

            if (keydownTimer >= 3f)
            {
                isChargeComplete = true;
                keydownTimer = 3f;
            }
        }
        else
        {
            isKeydown = false;
            keydownTimer = 0;
            GameManager.instance.player.keydownGuage.transform.localScale = Vector3.zero;
        }
    }
}
