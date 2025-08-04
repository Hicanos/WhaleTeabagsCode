using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <summary>
/// 아이템 데이터를 관리하는 매니저 클래스
/// 유저의 보유 아이템을 저장하고, DataSaver를 통해 데이터를 저장, 불러오는 역할
/// </summary>

public class ItemManager
{
    // 싱글톤 인스턴스

    private static ItemManager _instance;
    public static ItemManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ItemManager();
                _instance.Initialize();
            }
            return _instance;
        }
    }

    // 보유 중인 아이템과, 해당 아이템의 개수를 저장하는 딕셔너리
    private Dictionary<string, int> ownedItems;
    public Dictionary<string, int> OwnedItems => ownedItems;

    // Addressable로 로드된 모든 아이템 SO
    private Dictionary<string, ItemSO> allItems = new Dictionary<string, ItemSO>();
    public Dictionary<string, ItemSO> AllItems => allItems;
    // Addressables 핸들 캐싱 (릴리즈용)
    private List<AsyncOperationHandle<IList<ItemSO>>> handles = new List<AsyncOperationHandle<IList<ItemSO>>>();

    private bool isLoaded = false;

    //이니셜라이저
    private void Initialize()
    {
        ownedItems = new Dictionary<string, int>();
        allItems.Clear();
        handles.Clear();
#if UNITY_EDITOR
        Debug.Log("ItemManager 이니셜라이즈");
#endif
    }

    // Addressable에서 모든 아이템 SO 비동기 로드, 아이템은 ItemSO 라벨
    public async Task LoadAllItemSOs()
    {
        allItems.Clear();
        handles.Clear();
        var handle = Addressables.LoadAssetsAsync<ItemSO>("ItemSO", OnItemSOLoaded);
        handles.Add(handle);
        await handle.Task;
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            isLoaded = true;
            LoadOwnedItemsFromData();
#if UNITY_EDITOR
            Debug.Log($"모든 아이템 로드됨. 보유 아이템 로드 시작 Count: {allItems.Count}");
#endif
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogError("Failed to load ItemSOs.");
#endif
        }
    }

    // 각 SO가 로드될 때마다 딕셔너리에 저장
    private void OnItemSOLoaded(ItemSO itemSO)
    {
        if (itemSO != null && !allItems.ContainsKey(itemSO.ItemID))
            allItems.Add(itemSO.ItemID, itemSO);
    }

    // Json 파일로부터 보유 중인 아이템을 불러오는 메서드
    public void LoadOwnedItemsFromData()
    {
        ownedItems.Clear();
        foreach (var itemData in DataSaver.Instance.SaveData.items)
        {
            if (allItems.ContainsKey(itemData.ItemID))
            {
                ownedItems[itemData.ItemID] = itemData.Quantity;
#if UNITY_EDITOR
                Debug.Log($"보유 아이템 로드됨: \n{itemData.ItemID}\n아이템 이름: {allItems[itemData.ItemID].NameKr}\n개수: {itemData.Quantity}");
#endif
            }
        }
    }

    // 아이템ID 유효성 검사
    private bool IsValidItemID(string itemID)
    {
        if (string.IsNullOrEmpty(itemID))
            return false;
        return allItems.ContainsKey(itemID);
    }

    // 아이템 획득 메서드
    public void GetItem(string ItemID, int Count = 1)
    {
        if (!IsValidItemID(ItemID))
        {
            return;
        }
        if (ownedItems.ContainsKey(ItemID))
        {
            ownedItems[ItemID] += Count;
        }
        else
        {
            ownedItems.Add(ItemID, Count);
        }
#if UNITY_EDITOR
        Debug.Log($"아이템 획득: {ItemID}, 추가 개수: {Count}, 총 개수: {ownedItems[ItemID]}");
#endif
    }
    /// <summary>
    /// 아이템의 ID를 통해 해당 아이템SO를 반환하는 메서드
    /// </summary>
    /// <param name="itemID"></param>

    public ItemSO GetItemSO(string itemID)
    {    
        if (!IsValidItemID(itemID))
        {
            return null;
        }
        if (allItems.TryGetValue(itemID, out ItemSO itemSO))
        {
            return itemSO;
        }
        else
        {
            return null;
        }
    }

    // 가챠에서 중복 캐릭터를 얻었을 때 charID를 통해 돌파석(AscensionStone) 획득
    public void GetAscensionStone(string charID, int count = 1)
    {
        string itemID = null;
        foreach (var item in allItems.Values)
        {
            if (item is AscensionStone ascensionStone && ascensionStone.CharID == charID)
            {
                itemID = item.ItemID;
                break;
            }
        }

        if (!IsValidItemID(itemID))
        {
            return;
        }
        GetItem(itemID, count);
    }

    // 하위는 각 아이템 종류별로 보유 중인 아이템 반환(인벤토리 필터 등)

    public Dictionary<string, int> GetPotions()
    {
        Dictionary<string, int> potions = new Dictionary<string, int>();
        foreach (var item in ownedItems)
        {
            ItemSO itemSO = GetItemSO(item.Key);
            if (itemSO is EXPpotion expPotion)
            {
                potions.Add(item.Key, item.Value);
            }
        }
        return potions;
    }

    public Dictionary<string, int> GetSkillBooks()
    {
        Dictionary<string, int> skillBooks = new Dictionary<string, int>();
        foreach (var item in ownedItems)
        {
            ItemSO itemSO = GetItemSO(item.Key);
            if (itemSO is SkillBook skillBook)
            {
                skillBooks.Add(item.Key, item.Value);
            }
        }
        return skillBooks;
    }

    public Dictionary<string, int> GetAscensionStones()
    {
        Dictionary<string, int> ascensionStones = new Dictionary<string, int>();
        foreach (var item in ownedItems)
        {
            ItemSO itemSO = GetItemSO(item.Key);
            if (itemSO is AscensionStone ascensionStone)
            {
                ascensionStones.Add(item.Key, item.Value);
            }
        }
        return ascensionStones;
    }

    // Addressables 릴리즈 메서드 추가
    public void ReleaseAllItems()
    {
        foreach (var handle in handles)
        {
            if (handle.IsValid())
                Addressables.Release(handle);
        }
        allItems.Clear();
        handles.Clear();
        isLoaded = false;
    }
}
