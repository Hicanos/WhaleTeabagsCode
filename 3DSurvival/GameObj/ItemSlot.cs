using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public ItemData item;

    public UIInventory inventory;

    public Button button;
    public Image itemicon;
    public TextMeshProUGUI quatityText;
    private Outline outline;

    public int index; // 몇 번째 slot인지 index 할당
    public bool equipped; // 장착 여부
    public int quantity; // 수량 데이터

    private void Awake()
    {
        button.onClick.AddListener(OnClickButton);
        outline = GetComponent<Outline>();
    }

    private void OnEnable()
    {
        if (outline != null)
            outline.enabled = equipped;
    }

    // UI 업데이트 함수 - 아이템 데이터에서 필요한 정보를 각 UI에 표시
    public void Set()
    {
        itemicon.gameObject.SetActive(true);
        itemicon.sprite = item.itemIcon;
        quatityText.text = quantity > 1 ? quantity.ToString() : string.Empty;

        if (outline != null)
        {
            outline.enabled = equipped;
        }
    }

    // UI에 정보가 없을 때 UI를 비움
    public void Clear()
    {
        item = null;
        itemicon.gameObject.SetActive(false);
        quatityText.text = string.Empty;
        quantity = 0;
        equipped = false;

        if (outline != null)
        {
            outline.enabled = false;
        }
    }

    // 슬롯을 클릭했을 때
    public void OnClickButton()
    {
        if (inventory != null)
            inventory.SelectItem(index);
    }
}
