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
        if (collision.CompareTag("Player"))
        {
            GameManager.instance.gameResultPanel.pickUpScore++;
            GameManager.instance.coin++;
            AudioManager.instance.EffectPlay(AudioManager.Effect.GetCoin);
            gameObject.SetActive(false);
        }
    }

    void Update()
    {

    }
}
