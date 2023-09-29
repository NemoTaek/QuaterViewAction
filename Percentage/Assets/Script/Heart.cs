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
        // 체력이 다 차있다면 안먹어지도록
        if (collision.CompareTag("Player") && GameManager.instance.player.health != GameManager.instance.player.currentHealth)
        {
            GameManager.instance.player.health += recovery;
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        
    }
}
