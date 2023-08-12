using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public enum InfoType { Exp, Level, Kill, Coin, Time, Health };
    public InfoType type;
    Text myText;
    Slider mySlider;

    void Awake()
    {
        myText = GetComponent<Text>();
        mySlider = GetComponent<Slider>();
    }

    void LateUpdate()
    {
        GameManager managerInstance = GameManager.instance;
        switch(type)
        {
            case InfoType.Exp:
                float currentExp = managerInstance.exp;
                float maxExp = managerInstance.nextExp[managerInstance.level];
                mySlider.value = currentExp / maxExp;
                break;
            case InfoType.Level:
                myText.text = string.Format("Lv.{0:F0}", managerInstance.level);    // {0:F0}: 0번째 인덱스의 값을 Float(소수점) 0번까지 출력
                break;
            case InfoType.Kill:
                myText.text = string.Format("{0:F0}", managerInstance.kill);
                break;
            case InfoType.Coin:
                myText.text = string.Format("{0:F0}", managerInstance.currentCoin);    // {0:F0}: 0번째 인덱스의 값을 Float(소수점) 0번까지 출력
                break;
            case InfoType.Time:
                float remainTime = managerInstance.maxGameTime - managerInstance.gameTime;
                int min = Mathf.FloorToInt(remainTime / 60);
                int sec = Mathf.FloorToInt(remainTime % 60);
                myText.text = string.Format("{0:D2}:{1:D2}", min, sec); // {0:D2}: 0번째 인덱스의 값을 Decimal(10진수) 2자리까지 출력
                break;
            case InfoType.Health:
                float currentHealth = managerInstance.health;
                float maxHealth = managerInstance.maxHealth;
                mySlider.value = currentHealth / maxHealth;
                break;
        }
    }
}
