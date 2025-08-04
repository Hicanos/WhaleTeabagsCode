using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CookCraftRecipe", menuName = "Crafting/CookCraftRecipe")]
public class CookCraftRecipe : RecipeBase
{
    // 요리레시피 결과물
    [Header("CookCraftResult")]
    public ConsumeItem consumableData; // 결과로 나올 소비 아이템 데이터
    public int cookItemAmount; // 결과로 나올 소비 아이템의 개수
}
