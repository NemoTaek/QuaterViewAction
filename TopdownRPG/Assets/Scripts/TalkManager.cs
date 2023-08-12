using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkManager : MonoBehaviour
{
    public Dictionary<int, string[]> talkData;
    Dictionary<int, Sprite> portraitData;
    public Sprite[] portraitArray;

    void Awake()
    {
        talkData = new Dictionary<int, string[]>();
        portraitData = new Dictionary<int, Sprite>();
        GenerateData();
    }

    // 대화창을 만드는 함수
    // 대화할 대상의 id와 대사 배열을 넣어 대화창 생성
    void GenerateData()
    {
        talkData.Add(100, new string[] { "평범한 상자이다." });

        talkData.Add(101, new string[] { "평범한 책상이다. \n왠지 저장할 수 있을 것 같다." });

        talkData.Add(102, new string[] { "이런곳에 왜 상자가 있을까..?" });

        talkData.Add(1000, new string[] { "안녕?:0", "나는 루나라고해!:1", "이 마을은 처음이구나? \n반가워!:2" });
        portraitData.Add(1000 + 0, portraitArray[0]);
        portraitData.Add(1000 + 1, portraitArray[1]);
        portraitData.Add(1000 + 2, portraitArray[2]);
        portraitData.Add(1000 + 3, portraitArray[3]);
        talkData.Add(1000 + 10, new string[] { "저 옆 마을로 가면 내 남동생이 있는데,\n그녀석한테 한번 말 걸어볼래?:0" });

        talkData.Add(1100, new string[] { "안녕?:0", "나는 루도라고해.:1", "여기서 수영을 즐겨보지 않을래?:2" });
        portraitData.Add(1100 + 0, portraitArray[4]);
        portraitData.Add(1100 + 1, portraitArray[5]);
        portraitData.Add(1100 + 2, portraitArray[6]);
        portraitData.Add(1100 + 3, portraitArray[7]);
        talkData.Add(1100 + 10, new string[] { "누나가 또 나한테 일을 떠넘겼군..:0", "그러면 나도 일을 떠넘겨야겠어!:1", "호수 근처에 있는 상자에 있는 물건을 가져와줄래?:2" });

        talkData.Add(102 + 20, new string[] { "이거 말하는건가? 얼른 갖다주자.", "상자 안에서 무언가를 획득하였다!" });
        talkData.Add(1100 + 20, new string[] { "오, 고마워! 이것만 있으면...!!:2" });
    }

    // 해당 id에 맞는 대화를 가져오는 함수
    public string GetTalk(int id, int talkIndex)
    {
        // 대화 개수 전까지는 대화가 출력되고, 대화가 모두 끝나면 null 출력
        if(talkIndex == talkData[id].Length) { return null; }
        else return talkData[id][talkIndex];
    }

    public Sprite GetPortrait(int id, int portraitIndex)
    {
        return portraitData[id + portraitIndex];
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
