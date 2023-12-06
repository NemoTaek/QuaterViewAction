using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Dictionary ������ Json �����ͷ� �ٷ� �Ľ��� �ȵȴ�.
// �׷��� Json���� �Ľ� ������ List�� ��ȯ ��(��ó��)�� �Ľ��Ͽ� �����Ѵ�.
// ����ȭ: ������ ���� �Ǵ� ���� ������Ʈ�� ����Ƽ�� �����ϰ� ���߿� �ٽ� ������ �� �ִ� �������� ��ȯ�ϴ� �ڵ� ���μ���
// ����ȭ ����: Serializable Ŭ���� ���� -> ���� ���� -> Serializable Ŭ������ ������ �� ��� -> �̸� ����ȭ �Ͽ� ������ ���Ͽ� ���
// ������ȭ ����: ���� ���� -> ���Ͽ� ����� �� �Ľ� -> �ش� �����͸� Serializable Ŭ������ ��� -> �̸� ���� ����� ������ �Ҵ�
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

        // Dictionary�� List�� ���� �� Json���� �Ľ�
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
        // JSON ���ڿ��� List�� �Ľ�
        DataArray<TKey, TValue> dataList = JsonUtility.FromJson<DataArray<TKey, TValue>>(dataJson);
        
        // List�� ��ȸ�ϸ� Dictionary�� ����
        Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
        for(int i=0; i<dataList.data.Count; i++)
        {
            DataDictionary<TKey, TValue> dataDictionary = dataList.data[i];
            dictionary[dataDictionary.key] = dataDictionary.value;
        }
        return dictionary;
    }
}
