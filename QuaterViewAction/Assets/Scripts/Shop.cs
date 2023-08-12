using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public GameObject shopPanel;
    public RectTransform ui;
    public Animator animator;
    Player enterPlayer;

    public int[] itemValue;
    public int[] itemPrice;
    public Text talkText;
    public string[] talkData;

    public void Enter(Player player)
    {
        enterPlayer = player;
        shopPanel.SetActive(true);
        ui.anchoredPosition = Vector3.zero;
    }

    public void Exit()
    {
        animator.SetTrigger("doHello");
        shopPanel.SetActive(false);
        ui.anchoredPosition = Vector3.down * 1000;
    }

    public void Buy(int itemIndex)
    {
        int price = itemPrice[itemIndex];
        if(price > enterPlayer.coin)
        {
            StopCoroutine(Talk());
            StartCoroutine(Talk());
            return;
        }
        else
        {
            enterPlayer.coin -= price;

            if(itemIndex < 3)
            {
                enterPlayer.hasWeapon[itemIndex] = true;
            }
            else if(itemIndex == 3)
            {
                if (enterPlayer.ammo < enterPlayer.maxAmmo)
                {
                    enterPlayer.ammo += itemValue[itemIndex];
                    if (enterPlayer.ammo > enterPlayer.maxAmmo)
                    {
                        enterPlayer.ammo = enterPlayer.maxAmmo;
                    }
                }
                else
                {
                    enterPlayer.ammo = enterPlayer.maxAmmo;
                }
            }
            else if (itemIndex == 4)
            {
                if (enterPlayer.health < enterPlayer.maxHealth)
                {
                    enterPlayer.health += itemValue[itemIndex];
                    if (enterPlayer.health > enterPlayer.maxHealth)
                    {
                        enterPlayer.health = enterPlayer.maxHealth;
                    }
                }
                else
                {
                    enterPlayer.health = enterPlayer.maxHealth;
                }
            }
            else if (itemIndex == 5)
            {
                if (enterPlayer.bomb < enterPlayer.maxBomb)
                {
                    enterPlayer.bomb += itemValue[itemIndex];
                    if (enterPlayer.bomb > enterPlayer.maxBomb)
                    {
                        enterPlayer.bomb = enterPlayer.maxBomb;
                    }
                }
                else
                {
                    enterPlayer.bomb = enterPlayer.maxBomb;
                }
            }
        }
    }

    IEnumerator Talk()
    {
        talkText.text = talkData[1];
        yield return new WaitForSeconds(2f);
        talkText.text = talkData[0];
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
