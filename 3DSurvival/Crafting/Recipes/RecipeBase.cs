using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeBase : ScriptableObject
{
    [Header("Recipe")]

    public string recipeName; //레시피 이름
    public Sprite recipeIcon; //레시피 아이콘
    public ItemData[] requiredItems; //레시피에 필요한 아이템들
    public int[] requiredItemAmounts; //각 아이템의 필요 개수
}
