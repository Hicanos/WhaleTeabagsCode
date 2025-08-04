using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ResourceItem", menuName = "New Item/ResourceItem", order = 1)]
public class ResourceItem : ItemData
{
   
    // 자원 아이템은 제작에서 사용되는 재료로 분류
    // 특별한 기능이 없는 자원 아이템은 ItemType에 Resource만 설정
}
