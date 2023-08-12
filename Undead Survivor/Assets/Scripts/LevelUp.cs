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
        items = GetComponentsInChildren<Item>(true);    // �Ű������� true�� disable�� ������Ʈ�� �� �����´�.
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
        // ��� ������ ��Ȱ��ȭ
        foreach (Item item in items)
        {
            item.gameObject.SetActive(false);
        }

        // 3���� �������� �������� Ȱ��ȭ
        int[] random = new int[3];

        // �ߺ��� �ȵǵ��� ����
        while(true)
        {
            random[0] = Random.Range(0, items.Length - 2);
            random[1] = Random.Range(0, items.Length - 2);
            random[2] = Random.Range(0, items.Length - 2);

            // ���õ� �������� ��� �ٸ���, ��� �ر��� �� ���¿��� �н�
            if (random[0] != random[1] && random[0] != random[2] && random[1] != random[2] && 
                items[random[0]].itemData.isActive && items[random[1]].itemData.isActive && items[random[2]].itemData.isActive) break;
        }

        for(int i=0; i<random.Length; i++)
        {
            Item randomItem = items[random[i]];

            // ������ ���� �������� �����̰ų� ȸ�����̶�� ȸ���� ���������� ��ü
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
