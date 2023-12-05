using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUseItemButton : MonoBehaviour
{
    public int id;
    Sprite sprite;
    string name;
    public int count;

    Image image;
    Text countText;

    void Awake()
    {
        image = GetComponent<Image>();
        countText = GetComponentInChildren<Text>();
    }

    void Start()
    {

    }

    public void Init(UpgradeGameItemData data)
    {
        id = data.id;
        sprite = data.sprite;
        name = data.name;
        count = data.count;

        image.sprite = sprite;
        countText.text = $"{count}";
    }

    public void SetCountText(int itemCount)
    {
        count = itemCount;
        countText.text = $"{count}";
    }

    void Update()
    {
        
    }
}
