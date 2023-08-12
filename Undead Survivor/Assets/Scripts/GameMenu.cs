using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{
    public Text totalCoinText;

    void Awake()
    {
        SetCurrentCoin();
    }

    void OnEnable()
    {
        SetCurrentCoin();
    }

    void SetCurrentCoin()
    {
        totalCoinText.text = PlayerPrefs.GetInt("Coin").ToString();
    }
}
