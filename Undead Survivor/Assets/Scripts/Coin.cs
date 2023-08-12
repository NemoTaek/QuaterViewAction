using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    

    void Start()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player")) {
            GameManager.instance.currentCoin++;
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        
    }
}
