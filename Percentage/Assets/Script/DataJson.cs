using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Dictionary 형식은 Json 데이터로 바로 파싱이 안된다.
// 그래서 Json으로 파싱 가능한 List로 변환 후(후처리)에 파싱하여 저장한다.
// 직렬화: 데이터 구조 또는 게임 오브젝트를 유니티가 보관하고 나중에 다시 복구할 수 있는 포맷으로 변환하는 자동 프로세스
// 직렬화 과정: Serializable 클래스 선언 -> 파일 생성 -> Serializable 클래스에 저장할 값 담기 -> 이를 직렬화 하여 생성한 파일에 담기
// 역직렬화 과정: 파일 열기 -> 파일에 저장된 값 파싱 -> 해당 데이터를 Serializable 클래스에 담기 -> 이를 실제 사용할 변수에 할당
[Serializable]
public class DataDictionary<TKey, TValue>
{
    public TKey key;
    public TValue value;
}

[Serializable]
public class DataArray<TKey, TValue>
{
    public List<DataDictionary<TKey, TValue>> data;
}

public static class DataJson
{
    // Dictionary -> List -> Json
    public static string DictionaryToJson<Tkey, TValue>(Dictionary<Tkey, TValue> dictionary)
    {
        List<DataDictionary<Tkey, TValue>> dataList = new List<DataDictionary<Tkey, TValue>>();
        DataDictionary<Tkey, TValue> dataDictionary;

        // Dictionary를 List에 저장 후 Json으로 파싱
        foreach (Tkey key in dictionary.Keys)
        {
            dataDictionary = new DataDictionary<Tkey, TValue>();
            dataDictionary.key = key;
            dataDictionary.value = dictionary[key];
            dataList.Add(dataDictionary);
        }
        DataArray<Tkey, TValue> dataArray = new DataArray<Tkey, TValue>();
        dataArray.data = dataList;

        return JsonUtility.ToJson(dataArray);
    }

    // JSON -> List -> Dictionary
    public static Dictionary<TKey,TValue> DictionaryFromJson<TKey, TValue>(string dataJson)
    {
        // JSON 문자열을 List로 파싱
        DataArray<TKey, TValue> dataList = JsonUtility.FromJson<DataArray<TKey, TValue>>(dataJson);
        
        // List를 순회하며 Dictionary에 저장
        Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
        for(int i=0; i<dataList.data.Count; i++)
        {
            DataDictionary<TKey, TValue> dataDictionary = dataList.data[i];
            dictionary[dataDictionary.key] = dataDictionary.value;
        }
        return dictionary;
    }
}
