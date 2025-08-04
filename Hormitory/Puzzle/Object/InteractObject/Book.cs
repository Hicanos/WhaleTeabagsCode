using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Book : MonoBehaviour, IInteractable
{
    bool isInteracted = false;
    private ItemSO itemData;
    [SerializeField] protected int RequiredItemNumber; // 상호작용을 위해 필요한 아이템 번호
    public void Interact()
    {
        if (RequiredItemNumber > 0)
        {
            itemData = ItemManager.instance.Getitem(RequiredItemNumber);
            if (itemData == null || isInteracted == false)
            {
                isInteracted = true;
                UiManager.Instance.Get<TalkUi>().Popup("책: 답은 의외로 가까운 곳에 있다.");
                SoundManager.PlaySfx(SoundCategory.Interaction, "Rustle");
                // 아이템을 소비하지는 않음. 보유 여부만 확인.
            }
            else if (itemData != null && isInteracted == true)
            {
                UiManager.Instance.Get<TalkUi>().Popup("쪽지: 하나, 다섯, 여덟, 일곱.");
                SoundManager.PlaySfx(SoundCategory.Interaction, "Rustle");
            }
        }
    }
}
