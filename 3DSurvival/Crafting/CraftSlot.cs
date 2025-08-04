using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CraftSlot : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI makeCountText; // 제작 가능한 개수 표시용 텍스트
    [SerializeField] private TextMeshProUGUI objectAmount; // 제작할 아이템의 1회 생산 시 만들어지는 개수 표시용 텍스트
    [SerializeField] private Button button; // 슬롯 버튼
    public int maxCount; // 제작 가능한 최대 개수

    private RecipeBase curRecipeData;
    //읽기전용 레시피 프로퍼티
    public RecipeBase CurRecipeData
    {
        get { return curRecipeData; }
    }
    private int slotIndex; // 슬롯의 순서
    private bool isAvailable; // 제작 가능한 아이템이면 Alpha 1, 제작 불가능한 아이템이면 Alpha 0.4로 표시


    UIInventory inventory; // UI 인벤토리 참조

    private void Awake()
    {
        button.onClick.AddListener(OnClickButton);
    }


    //Slot을 선택하면 다른 슬롯에 필요한 아이템 슬롯을 만들어 인스턴스
    //각 슬롯에는 아이템의 Icon과 개수를 보여주며, 버튼을 눌러 선택
    //선택된 아이템 유형 메뉴에 따라 나오는 슬롯이 바뀌어야함(BuileItem은 BuildItem들 제작법만, CookRecipe는 요리법만 등)


    //레시피슬롯에 레시피이미지를 설정. 오브젝트에 리스트로 저장된 레시피를 불러와서 SetRecipe를 반복한다.
    public void SetRecipe(RecipeBase recipeBase)
    {
        curRecipeData = recipeBase;
        if(curRecipeData != null)
        {
            // 레시피 데이터가 있다면 레시피의 아이콘과 현재 제작 가능한 개수를 표시
            iconImage.sprite = curRecipeData.recipeIcon;
            iconImage.gameObject.SetActive(true); // 아이콘 활성화

            // 제작할 아이템의 1회 생산 시 만들어지는 개수 표시
            // BuildItem은 buildItemAmount, CookRecipe는 cookItemAmount, ToolRecipe는 toolItemAmount 등으로 설정
            // 현재 레시피가 어떤 레시피냐에 따라 할당하는 내용이 달라짐

            if (curRecipeData is BuildCraftRecipe buildCraftRecipe)
            {
                objectAmount.text = "x"+buildCraftRecipe.buildItemAmount.ToString(); // 빌드 아이템의 개수 표시
            }
            else if (curRecipeData is CookCraftRecipe cookRecipe)
            {
                objectAmount.text = "x"+cookRecipe.cookItemAmount.ToString(); // 요리 아이템의 개수 표시
            }
            else if (curRecipeData is ToolCraftRecipe toolRecipe)
            {
                objectAmount.text = "x"+toolRecipe.toolItemAmount.ToString(); // 도구 아이템의 개수 표시
            }
            else
            {
                objectAmount.text = "1"; // 기본값으로 1로 설정
            }


            //제작 가능한 개수 계산 필요
            maxCount = GetMaxCount(); // 현재 레시피에 따라 제작 가능한 최대 개수 계산
            makeCountText.text = maxCount >= 1
                ? maxCount.ToString() : string.Empty;

            if(maxCount > 0)
            {
                isAvailable = true; // 제작 가능한 아이템
                iconImage.color = Color.white; // 아이콘 색상 변경
            }
            else
            {
                isAvailable = false; // 제작 불가능한 아이템
                iconImage.color = new Color(1f, 1f, 1f, 0.4f); // 아이콘 투명도 조정
            }
            //버튼은 항상 활성화 - 필요한 재료를 보여주기 위함
        }
    }

    //각 아이템의 현재 보유 개수 반환하기
    private int GetItemCountInInventory(ItemData itemData)
    {
        if (inventory == null)
            inventory = GameManager.Instance.uiInventory;

        int total = 0;
        foreach (var slot in inventory.slots)
        {
            if (slot.item == itemData)
            {
                total += slot.quantity;
            }
        }
        return total;
    }

    public int GetMaxCount()
    {
        if(curRecipeData == null)
           return 0; // 레시피가 없으면 제작 불가능
        if(inventory == null)
            inventory = GameManager.Instance.uiInventory; // UIInventory가 할당되지 않았다면 GameManager에서 가져옴

     

        // 현재 레시피의 requiredItems와 requiredItemAmounts를 사용하여 제작 가능한 최대 개수를 계산
        // 가령 돌이 2개 나무가 1개 필요하고 돌 5개 나무가 3개 있다면, (가진 아이템 수/필요개수)를 각각 계산
        // 가장 작은 값이 제작 가능한 개수 (5/2 = 2, 3/1 =3) 이므로 2개가 제작 가능
        // 현재 인벤토리에서 아이템 개수를 가져와서 계산해야함

        int maxCount = int.MaxValue; // 제작 가능한 최대 개수를 무한대로 초기화

        // 0번부터 레시피 데이터의 requiredItems와 requiredItemAmounts를 순회하며 제작 가능한 개수를 계산
        for (int i = 0; i < curRecipeData.requiredItems.Length; i++)
        {
            ItemData requiredItem = curRecipeData.requiredItems[i]; //레시피에서 필요한 아이템의 데이터
            int requiredAmount = curRecipeData.requiredItemAmounts[i];//레시피에서 필요한 아이템의 개수

            // 인벤토리에서 해당 아이템의 개수를 가져옴
            int availableCount = GetItemCountInInventory(requiredItem); //여기서 필요한 아이템의 현재 개수만 가져옴
            // 제작 가능한 개수 계산
            if (availableCount < requiredAmount)
                return 0; // 하나라도 필요한 재료가 부족하면 제작 불가능

            //  0이 아닌 경우(= 제작 가능한 경우) 제작 가능한 개수를 계산
            int possibleCount = availableCount / requiredAmount;

            // 현재까지 계산된 최대 개수와 비교하여 최소값을 유지
            if (possibleCount < maxCount)
                maxCount = possibleCount;
        }
        return maxCount;

    }

    public void OnClickButton()
    {
        // SelectRecipe를 호출하여 현재 레시피를 선택.
        // 이 버튼을 누르면 CraftingSystem에서 resourceSlotPrefab에 ResourceSlot을 생성
        // 1회 제작에 필요한 재료와 아이템 개수를 표시
        if (curRecipeData != null)
        {
            CraftingSystem craftingSystem = FindObjectOfType<CraftingSystem>();
            if (craftingSystem != null)
            {
                craftingSystem.SelectRecipe(curRecipeData);
            }
        }
    }
}
