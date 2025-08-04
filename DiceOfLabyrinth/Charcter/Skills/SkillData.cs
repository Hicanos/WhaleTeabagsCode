using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class SkillData
{
    /// <summary>
    /// Key
    /// </summary>
    public int Key;

    /// <summary>
    /// Key
    /// </summary>
    public string SkillID;

    /// <summary>
    /// Name
    /// </summary>
    public string NameKr;

    /// <summary>
    /// Name_En
    /// </summary>
    public string NameEn;

    /// <summary>
    /// Skill_Type
    /// </summary>
    public DesignEnums.SkillType SkillType;

    /// <summary>
    /// Skill_Cost
    /// </summary>
    public int SkillCost;

    /// <summary>
    /// Skill_Target
    /// </summary>
    public DesignEnums.SkillTarget Target;

    /// <summary>
    /// Skill_Rule
    /// </summary>
    public DesignEnums.SkillRule SkillRule;

    /// <summary>
    /// Skill_Effect
    /// </summary>
    public string SkillDescription;

    /// <summary>
    /// BuffTable
    /// </summary>
    public string BuffID;

    /// <summary>
    /// BuffTable2
    /// </summary>
    public string BuffID_2;

    /// <summary>
    /// Skill_Value
    /// </summary>
    public float SkillValue;

    /// <summary>
    /// Plus_Skill_Value
    /// </summary>
    public float PlusSkillValue;

    /// <summary>
    /// Buff_Value
    /// </summary>
    public float BuffValue;

    /// <summary>
    /// Plus_Buff_Value
    /// </summary>
    public float PlusBuffValue;

    /// <summary>
    /// Buff_Probability(%)
    /// </summary>
    public int BuffProbability;

    /// <summary>
    /// Buff_Probability(%)
    /// </summary>
    public int PlusBuffProbability;

    /// <summary>
    /// Buff_Turn
    /// </summary>
    public int BuffTurn;

    /// <summary>
    /// Skill_Cool
    /// </summary>
    public int CoolTime;

    /// <summary>
    /// Skill_Icon
    /// </summary>
    public string SkillIcon;

    public bool IsAttacking;

}
public class SkillDataLoader
{
    public List<SkillData> ItemsList { get; private set; }
    public Dictionary<int, SkillData> ItemsDict { get; private set; }

    public SkillDataLoader(string path = "JSON/SkillData")
    {
        string jsonData;
        jsonData = Resources.Load<TextAsset>(path).text;
        ItemsList = JsonUtility.FromJson<Wrapper>(jsonData).Items;
        ItemsDict = new Dictionary<int, SkillData>();
        foreach (var item in ItemsList)
        {
            ItemsDict.Add(item.Key, item);
        }
    }

    [Serializable]
    private class Wrapper
    {
        public List<SkillData> Items;
    }

    public SkillData GetByKey(int key)
    {
        if (ItemsDict.ContainsKey(key))
        {
            return ItemsDict[key];
        }
        return null;
    }
    public SkillData GetByIndex(int index)
    {
        if (index >= 0 && index < ItemsList.Count)
        {
            return ItemsList[index];
        }
        return null;
    }
}
