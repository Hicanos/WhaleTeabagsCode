using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

public class CraftingSystem : MonoBehaviour
{
    // 크래프팅 시스템의 기본 골조
    // 재료 배열과 필요 개수의 배열을 매치시킴 (예: 재료[0]은 필요 개수[0]만큼 필요)
    // 레시피는 각각에 해당하는 것을 가져옴. BuiildCraftRecipe, CookingRecipe 등
    // 빌드 데이터는 데이터상으로만 추가되지만, 도구 제작 등은 완성 후 인벤토리에 추가되어야 함.
    // 외부에서 선택된 레시피를 받아와서 처리한다.
    // 제작하기 버튼을 누르면 결과물을 정해진 곳에 추가한다. (BuildCraftRecipe는 ResourceManager에 데이터를 추가, 나머지는 인벤토리에 추가)
    // 레시피를 받으면 그만큼 UI에 아이템 슬롯을 만들어서 보여준다.

    [Header("제작 레시피 설정")]
    public List<RecipeBase> recipes; // 크래프팅 레시피 목록 <빈 리스트에 아래에 선택된 레시피를 추가하며 사용
    public List<BuildCraftRecipe> buildCraftRecipes; // 빌드 크래프트 레시피 목록
    public List<CookCraftRecipe> cookingRecipes; // 요리 레시피 목록
    public List<ToolCraftRecipe> toolRecipes; // 도구 레시피 목록

    [Header("크래프팅 버튼")]
    [SerializeField] private Button BuildRecipeBtn; // 크래프팅 버튼
    [SerializeField] private Button CooktRecipeBtn; // 요리 버튼
    [SerializeField] private Button ToolRecipeBtn; // 도구 제작 버튼
    [SerializeField] private Button CraftButton; // 제작하기 버튼

    [Header("현재 선택된 레시피")]
    [SerializeField] RecipeBase curRecipe; // 현재 선택된 레시피
    [SerializeField] GameObject recipeSlotPrefab; // 레시피 슬롯 프리팹
    [SerializeField] GameObject resourceSlotPrefab; // 재료 슬롯 프리팹

    [Header("UI Slot 설정")]
    //RecipeSlot을 할당할 부모 컴포넌드
    public Transform recipeSlotParent;

    //재료를 할당할 부모 컴포넌트
    public Transform ResourcesParent;

    [Header("결과물 아이콘")]
    [SerializeField] private Image resultIcon; // 결과물 아이콘

    UIInventory inventory; // UI 인벤토리 참조

    public void Reset()
    {
        // 크래프팅 시스템 초기화
        recipes.Clear(); // 레시피 목록 초기화
        curRecipe = null; // 현재 선택된 레시피 초기화
        resultIcon.sprite = null; // 결과물 아이콘 초기화
        resultIcon.gameObject.SetActive(false); // 결과물 아이콘 비활성화
        // UI 슬롯 초기화
        foreach (Transform child in recipeSlotParent)
        {
            Destroy(child.gameObject); // 기존 레시피 슬롯 제거
        }
        foreach (Transform child in ResourcesParent)
        {
            Destroy(child.gameObject); // 기존 재료 슬롯 제거
        }

    }

    // 레시피를 선택하면 해당 레시피에 맞는 재료 슬롯을 생성하고 UI를 업데이트
    public void SelectRecipe(RecipeBase recipe)
    {
        //recipe가 null이면 return;
        if (recipe == null) return;

        // 슬롯을 누르면, 해당 슬롯에 할당된 레시피데이터를 업데이트
        curRecipe = recipe;

        ResourceSlotReset(); // 재료 슬롯 초기화

    }

    void ResourceSlotReset()
    {
        foreach (Transform child in ResourcesParent)
        {
            Destroy(child.gameObject);
        }
        UpdateRecipeUI(); // 재료 슬롯 초기화 후 UI 업데이트
    }

    public void SetRecipeUI()
    {

        // 레시피 목록을 순회하며 UI에 레시피 슬롯을 생성. 어떤 버튼을 눌렀느냐에 따라 나타나는 Recipe가 달라짐.
        foreach (RecipeBase recipe in recipes)
        {
            // 레시피 슬롯을 생성
            GameObject slot = Instantiate(recipeSlotPrefab, recipeSlotParent);
            // 슬롯에 RecipeBase를 할당
            CraftSlot recipeSlot = slot.GetComponent<CraftSlot>();
            recipeSlot.SetRecipe(recipe);
        }

    }


