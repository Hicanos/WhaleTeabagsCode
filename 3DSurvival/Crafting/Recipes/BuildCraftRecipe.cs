using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildCraftRecipe", menuName = "Crafting/BuildCraftRecipe")]
public class BuildCraftRecipe : RecipeBase
{
    [Header("BuildCraftResult")]
    public BuildItemData buildItemData; // 결과로 나올 빌드 아이템 데이터
    public int buildItemAmount; //결과로 나올 빌드 아이템의 개수
}
