using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ScriptableObject: ����Ƽ���� �����ϴ� �뷮�� �����͸� �����ϴµ� ����� �� �ִ� ������ �����̳�
// MonoBehaviour ��ũ��Ʈ�� ������� �ʴ� �����͸� �����ϴ� �������� ����� �� ����
// �޸𸮿� ��ũ���͸� ������Ʈ�� ������ �纻�� �����ϰ� �̸� �����ϴ� ������� �۵�
[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Object / ItemData")]   // Ŀ���� �޴��� �����ϴ� �Ӽ�
public class ItemData : ScriptableObject
{
    public enum ItemType { Melee, Range, Glove, Shoes, Missile, Spear, Boomerang, ShotGun, Heal=99 };

    [Header("------- Main Info -------")]
    public ItemType itemType;
    public int itemId;
    public string itemName;
    [TextArea]
    public string itemDesc;
    public Sprite itemIcon;
    public bool isActive;

    [Header("------- Level Data -------")]
    public float baseDamage;
    public int baseCount;
    public float baseSpeed;
    public float[] damages;
    public int[] counts;

    [Header("------- Weapon -------")]
    public GameObject projectile;   // �߻�ü ��� ��
    public Sprite hand;
}
