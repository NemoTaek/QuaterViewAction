using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public int questId; // 퀘스트 id
    public Dictionary<int, QuestData> questList;   // 퀘스트 리스트
    public int questActionIndex;    // 퀘스트에서의 대화 순서
    public GameObject[] questObject;    // 퀘스트에 필요한 게임 오브젝트

    void Awake()
    {
        questList = new Dictionary<int, QuestData>();
        GenerateData();
    }

    // 퀘스트 명과 퀘스트를 부여할 npc ID를 매개변수로 전달하여 생성
    void GenerateData()
    {
        questList.Add(0, new QuestData("우리 마을에 온걸 환영해!", new int[] { 1000 }));
        questList.Add(10, new QuestData("옆 마을 주민과 대화하기", new int[] { 1000, 1100 }));
        questList.Add(20, new QuestData("루도의 물건 찾아주기", new int[] { 102, 1100 }));
        questList.Add(30, new QuestData("근데.. 그게 뭐에요..?", new int[] { 1100 }));
    }

    public int GetQuestTalkIndex(int id)
    {
        return questId;
    }

    public string CheckQuest(int checkQuestId)
    {
        string questName = "";

        // 해당 퀘스트가 퀘스트 목록에 있는 NPC에 있으면 연계 퀘스트로 진행
        if (questList.ContainsKey(questId))
        {
            if (checkQuestId == questList[questId].npcId[questActionIndex] + questId)
            {
                questActionIndex++;
            }

            ControlObject();

            // 연계 퀘스트까지 다 끝냈으면 다음 퀘스트로 진행
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

    // 퀘스트 오브젝트를 관리하는 함수
    public void ControlObject()
    {
        switch(questId)
        {
            case 0:
                break;
            case 10:
                // 물건을 찾아달라는 대화를 들은 후에 상자 생성
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
                // 해당 퀘스트를 완료하면 상자 삭제
                if (questActionIndex == 1)
                {
                    questObject[0].SetActive(false);
                }
                break;
        }
    }
}
