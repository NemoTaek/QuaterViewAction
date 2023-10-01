using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameResult : MonoBehaviour
{
    // 게임 정보창에 적을거
    // 먹은 아이템, 걸린 시간, 현재 스테이지, 직업
    public Text role;
    public Text stage;
    public Text time;
    public GameObject itemImageContainer;
    public Image itemImageCase;
    Image itemImage;

    void OnEnable()
    {
        role.text = GameManager.instance.player.roleName;
        stage.text = GameManager.instance.stage.ToString();
        time.text = GameManager.instance.elapsedTime.ToString();

        StartCoroutine(PrintGetItems());
    }

    IEnumerator PrintGetItems()
    {
        foreach(Sprite item in GameManager.instance.getItemList)
        {
            itemImage = Instantiate(itemImageCase, itemImageContainer.transform);
            itemImage.sprite = item;
            yield return new WaitForSeconds(0.2f);
        }
    }
}