    // 현재 선택된 레시피에 따라 UI를 업데이트
    public void UpdateRecipeUI()
    {


        // 현재 선택된 레시피가 null이 아니면 UI 업데이트
        // curRecipe가 null이 아니니까 여기로 왔잖아
        if (curRecipe != null)
        {
            // 결과물 아이콘 업데이트
            resultIcon.gameObject.SetActive(true);
            resultIcon.sprite = curRecipe.recipeIcon;

            // 재료 슬롯 업데이트 (재료 아이콘과 개수 표시. 나무2, 돌1 등)
            for (int i = 0; i < curRecipe.requiredItems.Length; i++)
            {
                // 재료 아이템과 개수를 가져옴
                ItemData itemData = curRecipe.requiredItems[i];
                int requiredCount = curRecipe.requiredItemAmounts[i];

                Debug.LogWarning(Equals(itemData, null) ? "아이템 데이터가 null입니다." : $"{i}아이템: {itemData.itemName}, 필요 개수: {requiredCount}");
                //StartCoroutine("ResetResourceSlot"); // 재료 슬롯 초기화
                //재료 슬롯 생성(개수가 늘어날 때마다 추가 생성)
                GameObject slotObj =  Instantiate(resourceSlotPrefab, ResourcesParent);
                //생성된 슬롯에 지정된 재료들의 아이콘과 개수를 설정
                ResourceSlot slot = slotObj.GetComponent<ResourceSlot>();
                slot.SetItem(itemData, requiredCount); 
            }

            //CraftSlot의 maxCount가 1이상이면, 제작 버튼 활성화

        }
        else
        {
            // 결과 아이콘 초기화
            resultIcon.sprite = null;
            //resultIcon의 오브젝트를 비활성화
            resultIcon.gameObject.SetActive(false);
        }
    }

    public void OnCraft()
    {
        if (curRecipe == null) return;
        // 제작 완료 했으니, CraftSlot에서 maxCount를 재설정
        CraftSlot[] craftSlots = FindObjectsOfType<CraftSlot>();

        CraftSlot craftSlot = null;
        foreach (var slot in craftSlots)
        {
            if (slot.CurRecipeData == curRecipe) // CraftSlot에 Recipe 프로퍼티가 있다고 가정
            {
                craftSlot = slot;
                break;
            }
        }

        if (craftSlot == null)
        {
            Debug.LogError("[CraftingSystem] 선택된 레시피에 해당하는 CraftSlot을 찾을 수 없습니다.");
            return;
        }

        //현재 선택된 레시피의 maxCount를 가져옴
        craftSlot.GetMaxCount(); // 현재 레시피에 따라 제작 가능한 최대 개수 계산

        if (craftSlot.maxCount <= 0)
        {
            
            Debug.Log($"[CraftingSystem] 제작 가능한 개수가 0입니다. 제작을 진행할 수 없습니다.");
            return; // 제작 가능한 개수가 0이면 아무 작업도 하지 않음
        }

        // 인벤토리에서 재료가 충분한지 확인 - CraftSlot에서 재료를 확인하는 로직이 필요

        // 현재 레시피에 따라 크래프팅 실행
        // 결과물이 BuildItem이면 ResourceManager에 추가
        // 결과물이 ToolRecipe나 CookingRecipe이면 인벤토리에 추가
        inventory = GameManager.Instance.uiInventory;

        if (curRecipe is BuildCraftRecipe buildRecipe)
        {
            // 빌드 레시피인 경우, ResourceManager에 추가
            // ResourceManager에 빌드 아이템 데이터와 개수를 추가
            // 리소스 매니저가 필요
            ResourceManager resourceManager = FindObjectOfType<ResourceManager>();
            if(resourceManager != null)
            {
                //이미 재료 확인은 마친 상황에서 여기로 옴
                // 리소스 매니저에 빌드 아이템 데이터와 개수를 추가
                resourceManager.AddResource(buildRecipe.buildItemData.itemID, buildRecipe.buildItemAmount);
                Debug.Log($"[CraftingSystem] 빌드 레시피 완료: {buildRecipe.buildItemData.name} x{buildRecipe.buildItemAmount}");

                // 빌드 아이템이 추가되었으므로, 현재 레시피에서 필요한 재료를 소모시킴
                // 재료 아이템은 UIInventory에서 관리되며, resourceManager에는 제작된 오브젝트만 추가됨

                for(int i = 0; i < buildRecipe.requiredItems.Length; i++)
                {
                    ItemData itemData = buildRecipe.requiredItems[i];
                    int requiredCount = buildRecipe.requiredItemAmounts[i];
                    //UIInventory에서 해당 아이템의 개수를 가져와서 소모
                    inventory.RemoveItem(itemData, requiredCount);
                }


            }
            OnBuildButton(); 

        }
        else if(curRecipe is CookCraftRecipe cookRecipe)
        {
           inventory.AddCraftItem(cookRecipe.consumableData, cookRecipe.cookItemAmount);
            Debug.Log($"[CraftingSystem] 요리 레시피 완료: {cookRecipe.consumableData.itemName} x{cookRecipe.cookItemAmount}");
            // 요리 아이템이 추가되었으므로, 현재 레시피에서 필요한 재료를 소모시킴
            for (int i = 0; i < cookRecipe.requiredItems.Length; i++)
            {
                ItemData itemData = cookRecipe.requiredItems[i];
                int requiredCount = cookRecipe.requiredItemAmounts[i];
                //UIInventory에서 해당 아이템의 개수를 가져와서 소모
                inventory.RemoveItem(itemData, requiredCount);
            }
            OnCookButton();
        }
        else if (curRecipe is ToolCraftRecipe toolRecipe)
        {
            // 도구 레시피인 경우, 인벤토리에 추가
            inventory.AddCraftItem(toolRecipe.toolData, toolRecipe.toolItemAmount);
            Debug.Log($"[CraftingSystem] 도구 레시피 완료: {toolRecipe.toolData.itemName} x{toolRecipe.toolItemAmount}");
            // 도구 아이템이 추가되었으므로, 현재 레시피에서 필요한 재료를 소모시킴
            for (int i = 0; i < toolRecipe.requiredItems.Length; i++)
            {
                ItemData itemData = toolRecipe.requiredItems[i];
                int requiredCount = toolRecipe.requiredItemAmounts[i];
                //UIInventory에서 해당 아이템의 개수를 가져와서 소모
                inventory.RemoveItem(itemData, requiredCount);
            }
            OnToolButton();
        }
    }


