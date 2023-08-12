using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ScriptableObject: ����Ƽ���� �����ϴ� �뷮�� �����͸� �����ϴµ� ����� �� �ִ� ������ �����̳�
// MonoBehaviour ��ũ��Ʈ�� ������� �ʴ� �����͸� �����ϴ� �������� ����� �� ����
// �޸𸮿� ��ũ���͸� ������Ʈ�� ������ �纻�� �����ϰ� �̸� �����ϴ� ������� �۵�
[CreateAssetMenu(fileName = "Character", menuName = "Scriptable Object / CharacterData")]   // Ŀ���� �޴��� �����ϴ� �Ӽ�
public class CharacterData : ScriptableObject
{
    [Header("------- Main Info -------")]
    public int characterId;
    public string characterName;
    [TextArea]
    public string characterDesc;
    public Sprite characterIcon;
    public bool isActive;

    [Header("------- Stat Data -------")]
    public float baseDamage;
    public int baseCount;
    public float baseSpeed;
    public float baseWeaponSpeed;
    public float baseWeaponRate;
}
