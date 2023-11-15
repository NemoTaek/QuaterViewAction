using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameResult : MonoBehaviour
{
    // 게임 정보창에 적을거
    // 먹은 아이템, 걸린 시간, 현재 스테이지, 직업
    public Text result;
    public Image resultImage;
    public Text role;
    public Text stage;
    public Text time;
    public GameObject itemImageContainer;
    public Image itemImageCase;
    Image itemImage;
    public Sprite[] bannerImage;

    void OnEnable()
    {
        if (GameManager.instance.player.isDead)
        {
            result.text = "플레이어 사망";
            resultImage.sprite = bannerImage[0];
            AudioManager.instance.EffectPlay(AudioManager.Effect.Dead);
        }
        else
        {
            result.text = "행운의 플레이어";
            resultImage.sprite = bannerImage[1];
            AudioManager.instance.EffectPlay(AudioManager.Effect.Victory);
        }

        role.text = GameManager.instance.player.roleName;
        stage.text = GameManager.instance.stage.ToString();
        time.text = TimeSpan.FromSeconds(GameManager.instance.elapsedTime).ToString("mm\\:ss\\:ff");

        StartCoroutine(PrintGetItems());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Intro");
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("Permanent Scene");
            SceneManager.LoadScene("Loading", LoadSceneMode.Additive);
        }
    }

    IEnumerator PrintGetItems()
    {
        foreach(Item item in GameManager.instance.getItemList)
        {
            itemImage = Instantiate(itemImageCase, itemImageContainer.transform);
            itemImage.sprite = item.image;
            yield return new WaitForSeconds(0.2f);
        }
    }
}
