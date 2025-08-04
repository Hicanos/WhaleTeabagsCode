using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class BuffData
{
    /// <summary>
    /// Key
    /// </summary>
    public int Key;

    /// <summary>
    /// Key
    /// </summary>
    public string CharID;

    /// <summary>
    /// Buff_Name
    /// </summary>
    public string NameKr;

    /// <summary>
    /// Buff_Name_En
    /// </summary>
    public string NameEn;

    /// <summary>
    /// Buff_Type
    /// </summary>
    public DesignEnums.BuffType BuffType;

    /// <summary>
    /// Buff_Category
    /// </summary>
    public DesignEnums.BuffCategory BuffCategory;

    /// <summary>
    /// Buff_Effect
    /// </summary>
    public string EffectDescription;

    /// <summary>
    /// Buff_Value
    /// </summary>
    public float Value;

    /// <summary>
    /// Buff_Maxtack
    /// </summary>
    public int Stack;

}
public class BuffDataLoader
{
    public List<BuffData> ItemsList { get; private set; }
    public Dictionary<int, BuffData> ItemsDict { get; private set; }

    public BuffDataLoader(string path = "JSON/BuffData")
    {
        string jsonData;
        jsonData = Resources.Load<TextAsset>(path).text;
        ItemsList = JsonUtility.FromJson<Wrapper>(jsonData).Items;
        ItemsDict = new Dictionary<int, BuffData>();
        foreach (var item in ItemsList)
        {
            ItemsDict.Add(item.Key, item);
        }
    }

    [Serializable]
    private class Wrapper
    {
        public List<BuffData> Items;
    }

    public BuffData GetByKey(int key)
    {
        if (ItemsDict.ContainsKey(key))
        {
            return ItemsDict[key];
        }
        return null;
    }
    public BuffData GetByIndex(int index)
    {
        if (index >= 0 && index < ItemsList.Count)
        {
            return ItemsList[index];
        }
        return null;
    }
}
