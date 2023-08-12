using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchieveManager : MonoBehaviour
{
    enum Achieve { UnlockPotato, UnlockBean, UnlockGoldenGomool };
    Achieve[] achieves;

    public GameObject[] lockCharacter;
    public GameObject[] unlockCharacter;
    public GameObject noticeUI;
    Item[] items;

    WaitForSecondsRealtime wait5;

    void Awake()
    {
        achieves = (Achieve[])Enum.GetValues(typeof(Achieve));
        wait5 = new WaitForSecondsRealtime(5);

        if (!PlayerPrefs.HasKey("MyData"))
        {
            Init();
        }
    }

    void Init()
    {
        PlayerPrefs.SetInt("MyData", 1);

        // 모든 업적 초기화
        foreach(Achieve achieve in achieves)
        {
            PlayerPrefs.SetInt(achieve.ToString(), 0);
        }
    }

    void UnlockCharacter()
    {
        for(int i=0; i<lockCharacter.Length; i++)
        {
            string achieveName = achieves[i].ToString();
            bool isUnlock = PlayerPrefs.GetInt(achieveName) == 1;
            lockCharacter[i].SetActive(!isUnlock);
            unlockCharacter[i].SetActive(isUnlock);
        }
    }

    void Start()
    {
        items = GameManager.instance.gameItems;
        UnlockCharacter();
    }

    // 기본 로직의 경우는 Update에서 처리하기 때문에
    // 처리가 끝난 후에 업적을 관리하기 위해 LateUpdate에서 구현
    void LateUpdate()
    {
        foreach(Achieve achieve in achieves)
        {
            CheckAchieve(achieve);
        }
    }

    void CheckAchieve(Achieve achieve)
    {
        bool isAchieve = false;

        switch (achieve)
        {
            case Achieve.UnlockPotato:
                isAchieve = items[4].itemData.isActive;
                break;
            case Achieve.UnlockBean:
                isAchieve = GameManager.instance.gameTime == GameManager.instance.maxGameTime;
                break;
            case Achieve.UnlockGoldenGomool:
                isAchieve = GameManager.instance.gameCharacters[4].characterData.isActive;
                break;
        }

        if(isAchieve && PlayerPrefs.GetInt(achieve.ToString()) == 0)
        {
            PlayerPrefs.SetInt(achieve.ToString(), 1);
            for(int i=0; i<noticeUI.transform.childCount; i++)
            {
                bool isActive = i == (int)achieve;
                noticeUI.transform.GetChild(i).gameObject.SetActive(isActive);
            }
            StartCoroutine(NoticeRoutine());
            UnlockCharacter();
        }
    }

    IEnumerator NoticeRoutine()
    {
        noticeUI.SetActive(true);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.LevelUp);

        yield return wait5;

        noticeUI.SetActive(false);
    }
}
