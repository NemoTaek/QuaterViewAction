using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetItemPanel : MonoBehaviour
{
    public Text itemNameText;
    public Text itemDescText;
    public Image playerImage;
    public Image itemImage;

    void OnEnable()
    {

    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void SetItemPanel(string itemName, string itemDesc, Sprite image)
    {
        itemNameText.text = itemName;
        itemDescText.text = itemDesc;
        playerImage.sprite = GameManager.instance.player.spriteRenderer.sprite;
        itemImage.sprite = image;
    }
}
