using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;

/// <summary>
/// 모든 캐릭터 데이터와 유저가 획득한 캐릭터를 관리하는 매니저 (MonoBehaviour 미상속)
/// </summary>
public class CharacterManager
{
    private static CharacterManager instance;
    public static CharacterManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new CharacterManager();
                instance.Initialize();
            }
            return instance;
        }
    }

    public Dictionary<string, CharacterSO> AllCharacters { get; private set; } = new Dictionary<string, CharacterSO>();
    public List<LobbyCharacter> OwnedCharacters { get; private set; } = new List<LobbyCharacter>();
    public Dictionary<string, BattleCharacter> BattleCharacters { get; private set; } = new Dictionary<string, BattleCharacter>();
    private List<AsyncOperationHandle<IList<CharacterSO>>> handles = new List<AsyncOperationHandle<IList<CharacterSO>>>();
    public bool IsLoaded { get; private set; } = false;

    private void Initialize()
    {
        OwnedCharacters.Clear();
        BattleCharacters.Clear();
        AllCharacters.Clear();
        handles.Clear();
    }

    // Addressable로 등록된 캐릭터 SO들을 비동기로 로드
    public async Task LoadAllCharactersAsync()
    {
        handles.Clear();
        var handle = Addressables.LoadAssetsAsync<CharacterSO>("CharacterSO", null);
        handles.Add(handle);
        await handle.Task;
        AllCharacters = handle.Result.ToDictionary(c => c.charID, c => c);
        IsLoaded = true;
        LoadOwnedCharactersFromData();
    }

    public void ReleaseAllCharacters()
    {
        foreach (var handle in handles)
        {
            if (handle.IsValid())
                Addressables.Release(handle);
        }
        AllCharacters.Clear();
        OwnedCharacters.Clear();
        BattleCharacters.Clear();
        handles.Clear();
        IsLoaded = false;
    }

    /// <summary>
    /// 저장 데이터에서 보유 캐릭터 리스트를 LobbyCharacter로 복원
    /// </summary>
    public void LoadOwnedCharactersFromData()
    {
        OwnedCharacters.Clear();
        foreach (var charData in DataSaver.Instance.SaveData.characters)
        {
            if (AllCharacters.TryGetValue(charData.CharacterID, out var so))
            {
                var lobbyChar = new LobbyCharacter();
                lobbyChar.Initialize(so, charData.Level);
                lobbyChar.CurrentExp = charData.CurrentExp;
                lobbyChar.RegularATK = charData.ATK;
                lobbyChar.RegularDEF = charData.DEF;
                lobbyChar.RegularHP = charData.HP;
                lobbyChar.CritChance = charData.CritChance;
                lobbyChar.CritDamage = charData.CritDamage;
                OwnedCharacters.Add(lobbyChar);
            }
        }
#if UNITY_EDITOR
        Debug.Log($"보유 캐릭터 로드됨: {OwnedCharacters.Count}개");
#endif
    }

    /// <summary>
    /// 캐릭터 획득 (중복 방지)
    /// </summary>
    public void AcquireCharacter(string charID)
    {
        
        if (AllCharacters.TryGetValue(charID, out var so))
        {
            var lobbyChar = new LobbyCharacter();
            lobbyChar.Initialize(so, 1);
            OwnedCharacters.Add(lobbyChar);            
            DataSaver.Instance.SaveAllCharacters(OwnedCharacters);
        }
    }

    /// <summary>
    /// 보유 캐릭터 삭제
    /// </summary>
    public void RemoveCharacter(string charID)
    {
        var lobbyChar = OwnedCharacters.FirstOrDefault(c => c.CharacterData.charID == charID);
        if (lobbyChar != null)
        {
            OwnedCharacters.Remove(lobbyChar);
            DataSaver.Instance.SaveAllCharacters(OwnedCharacters);
        }
    }

    /// <summary>
    /// 전체 보유 캐릭터 저장
    /// </summary>
    public void SaveAll()
    {
        DataSaver.Instance.SaveAllCharacters(OwnedCharacters);
    }

    /// <summary>
    /// LobbyCharacter를 CharacterSO로 검색
    /// </summary>
    public LobbyCharacter GetLobbyCharacterBySO(CharacterSO so)
    {
        return OwnedCharacters.FirstOrDefault(lc => lc.CharacterData == so);
    }

    /// <summary>
    /// LobbyCharacter를 charID로 검색
    /// </summary>
    public LobbyCharacter GetLobbyCharacterByID(string charID)
    {
        return OwnedCharacters.FirstOrDefault(lc => lc.CharacterData != null && lc.CharacterData.charID == charID);
    }

    /// <summary>
    /// BattleCharacter 데이터만 등록 (프리팹 생성 X)
    /// 이미 등록되어 있으면 지우고 새로 생성
    /// </summary>
    public BattleCharacter RegisterBattleCharacterData(string charID)
    {
        // 이미 등록된 BattleCharacter가 있으면 지우고 새로 생성
        if (BattleCharacters.TryGetValue(charID, out var battleChar))
        {
            BattleCharacters.Remove(charID);
        }

        // 캐릭터 SO가 존재하는지 확인
        if (!AllCharacters.TryGetValue(charID, out var so))
            throw new System.Exception($"존재하지 않는 캐릭터 ID: {charID}");
        
        // BattleCharacter를 so를 기반으로 생성 (내부에서 LobbyCharacter까지 읽어옴)
        var battleData = new BattleCharacter(so);
        BattleCharacters[charID] = battleData;
        return battleData;
    }

    /// <summary>
    /// BattleCharacter 데이터 제거 (프리팹 오브젝트는 따로 관리)
    /// </summary>
    public void UnregisterBattleCharacterData(string charID)
    {
        if (BattleCharacters.ContainsKey(charID))
            BattleCharacters.Remove(charID);
    }

    /// <summary>
    /// BattleCharacter 데이터 조회
    /// </summary>
    public BattleCharacter GetBattleCharacterData(string charID)
    {
        BattleCharacters.TryGetValue(charID, out var battleChar);
        return battleChar;
    }

    /// <summary>  
    /// 디폴트 캐릭터(Char_0~ Char_4) 획득
    /// </summary>
    
    public async void AcquireDefaultCharacters()
    {
        // Addressables가 로드될 때까지 대기
        const int MaxWaitFrames = 200; // 최대 대기 프레임(약 4초)
        const int WaitFrameMillis = 20; // 프레임당 대기(ms)
        int waitCount = 0;
        while (!IsLoaded && waitCount < MaxWaitFrames)
        {
            await Task.Delay(WaitFrameMillis);
            waitCount++;
        }
        if (!IsLoaded)
        {
            Debug.LogError("캐릭터 SO가 로드되지 않아 기본 캐릭터를 획득할 수 없습니다.");
            return;
        }

        // 데이터 세이버에 캐릭터 정보가 없으면(최초 실행) 강제로 추가
        if (DataSaver.Instance.SaveData.characters == null || DataSaver.Instance.SaveData.characters.Count == 0)
        {
            for (int i = 0; i < 5; i++)
            {
                string charID = "Char_" + i.ToString();
                AcquireCharacter(charID);
                Debug.Log($"최초 실행: 기본 캐릭터 강제 획득: {charID}");
            }
            return;
        }

        // 기존 데이터에 없는 캐릭터만 추가
        for (int i = 0; i < 5; i++)
        {
            string charID = "Char_" + i.ToString();
            bool hasInOwned = OwnedCharacters.Any(c => c.CharacterData != null && c.CharacterData.charID == charID);
            bool hasInSaveData = DataSaver.Instance.SaveData.characters.Any(c => c.CharacterID == charID);
            if (!hasInOwned || !hasInSaveData)
            {
                AcquireCharacter(charID);
                Debug.Log($"기존 데이터에 없어서 기본 캐릭터 획득: {charID}");
            }
        }
    }

}
