using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameResult : MonoBehaviour
{
    // ���� ����â�� ������
    // ���� ������, �ɸ� �ð�, ���� ��������, ����
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
        result.text = GameManager.instance.player.isDead ? "�÷��̾� ���" : "����� �÷��̾�";
        resultImage.sprite = GameManager.instance.player.isDead ? bannerImage[0] : bannerImage[1];
        role.text = GameManager.instance.player.roleName;
        stage.text = GameManager.instance.stage.ToString();
        time.text = TimeSpan.FromSeconds(GameManager.instance.elapsedTime).ToString("mm\\:ss\\:ff");

        StartCoroutine(PrintGetItems());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Intro1");
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
