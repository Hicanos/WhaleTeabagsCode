using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BuffData.json을 런타임에 관리하는 싱글톤 매니저. SO 변환 없이 SkillSO 등에서 쉽게 접근 가능.
/// </summary>
public class BuffManager
{
    private static BuffManager _instance;
    public static BuffManager Instance => _instance ?? (_instance = new BuffManager());

    private Dictionary<string, BuffData> buffByCharID;
    private Dictionary<int, BuffData> buffByKey;
    private List<BuffData> buffList;

    private BuffManager()
    {
        LoadBuffData();
    }

    private void LoadBuffData()
    {
        // Resources/Json/BuffData.json 경로 기준
        var textAsset = Resources.Load<TextAsset>("Json/BuffData");
        if (textAsset == null)
        {
            Debug.LogError("BuffData.json을 찾을 수 없습니다.");
            buffList = new List<BuffData>();
            buffByCharID = new Dictionary<string, BuffData>();
            buffByKey = new Dictionary<int, BuffData>();
            return;
        }
        var wrapper = JsonUtility.FromJson<BuffDataWrapper>(textAsset.text);
        buffList = wrapper.Items ?? new List<BuffData>();
        buffByCharID = new Dictionary<string, BuffData>();
        buffByKey = new Dictionary<int, BuffData>();
        foreach (var buff in buffList)
        {
            if (!string.IsNullOrEmpty(buff.CharID))
                buffByCharID[buff.CharID] = buff;
            buffByKey[buff.Key] = buff;
        }
    }

    [Serializable]
    private class BuffDataWrapper
    {
        public List<BuffData> Items;
    }

    /// <summary>
    /// CharID로 BuffData 조회
    /// </summary>
    public BuffData GetBuffByCharID(string charID)
    {
        if (buffByCharID.TryGetValue(charID, out var buff))
            return buff;
        return null;
    }

    /// <summary>
    /// Key로 BuffData 조회
    /// </summary>
    public BuffData GetBuffByKey(int key)
    {
        if (buffByKey.TryGetValue(key, out var buff))
            return buff;
        return null;
    }

    /// <summary>
    /// 전체 BuffData 리스트 반환
    /// </summary>
    public List<BuffData> GetAllBuffs() => buffList;
}
