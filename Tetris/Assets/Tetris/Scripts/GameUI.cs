using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public Image[] tetrominoImages;
    public GameObject levelBox;
    public GameObject stageBox;
    public Text scoreText;
    public Text lineText;
    Text levelStageText;
    public Button pauseButton;

    void OnEnable()
    {
        pauseButton.interactable = true;

        // 노말 하드모드면 레벨, 스테이지 모드면 스테이지
        if (GameManager.instance.mode == 2)
        {
            levelBox.SetActive(false);
            stageBox.SetActive(true);
            levelStageText = stageBox.GetComponentsInChildren<Text>()[1];
        }
        else
        {
            levelBox.SetActive(true);
            stageBox.SetActive(false);
            levelStageText = levelBox.GetComponentsInChildren<Text>()[1];
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        scoreText.text = GameManager.instance.score.ToString();
        lineText.text = GameManager.instance.line.ToString();

        // 노말 하드모드면 레벨, 스테이지 모드면 스테이지
        if (GameManager.instance.mode == 2)
        {
            levelStageText.text = GameManager.instance.stage.ToString();
            lineText.text = GameManager.instance.remainLines.ToString();
        }
        else levelStageText.text = GameManager.instance.level.ToString();
    }

    public void SetNextBlock(int tetIndex)
    {
        foreach (Image image in tetrominoImages)
        {
            image.gameObject.SetActive(false);
        }
        tetrominoImages[tetIndex].gameObject.SetActive(true);
    }
}
