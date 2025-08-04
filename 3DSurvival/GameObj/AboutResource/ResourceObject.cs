using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceObject : MonoBehaviour
{
    //자원을 생성하는 오브젝트는 게임 플레이 중 랜덤 위치 생성
    //건물이 있는 곳에는 생성되지 않음
    [SerializeField] private ItemData[] itemsToGive; //획득할 총 아이템들 배열
    [SerializeField] private ItemData[] itemsToGatherType; //획득할 GatherType에 따른 아이템들 배열
    [SerializeField] private ItemData[] baseItemsToGive; //기본 아이템들 배열(어떤 도구를 사용해도 획득할 수 있는 아이템들)
    public int quantityPerHit = 1; //한 번의 타격으로 획득할 아이템 수(기본값 1)
    public int capacity; //해당 오브젝트에서 획득할 수 있는 총량
    private int baseCapacity; // 외부에서 설정한 기본값을 저장하는 변수
    [SerializeField] private float respawnTime = 30f; // 자원 오브젝트 재생성 시간(초 단위)
    [SerializeField] private float decayStamina; //아이템을 획득할 때마다 감소하는 스태미나 양(기본값 0)

    //GatherType 설정
    public GatherType gatherType; //해당 리소스가 요구하는 GatherType

    // 자원 오브젝트를 30초 마다 재생성(Test용)
    private void Start()
    {
        itemsToGive = baseItemsToGive; // 기본 아이템들을 획득 가능한 아이템 목록에 추가
        baseCapacity = capacity; // 초기 용량을 저장
    }

    public void Gather(Vector3 hitPoint, Vector3 hitNormal)
    {
        // 장착한 장비의 canGathering이 true일 때 획득 가능한 자원과 아닐 때의 자원 분리

        //만약 장착한 장비의 canGathering이 false라면 획득할 수 없는 아이템이 있음
        // 각 리소스마다 GatherType을 다르게 설정할 수 있고, 그렇게 설정된 GatherType과 현재 장비중인 EquipItem의 getType이 일치할 때만 획득 가능

        //일단 현재는 상관없이 획득한다고 함
        for (int i = 0; i < quantityPerHit; i++)
        {

            ConditionManager.Instance.Condition.DeltaStamina(decayStamina); // 아이템을 획득할 때마다 스태미나 감소
            if (ConditionManager.Instance.curStamina <= 0) break;

            Debug.Log($"스태미나 감소: {decayStamina} ");

            if (capacity <= 0) break; // 용량이 0 이하일 경우 더 이상 획득하지 않음
            // 아이템을 랜덤으로 선택
            ItemData itemToGive = itemsToGive[Random.Range(0, itemsToGive.Length)];

            //아이템 생성
            capacity--; // 획득할 때마다 용량 감소
            Instantiate(itemToGive.dropPrefab, hitPoint+Vector3.up, Quaternion.LookRotation(hitNormal, Vector3.up));
        }


        if(capacity <= 0)
        {            
            gameObject.SetActive(false);
            // 일정 시간 후에 다시 활성화
            ResourceControl.Instance.RequestRespawn(this, respawnTime); // ResourceControl의 RequestRespawn 메서드를 호출하여 재생성 요청
        }

    }

    public void Initialize()
    {
        capacity = baseCapacity; // 초기 용량으로 설정
    }



}
