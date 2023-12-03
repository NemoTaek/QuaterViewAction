using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUseItemButton : MonoBehaviour
{
    public int id;
    public Sprite sprite;
    Image image;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    void Start()
    {
        image.sprite = sprite;
    }

    void Update()
    {
        
    }
}
