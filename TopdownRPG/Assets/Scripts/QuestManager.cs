using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public int questId; // ����Ʈ id
    public Dictionary<int, QuestData> questList;   // ����Ʈ ����Ʈ
    public int questActionIndex;    // ����Ʈ������ ��ȭ ����
    public GameObject[] questObject;    // ����Ʈ�� �ʿ��� ���� ������Ʈ

    void Awake()
    {
        questList = new Dictionary<int, QuestData>();
        GenerateData();
    }

    // ����Ʈ ��� ����Ʈ�� �ο��� npc ID�� �Ű������� �����Ͽ� ����
    void GenerateData()
    {
        questList.Add(0, new QuestData("�츮 ������ �°� ȯ����!", new int[] { 1000 }));
        questList.Add(10, new QuestData("�� ���� �ֹΰ� ��ȭ�ϱ�", new int[] { 1000, 1100 }));
        questList.Add(20, new QuestData("�絵�� ���� ã���ֱ�", new int[] { 102, 1100 }));
        questList.Add(30, new QuestData("�ٵ�.. �װ� ������..?", new int[] { 1100 }));
    }

    public int GetQuestTalkIndex(int id)
    {
        return questId;
    }

    public string CheckQuest(int checkQuestId)
    {
        string questName = "";

        // �ش� ����Ʈ�� ����Ʈ ��Ͽ� �ִ� NPC�� ������ ���� ����Ʈ�� ����
        if (questList.ContainsKey(questId))
        {
            if (checkQuestId == questList[questId].npcId[questActionIndex] + questId)
            {
                questActionIndex++;
            }

            ControlObject();

            // ���� ����Ʈ���� �� �������� ���� ����Ʈ�� ����
            if (questActionIndex == questList[questId].npcId.Length)
            {
                NextQuest();
            }

            questName = questList[questId].questName;
        }

        return questName;
    }

    public string CheckQuest()
    {
        return questList[questId].questName;
    }

    void NextQuest()
    {
        questId += 10;
        questActionIndex = 0;
    }

    // ����Ʈ ������Ʈ�� �����ϴ� �Լ�
    public void ControlObject()
    {
        switch(questId)
        {
            case 0:
                break;
            case 10:
                // ������ ã�ƴ޶�� ��ȭ�� ���� �Ŀ� ���� ����
                if(questActionIndex == 2)
                {
                    questObject[0].SetActive(true);
                }
                break;
            case 20:
                if (questActionIndex == 0)
                {
                    questObject[0].SetActive(true);
                }
                // �ش� ����Ʈ�� �Ϸ��ϸ� ���� ����
                if (questActionIndex == 1)
                {
                    questObject[0].SetActive(false);
                }
                break;
        }
    }
}
