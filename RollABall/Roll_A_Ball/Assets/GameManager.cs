using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int totalScore;
    public int stage;
    public Text stageItemText;
    public Text PlayerItemText;

    void Awake()
    {
        stageItemText.text = "/ " + totalScore;
    }

    public void GetItemCount(int count)
    {
        PlayerItemText.text = count.ToString();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            SceneManager.LoadScene("Stage" + stage);
        }
    }
}
