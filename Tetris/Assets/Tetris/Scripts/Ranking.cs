using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ranking : MonoBehaviour
{
    public Text firstScoreText;
    public Text secondScoreText;
    public Text thirdScoreText;
    public Text firstComboText;
    public Text secondComboText;
    public Text thirdComboText;

    void OnEnable()
    {
        firstScoreText.text = PlayerPrefs.GetInt("FirstScore").ToString();
        secondScoreText.text = PlayerPrefs.GetInt("SecondScore").ToString();
        thirdScoreText.text = PlayerPrefs.GetInt("ThirdScore").ToString();
        firstComboText.text = PlayerPrefs.GetInt("FirstCombo").ToString();
        secondComboText.text = PlayerPrefs.GetInt("SecondCombo").ToString();
        thirdComboText.text = PlayerPrefs.GetInt("ThridCombo").ToString();
    }
}
