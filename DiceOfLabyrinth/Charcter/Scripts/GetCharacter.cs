using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GetCharacter : MonoBehaviour
{
    //확률에 따라 캐릭터 혹은 아이템을 획득
    //이미 보유한 캐릭터를 획득할 경우, 돌파석(Ascension Stone) 획득
    //아이템은 ItemManager, 캐릭터는 CharacterManager에서 관리
    // SSR 2% SR 18% R 80% 확률로 획득
    // SSR : 현재는 모든 캐릭터들이 SSR등급, 및 상급 포션/상급 스킬북이 SSR등급
    // 포션/스킬북 획득확률이 캐릭터 획득 확률의 2배 (예: SSR 캐릭터 획득 확률 2% -> 상급 포션/스킬북 획득 확률 4%)
    // SR : 스킬북 중급/포션 중급
    // R : 스킬북 하급/포션 하급
    // 아이템 개수는 5~10개 사이로 랜덤 획득

    int jewelAmount = 150; // 1회 소환에 필요한 보석 수

    public void GetCharacters()
    {
        CharacterManager.Instance.AcquireDefaultCharacters();
    }


    // SSR ID, SR ID, R ID 배열

    // SSR 캐릭터 ID 리스트 (char_0~char_4)
    [Header("소환 등급 별 ID")]
    [SerializeField] private List<string> SSRCharacterIDs = new List<string>
    {
        "Char_0", "Char_1", "Char_2", "Char_3", "Char_4"
    };

    [SerializeField]
    private List<string> SSRItemIds = new List<string>
    {
        "Item_3", "Item_7"
    };
    // SSR 아이템 ID 리스트
    [SerializeField] private List<string> SRCharacterIDs; // SR 캐릭터 ID 리스트, 현재는 Null
    [SerializeField]
    private List<string> SRItemIds = new List<string>
    {
        "Item_2","Item_6"
    }; // SR 아이템 ID 리스트
    [SerializeField] private List<string> RCharacterIDs; // R 캐릭터 ID 리스트, 현재 Null
    [SerializeField] private List<string> RItemIds = new List<string>
    {
        "Item_1","Item_5"
    }; // R 아이템 ID 리스트


    // 소환 버튼
    // 단차
    public void GatchaSingle()
    {
        // 1회 소환 로직+ UI 업데이트도 포함
        UserDataManager.Instance.UseJewel(jewelAmount);
        Gatcha();
    }

    // 10연차
    public void GatchaTen()
    {
        // 10연차 소환 로직 구현 + UI 업데이트도 포함
        UserDataManager.Instance.UseJewel(jewelAmount * 10);
        int count = 0; 
        for (int i = 0; i < 10; i++)
        {
            count++;
            Gatcha();
        }

    }

    // 확률 계산 함수 - Random.Range를 사용하여 확률에 따라 캐릭터 또는 아이템 획득
    public void Gatcha()
    {
        // 0~100 사이의 랜덤 값 생성
        float randomValue = Random.Range(0f, 100f);
        Debug.Log($"뽑기 확률: {randomValue}");

        // 확률에 따라 캐릭터 또는 아이템 획득
        if (randomValue < 2) // SSR 캐릭터 획득 확률 2%
        {                        
            Debug.Log("SSR! 보상 획득");
            //여기서도 확률로 SSR or SSR 아이템
            // 1.25:1 (캐릭터 1.25, 아이템 1)
            float randomItemValue = Random.Range(0f, 2.25f);

            if(randomItemValue < 1.25f)
            {
                Debug.Log("SSR 캐릭터 획득!");
                // SSR 캐릭터 5명 중 1개 획득
                string randomCharID = SSRCharacterIDs[Random.Range(0, SSRCharacterIDs.Count)];
                ResultCharacters(randomCharID);

            }
            else
            {
                Debug.Log("SSR 아이템 획득!");
                // SSR 아이템 2개 중 1개 획득
                string randomItemID = SSRItemIds[Random.Range(0, SSRItemIds.Count)];
                Resulttems(randomItemID, Random.Range(5, 11)); // 5~10개 획득
                Debug.Log($"획득한 SSR 아이템 ID: {randomItemID}");
            }
        }
        else if (randomValue < 20) // SR 캐릭터 획득 확률 18%
        {
            Debug.Log("SR 보상 획득");
            // 현재 SR캐릭터 없음, Item획득
            string randomItemID = SRItemIds[Random.Range(0, SRItemIds.Count)];
            Resulttems(randomItemID, Random.Range(5, 11)); // 5~10개 획득
            Debug.Log($"획득한 SR 아이템 ID: {randomItemID}");


        }
        else // R 캐릭터 획득 확률 80%
        {
            Debug.Log("R 보상 획득!");
            // R 캐릭터 획득 로직 추가
            // 현재 R 캐릭터 없음, Item획득
            string randomItemID = RItemIds[Random.Range(0, RItemIds.Count)];
            Resulttems(randomItemID, Random.Range(5, 11)); // 5~10개 획득
            Debug.Log($"획득한 R 아이템 ID: {randomItemID}");
        }
    }


    // 캐릭터 획득 함수
    public void ResultCharacters(string CharID)
    {
        // 만약 획득한 캐릭터가 이미 보유한 캐릭터라면 돌파석 획득 - CharacterManager에서 처리함
        if (CharacterManager.Instance.IsLoaded == false)
        {
            Debug.LogError("캐릭터 데이터가 로드되지 않았습니다.");
            return;
        }
        List<LobbyCharacter> ownedCharacters = CharacterManager.Instance.OwnedCharacters;

        // 아래의 함수는 중복된 캐릭터면 돌파석, 아니면 새로운 캐릭터를 획득함
        // CharacterManager의 OwnedCharacters에 CharID가 있는지 확인 후, 동일하면 돌파석 표시를, 없다면 캐릭터를 표시 (GridView에 슬롯 추가)

        //true/false로 중복 캐릭터 확인 후 UI표시에 활용
        bool isDuplicate = DuplicateCharacterCheck(CharID);

        if (isDuplicate)
        {
            // 이미 보유한 캐릭터는 같은 charID를 가진 Ascension Stone 획득
            ItemManager.Instance.GetAscensionStone(CharID);
            Debug.Log($"이미 보유한 캐릭터로 돌파석 획득: {CharID}");
            return;
            
        }
        
        CharacterManager.Instance.AcquireCharacter(CharID);

        Debug.Log($"획득한 SSR 캐릭터 ID: {CharID}");
    }

    private bool DuplicateCharacterCheck(string charID)
    {
        // CharacterManager의 OwnedCharacters에 charID가 있는지 확인
        return CharacterManager.Instance.OwnedCharacters.Any(c => c.CharacterData.charID == charID);
    }

    // 아이템 획득 함수(임시)

    public void Resulttems(string itemID, int count)
    {
        ItemManager.Instance.GetItem(itemID, count);
    }
}
