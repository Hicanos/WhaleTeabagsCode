using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 아이템 획득 및 게임 종료 시 데이터가 저장되는지 확인하기 위한 디버그 툴
/// </summary>
public class ItemDebugTool : EditorWindow
{
    private Vector2 scrollPos;
    private int addAmount = 1;

    [MenuItem("Tools/Item Debug Tool")]
    public static void ShowWindow()
    {
        GetWindow<ItemDebugTool>("Item Debug Tool");
    }

    private void OnGUI()
    {
        GUILayout.Label("모든 아이템 리스트", EditorStyles.boldLabel);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        // 모든 아이템 리스트
        Dictionary<string, ItemSO> allItems = GetAllItems();
        if (allItems != null)
        {
            foreach (var kvp in allItems)
            {
                var itemSO = kvp.Value;
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label($"{itemSO.ItemID} | {itemSO.NameKr}", GUILayout.Width(200));
                GUILayout.Label(itemSO.Description, GUILayout.Width(250));
                addAmount = EditorGUILayout.IntField(addAmount, GUILayout.Width(40));
                if (GUILayout.Button("획득", GUILayout.Width(60)))
                {
                    ItemManager.Instance.GetItem(itemSO.ItemID, addAmount);
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        else
        {
            GUILayout.Label("아이템 데이터가 로드되지 않았습니다.");
        }
        EditorGUILayout.EndScrollView();

        GUILayout.Space(20);
        GUILayout.Label("보유 아이템 리스트", EditorStyles.boldLabel);
        Dictionary<string, int> ownedItems = GetOwnedItems();
        if (ownedItems != null)
        {
            foreach (var kvp in ownedItems)
            {
                var itemSO = ItemManager.Instance.GetItemSO(kvp.Key);
                string name = itemSO != null ? itemSO.NameKr : kvp.Key;
                GUILayout.Label($"{kvp.Key} | {name} : {kvp.Value}개");
            }
        }
        else
        {
            GUILayout.Label("보유 아이템 데이터가 없습니다.");
        }
    }

    // ItemManager의 모든 아이템 반환
    private Dictionary<string, ItemSO> GetAllItems()
    {
        // ItemManager의 private allItems에 접근할 수 없으므로, 리플렉션 사용
        var manager = ItemManager.Instance;
        var field = typeof(ItemManager).GetField("allItems", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return field?.GetValue(manager) as Dictionary<string, ItemSO>;
    }

    // ItemManager의 보유 아이템 반환
    private Dictionary<string, int> GetOwnedItems()
    {
        var manager = ItemManager.Instance;
        var field = typeof(ItemManager).GetField("ownedItems", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return field?.GetValue(manager) as Dictionary<string, int>;
    }
}
