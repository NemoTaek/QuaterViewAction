using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUp : MonoBehaviour
{
    RectTransform rect;
    public Item[] items;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        items = GetComponentsInChildren<Item>(true);    // 매개변수에 true면 disable인 컴포넌트도 다 가져온다.
    }

    public void Show()
    {
        NextItemList();
        rect.localScale = Vector3.one;
        GameManager.instance.Pause();
        AudioManager.instance.PlaySfx(AudioManager.Sfx.LevelUp);
        AudioManager.instance.HighPassBgm(true);
    }

    public void Hide()
    {
        rect.localScale = Vector3.zero;
        GameManager.instance.Resume();
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
        AudioManager.instance.HighPassBgm(false);
    }

    public void Select(int index)
    {
        items[index].OnClick();
    }

    void NextItemList()
    {
        // 모든 아이템 비활성화
        foreach (Item item in items)
        {
            item.gameObject.SetActive(false);
        }

        // 3개의 아이템을 랜덤으로 활성화
        int[] random = new int[3];

        // 중복이 안되도록 리세
        while(true)
        {
            random[0] = Random.Range(0, items.Length - 2);
            random[1] = Random.Range(0, items.Length - 2);
            random[2] = Random.Range(0, items.Length - 2);

            // 선택된 아이템이 모두 다르고, 모두 해금이 된 상태여야 패스
            if (random[0] != random[1] && random[0] != random[2] && random[1] != random[2] && 
                items[random[0]].itemData.isActive && items[random[1]].itemData.isActive && items[random[2]].itemData.isActive) break;
        }

        for(int i=0; i<random.Length; i++)
        {
            Item randomItem = items[random[i]];

            // 하지만 뽑은 아이템이 만렙이거나 회복약이라면 회복약 아이템으로 대체
            if (randomItem.level == randomItem.itemData.damages.Length || random[i] >= items.Length - 3)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (!items[items.Length - 3 + j].gameObject.activeSelf)
                    {
                        items[items.Length - 3 + j].gameObject.SetActive(true);
                        break;
                    }
                }
            }
            else
            {
                randomItem.gameObject.SetActive(true);
            }
        }
    }
}
