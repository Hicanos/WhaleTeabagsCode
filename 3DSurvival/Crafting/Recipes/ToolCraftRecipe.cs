using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ToolCraftRecipe", menuName = "Crafting/ToolCraftRecipe")]
public class ToolCraftRecipe : RecipeBase
{
    // 도구 제작 레시피 결과물
    [Header("ToolCraftResult")]
    public EquipItem toolData; // 결과로 나올 장비 아이템 데이터
    public int toolItemAmount=1; // 결과로 나올 장비 아이템의 개수: 기본값 1
}
