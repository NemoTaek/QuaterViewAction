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

    // ��ȭâ�� ����� �Լ�
    // ��ȭ�� ����� id�� ��� �迭�� �־� ��ȭâ ����
    void GenerateData()
    {
        talkData.Add(100, new string[] { "����� �����̴�." });

        talkData.Add(101, new string[] { "����� å���̴�. \n���� ������ �� ���� �� ����." });

        talkData.Add(102, new string[] { "�̷����� �� ���ڰ� ������..?" });

        talkData.Add(1000, new string[] { "�ȳ�?:0", "���� �糪�����!:1", "�� ������ ó���̱���? \n�ݰ���!:2" });
        portraitData.Add(1000 + 0, portraitArray[0]);
        portraitData.Add(1000 + 1, portraitArray[1]);
        portraitData.Add(1000 + 2, portraitArray[2]);
        portraitData.Add(1000 + 3, portraitArray[3]);
        talkData.Add(1000 + 10, new string[] { "�� �� ������ ���� �� �������� �ִµ�,\n�׳༮���� �ѹ� �� �ɾ��?:0" });

        talkData.Add(1100, new string[] { "�ȳ�?:0", "���� �絵�����.:1", "���⼭ ������ ��ܺ��� ������?:2" });
        portraitData.Add(1100 + 0, portraitArray[4]);
        portraitData.Add(1100 + 1, portraitArray[5]);
        portraitData.Add(1100 + 2, portraitArray[6]);
        portraitData.Add(1100 + 3, portraitArray[7]);
        talkData.Add(1100 + 10, new string[] { "������ �� ������ ���� ���Ѱ屺..:0", "�׷��� ���� ���� ���Ѱܾ߰ھ�!:1", "ȣ�� ��ó�� �ִ� ���ڿ� �ִ� ������ �������ٷ�?:2" });

        talkData.Add(102 + 20, new string[] { "�̰� ���ϴ°ǰ�? �� ��������.", "���� �ȿ��� ���𰡸� ȹ���Ͽ���!" });
        talkData.Add(1100 + 20, new string[] { "��, ����! �̰͸� ������...!!:2" });
    }

    // �ش� id�� �´� ��ȭ�� �������� �Լ�
    public string GetTalk(int id, int talkIndex)
    {
        // ��ȭ ���� �������� ��ȭ�� ��µǰ�, ��ȭ�� ��� ������ null ���
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
