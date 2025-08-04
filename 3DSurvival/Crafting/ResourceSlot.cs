using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceSlot : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI makeCountText; // 제작에 필요한 개수 표시용 텍스트

    // 재료 슬롯은 이미 저장되어있는 상수를 그대로 사용하여 아이콘과 개수표시
    public void SetItem(ItemData data, int count)
    {
        Debug.LogWarning($"{data.itemIcon.name} 아이콘 설정됨, 개수: {count}"); // 아이콘 설정 확인용 로그
        iconImage.sprite = data.itemIcon;
        
        iconImage.gameObject.SetActive(true); // 아이콘 활성화
        makeCountText.text = count.ToString(); // 제작에 필요한 재료 개수 표시, 필요한 아이템만 인스턴스됨


        // 만약 가지고 있는 재료가 필요로 하는 재료 수보다 적다면, text는 붉은색으로 icon은 0.4f 투명도
        // 색상은 0, 224/255f, 1f, 1f -> 재료 개수 확인 하는 곳에서 작성할 것
    }

}
