using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour
{
    public float recovery;

    void Start()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // ü���� �� ���ִٸ� �ȸԾ�������
        if (collision.CompareTag("Player") && GameManager.instance.player.health > GameManager.instance.player.currentHealth)
        {
            GameManager.instance.player.currentHealth += recovery;
            GameManager.instance.ui.isChanged = true;
            AudioManager.instance.EffectPlay(AudioManager.Effect.GetHealth);
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        
    }
}