    public void OnBuildButton()
    {
        // 빌드 레시피 버튼을 클릭할 경우, 빌드 레시피배열을 레시피에 할당하고 SetRecipeUI를 호출
        curRecipe = null; // 현재 선택된 레시피 초기화
        // 리스트를 비우기
        recipes.Clear(); // 레시피 목록 초기화
        // 생성된 슬롯 모두 제거

        foreach (Transform child in recipeSlotParent)
        {
            Destroy(child.gameObject);
        }

        recipes = buildCraftRecipes.Cast<RecipeBase>().ToList(); // 빌드 레시피를 레시피 목록에 할당
        SetRecipeUI(); // 레시피 UI 업데이트
    }

    public void OnCookButton()
    {
        // 요리 레시피 버튼을 클릭할 경우, 요리 레시피를 레시피에 할당하고 SetRecipeUI를 호출
        curRecipe = null; // 현재 선택된 레시피 초기화
        // 리스트를 비우기
        recipes.Clear(); // 레시피 목록 초기화
        // 생성된 슬롯 모두 제거

        foreach (Transform child in recipeSlotParent)
        {
            Destroy(child.gameObject);
        }

        recipes = cookingRecipes.Cast<RecipeBase>().ToList(); // 요리 레시피를 레시피 목록에 할당
        SetRecipeUI(); // 레시피 UI 업데이트
    }

    public void OnToolButton()
    {
        // 도구 레시피 버튼을 클릭할 경우, 도구 레시피를 레시피에 할당하고 SetRecipeUI를 호출
        curRecipe = null; // 현재 선택된 레시피 초기화
                          // 리스트를 비우기
        recipes.Clear(); // 레시피 목록 초기화
                         // 생성된 슬롯 모두 제거

        foreach (Transform child in recipeSlotParent)
        {
            Destroy(child.gameObject);
        }

        recipes = toolRecipes.Cast<RecipeBase>().ToList(); // 도구 레시피를 레시피 목록에 할당
        SetRecipeUI(); // 레시피 UI 업데이트
    }


}
