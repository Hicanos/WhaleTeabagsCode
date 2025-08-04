using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class CharDiceData
{
    /// <summary>
    /// Key
    /// </summary>
    public int Key;

    /// <summary>
    /// Key
    /// </summary>
    public string DiceID;

    /// <summary>
    /// Dice_Name
    /// </summary>
    public string NameKr;

    /// <summary>
    /// Name_En
    /// </summary>
    public string NameEn;

    /// <summary>
    /// C_No
    /// </summary>
    public int CignatureNo;

    /// <summary>
    /// C_Face
    /// </summary>
    public string CignatureFace;

    /// <summary>
    /// Dice_F1
    /// </summary>
    public bool CFace1;

    /// <summary>
    /// Dice_F2
    /// </summary>
    public bool CFace2;

    /// <summary>
    /// Dice_F3
    /// </summary>
    public bool CFace3;

    /// <summary>
    /// Dice_F4
    /// </summary>
    public bool CFace4;

    /// <summary>
    /// Dice_F5
    /// </summary>
    public bool CFace5;

    /// <summary>
    /// Dice_F6
    /// </summary>
    public bool CFace6;

    /// <summary>
    /// Dice_P1
    /// </summary>
    public int FaceProbability1;

    /// <summary>
    /// Dice_P2
    /// </summary>
    public int FaceProbability2;

    /// <summary>
    /// Dice_P3
    /// </summary>
    public int FaceProbability3;

    /// <summary>
    /// Dice_P4
    /// </summary>
    public int FaceProbability4;

    /// <summary>
    /// Dice_P5
    /// </summary>
    public int FaceProbability5;

    /// <summary>
    /// Dice_P6
    /// </summary>
    public int FaceProbability6;

    /// <summary>
    /// Dice_Des
    /// </summary>
    public string Description;

}
public class DiceDataLoader
{
    public List<CharDiceData> ItemsList { get; private set; }
    public Dictionary<int, CharDiceData> ItemsDict { get; private set; }

    public DiceDataLoader(string path = "JSON/DiceTable")
    {
        string jsonData;
        jsonData = Resources.Load<TextAsset>(path).text;
        ItemsList = JsonUtility.FromJson<Wrapper>(jsonData).Items;
        ItemsDict = new Dictionary<int, CharDiceData>();
        foreach (var item in ItemsList)
        {
            ItemsDict.Add(item.Key, item);
        }
    }

    [Serializable]
    private class Wrapper
    {
        public List<CharDiceData> Items;
    }

    public CharDiceData GetByKey(int key)
    {
        if (ItemsDict.ContainsKey(key))
        {
            return ItemsDict[key];
        }
        return null;
    }
    public CharDiceData GetByIndex(int index)
    {
        if (index >= 0 && index < ItemsList.Count)
        {
            return ItemsList[index];
        }
        return null;
    }
}
