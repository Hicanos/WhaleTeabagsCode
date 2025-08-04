using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CharData
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
    /// Name
    /// </summary>
    public string NameKr;

    /// <summary>
    /// Name_En
    /// </summary>
    public string NameEn;

    /// <summary>
    /// ClassType
    /// </summary>
    public DesignEnums.ClassTypes ClassType;

    /// <summary>
    /// Char_PlusATK
    /// </summary>
    public int BaseATK;

    /// <summary>
    /// Char_Stat_DEF
    /// </summary>
    public int PlusATK;

    /// <summary>
    /// Char_PlusDEF
    /// </summary>
    public int BaseDEF;

    /// <summary>
    /// Char_Stat_HP
    /// </summary>
    public int PlusDEF;

    /// <summary>
    /// Char_HP_Bonus
    /// </summary>
    public int BaseHP;

    /// <summary>
    /// Char_Stat_CriC(%)
    /// </summary>
    public int PlusHP;

    /// <summary>
    /// Char_Stat_CriD
    /// </summary>
    public float CritChance;

    /// <summary>
    /// Char_Stat_Pene
    /// </summary>
    public float CritDamage;

    /// <summary>
    /// Char_Stat_Pene
    /// </summary>
    public int Penetration;

    /// <summary>
    /// E_DMG
    /// </summary>
    public float ElementDMG;

    /// <summary>
    /// E_Type
    /// </summary>
    public DesignEnums.ElementTypes ElementType;

    /// <summary>
    /// Cha_Des
    /// </summary>
    public string Description;

    /// <summary>
    /// Cha_DLog1
    /// </summary>
    public string dialog1;

    /// <summary>
    /// Cha_DLog2
    /// </summary>
    public string dialog2;

    /// <summary>
    /// DiceID
    /// </summary>
    public string DiceID;

    /// <summary>
    /// ActiveSkil_lID
    /// </summary>
    public string ActiveSkill;

    /// <summary>
    /// PassiveSkill_ID
    /// </summary>
    public string PassiveSkill;

    /// <summary>
    /// BattlePrefabPath
    /// </summary>
    public string BattlePrefabPath;

    /// <summary>
    /// IconPath
    /// </summary>
    public string IconPath;

    /// <summary>
    /// UpperPath
    /// </summary>
    public string UpperPath;

    /// <summary>
    /// StandingPath
    /// </summary>
    public string StandingPath;

    /// <summary>
    /// DicePrefabPath
    /// </summary>
    public string DicePrefabPath;

    public string ElementalIconPath;
    public string RoleIconPath;
    public string BackGroundPath;
    public string DiceNumIconPath;

}
public class CharDataLoader
{
    public List<CharData> ItemsList { get; private set; }
    public Dictionary<int, CharData> ItemsDict { get; private set; }
    public Dictionary<string, CharData> ItemsByCharID { get; private set; }

    public CharDataLoader(string path = "JSON/CharData")
    {
        string jsonData;
        jsonData = Resources.Load<TextAsset>(path).text;
        ItemsList = JsonUtility.FromJson<Wrapper>(jsonData).Items;
        ItemsDict = new Dictionary<int, CharData>();
        ItemsByCharID = new Dictionary<string, CharData>();
        foreach (var item in ItemsList)
        {
            ItemsDict.Add(item.Key, item);
            ItemsByCharID[item.CharID] = item;
        }
    }

    [Serializable]
    private class Wrapper
    {
        public List<CharData> Items;
    }

    public CharData GetByKey(int key)
    {
        if (ItemsDict.ContainsKey(key))
        {
            return ItemsDict[key];
        }
        return null;
    }
    public CharData GetByIndex(int index)
    {
        if (index >= 0 && index < ItemsList.Count)
        {
            return ItemsList[index];
        }
        return null;
    }
    public CharData GetByCharID(string charID)
    {
        if (ItemsByCharID.ContainsKey(charID))
            return ItemsByCharID[charID];
        return null;
    }
}
