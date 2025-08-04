using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

// <summary>
/// 모든 게임데이터를 저장하는 역할의 매니저
/// json 파일로 저장함, 불러오는 건 Loader가 담당
/// </summary>
public class DataSaver
{
    public static DataSaver Instance { get; private set; }
    static DataSaver()
    {
        Instance = new DataSaver();
        Debug.Log("DataSaver 인스턴스 생성됨");
    }

    [Serializable]
    public class UserData
    {
        public string NickName;
        public int Level;
        public int Exp;
        public int Gold;
        public int Jewel;
        public int currentStamina;
        public DateTime LastQuitTime;

        // 생성자
        public UserData() { }
        public UserData(string nickName, int level, int exp, int gold, int jewel, int currentStamina)
        {
            NickName = nickName;
            Level = level;
            Exp = exp;
            Gold = gold;
            Jewel = jewel;
            this.currentStamina = currentStamina;
            LastQuitTime = DateTime.Now; // 현재 시간으로 초기화
        }
    }

    public class TutorialData 
    {
        // 튜토리얼 클리어 여부
        public bool isLobbyTutorialCompleted = false;
        public bool isGameTutorialCompleted = false;
        // TutorialManager의 값을 저장/로드
    }

    [Serializable]
    public class OptionData
    {
        public float masterVolume = 1f;
        public float bgmVolume = 1f;
        public float sfxVolume = 1f;

        // 생성자
        public OptionData() { }
        public OptionData(float masterVolume, float bgmVolume, float sfxVolume)
        {
            this.masterVolume = masterVolume;
            this.bgmVolume = bgmVolume;
            this.sfxVolume = sfxVolume;
        }
    }

    [Serializable]
    public class CharacterData
    {
        // 플레이어가 보유한 캐릭터 정보와 그 캐릭터의 레벨 정보(스킬, 주사위 포함)
        // 고유 식별자 : CharacterSO에서 캐릭터 정보를 가져올 수 있음
        public string CharacterID; // 캐릭터 ID
        public int Level; // 캐릭터 레벨
        public int CurrentExp; // 현재 경험치

        // 캐릭터의 능력치
        public int ATK; // 공격력
        public int DEF; // 방어력
        public int HP; // 체력
        public float CritChance; // 치명타 확률
        public float CritDamage; // 치명타 피해량
        // 캐릭터의 스킬 정보 - SkillData 리스트로 저장
        // public List<SkillData> Skills = new List<SkillData>(); // 각 캐릭터가 보유한 스킬 정보

        // 생성자
        public CharacterData() { }
        public CharacterData(string characterID, int level, int atk, int def, int hp, float critChance, float critDamage)
        {
            CharacterID = characterID;
            Level = level;
            ATK = atk;
            DEF = def;
            HP = hp;
            CritChance = critChance;
            CritDamage = critDamage;
        }
    }

    [Serializable]
    public class SkillData
    {
        // 캐릭터의 스킬 정보 (스킬 레벨, 효과 등)
        public string SkillID; // 스킬 ID
        public int Level; // 스킬 레벨
        public int Cooldown; // 쿨타임
        public int Power; // 스킬 파워
    }

    [Serializable]
    public class StageData
    {
        // StageSaveData의 모든 필드 복사 (ScriptableObject는 이름/ID만 저장)
        public int currentChapterIndex;
        public int currentStageIndex;
        public int currentPhaseIndex;
        public int normalStageCompleteCount;
        public int eliteStageCompleteCount;
        public int manaStone;
        public int savedExpReward;
        public int savedGoldReward;
        public int savedPotionReward;

        public int currentFormationType;
        public int currentPhaseState;

        // ScriptableObject는 이름/ID만 저장
        public List<string> artifactNames = new List<string>(12);
        public List<string> engravingNames = new List<string>(3);
        public List<string> equipedArtifactNames = new List<string>(4);
        public List<string> entryCharacterIDs = new List<string>(5);
        public string leaderCharacterID;
        public List<BattleCharacterData> battleCharacters = new List<BattleCharacterData>(5);
        public string selectedEnemyID;
        public List<ChapterStates> chapterStates = new List<ChapterStates>();

        // 역직렬화용 기본 생성자
        public StageData() { }

        // 변환 생성자(직렬화/역직렬화에는 사용하지 않음, 오직 코드 내 변환용)
        public StageData(StageSaveData saveData)
        {
            if (saveData == null) return;

            currentChapterIndex = saveData.currentChapterIndex;
            currentStageIndex = saveData.currentStageIndex;
            currentPhaseIndex = saveData.currentPhaseIndex;
            normalStageCompleteCount = saveData.normalStageCompleteCount;
            eliteStageCompleteCount = saveData.eliteStageCompleteCount;
            manaStone = saveData.manaStone;
            savedExpReward = saveData.savedExpReward;
            savedGoldReward = saveData.savedGoldReward;
            savedPotionReward = saveData.savedPotionReward;
            currentFormationType = (int)saveData.currentFormationType;
            currentPhaseState = (int)saveData.currentPhaseState;

            artifactNames = saveData.artifacts?.Select(a => a != null ? a.ArtifactName : null).ToList() ?? new List<string>(new string[12]);
            engravingNames = saveData.engravings?.Select(e => e != null ? e.EngravingName : null).ToList() ?? new List<string>(new string[3]);
            equipedArtifactNames = saveData.equipedArtifacts?.Select(a => a != null ? a.ArtifactName : null).ToList() ?? new List<string>(new string[4]);
            entryCharacterIDs = saveData.entryCharacters?.Select(c => c != null ? c.name : null).ToList() ?? new List<string>(new string[5]);
            leaderCharacterID = saveData.leaderCharacter != null ? saveData.leaderCharacter.name : null;
            battleCharacters = saveData.battleCharacters?.Select(bc => new BattleCharacterData(bc)).ToList() ?? new List<BattleCharacterData>();
            selectedEnemyID = saveData.selectedEnemy != null ? saveData.selectedEnemy.name : null;
            chapterStates = saveData.chapterStates != null ? new List<ChapterStates>(saveData.chapterStates) : new List<ChapterStates>();
        }

        // StageData → StageSaveData 변환 (string → SO 복원)
        public StageSaveData ToStageSaveData()
        {
            var saveData = new StageSaveData();
            saveData.currentChapterIndex = currentChapterIndex;
            saveData.currentStageIndex = currentStageIndex;
            saveData.currentPhaseIndex = currentPhaseIndex;
            saveData.normalStageCompleteCount = normalStageCompleteCount;
            saveData.eliteStageCompleteCount = eliteStageCompleteCount;
            saveData.manaStone = manaStone;
            saveData.savedExpReward = savedExpReward;
            saveData.savedGoldReward = savedGoldReward;
            saveData.savedPotionReward = savedPotionReward;
            saveData.currentFormationType = (StageSaveData.CurrentFormationType)currentFormationType;
            saveData.currentPhaseState = (StageSaveData.CurrentPhaseState)currentPhaseState;

            // StaticDataManager를 통해 이름 기반 SO 복원 (null/빈 문자열 안전 처리)
            saveData.artifacts = artifactNames
                .Select(name => string.IsNullOrEmpty(name) ? null : StaticDataManager.Instance.GetArtifact(name))
                .ToList();
            saveData.engravings = engravingNames
                .Select(name => string.IsNullOrEmpty(name) ? null : StaticDataManager.Instance.GetEngraving(name))
                .ToList();
            saveData.equipedArtifacts = equipedArtifactNames
                .Select(name => string.IsNullOrEmpty(name) ? null : StaticDataManager.Instance.GetArtifact(name))
                .ToList();

            // entryCharacters 복원 (CharacterSO)
            saveData.entryCharacters = entryCharacterIDs
                .Select(id => !string.IsNullOrEmpty(id) && CharacterManager.Instance.AllCharacters.ContainsKey(id)
                    ? CharacterManager.Instance.AllCharacters[id]
                    : null)
                .ToList();

            // leaderCharacter 복원 (CharacterSO)
            saveData.leaderCharacter = !string.IsNullOrEmpty(leaderCharacterID) && CharacterManager.Instance.AllCharacters.ContainsKey(leaderCharacterID)
                ? CharacterManager.Instance.AllCharacters[leaderCharacterID]
                : null;

            // battleCharacters 복원 (BattleCharacter)
            saveData.battleCharacters.Clear();
            foreach (var bcData in battleCharacters)
            {
                if (bcData == null || string.IsNullOrEmpty(bcData.charID)) continue;
                var battleChar = CharacterManager.Instance.RegisterBattleCharacterData(bcData.charID);
                battleChar.DataSetting(bcData);
                saveData.battleCharacters.Add(battleChar);
            }

            // Enemy SO 복원
            saveData.selectedEnemy = !string.IsNullOrEmpty(selectedEnemyID)
                ? StaticDataManager.Instance.GetEnemy(selectedEnemyID)
                : null;

            saveData.chapterStates = new List<ChapterStates>(chapterStates);
            return saveData;
        }
    }

    public static void CopyStageSaveData(StageSaveData target, StageSaveData source)
    {
        if (target == null || source == null) return;

        target.currentChapterIndex = source.currentChapterIndex;
        target.currentStageIndex = source.currentStageIndex;
        target.currentPhaseIndex = source.currentPhaseIndex;
        target.currentFormationType = source.currentFormationType;
        target.normalStageCompleteCount = source.normalStageCompleteCount;
        target.eliteStageCompleteCount = source.eliteStageCompleteCount;
        target.currentPhaseState = source.currentPhaseState;
        target.manaStone = source.manaStone;
        target.savedExpReward = source.savedExpReward;
        target.savedGoldReward = source.savedGoldReward;
        target.savedPotionReward = source.savedPotionReward;

        while (target.artifacts.Count < source.artifacts.Count)
            target.artifacts.Add(null);
        while (target.artifacts.Count > source.artifacts.Count)
            target.artifacts.RemoveAt(target.artifacts.Count - 1);
        for (int i = 0; i < source.artifacts.Count; i++)
            target.artifacts[i] = source.artifacts[i];

        while (target.engravings.Count < source.engravings.Count)
            target.engravings.Add(null);
        while (target.engravings.Count > source.engravings.Count)
            target.engravings.RemoveAt(target.engravings.Count - 1);
        for (int i = 0; i < source.engravings.Count; i++)
            target.engravings[i] = source.engravings[i];

        while (target.equipedArtifacts.Count < source.equipedArtifacts.Count)
            target.equipedArtifacts.Add(null);
        while (target.equipedArtifacts.Count > source.equipedArtifacts.Count)
            target.equipedArtifacts.RemoveAt(target.equipedArtifacts.Count - 1);
        for (int i = 0; i < source.equipedArtifacts.Count; i++)
            target.equipedArtifacts[i] = source.equipedArtifacts[i];

        while (target.entryCharacters.Count < source.entryCharacters.Count)
            target.entryCharacters.Add(null);
        while (target.entryCharacters.Count > source.entryCharacters.Count)
            target.entryCharacters.RemoveAt(target.entryCharacters.Count - 1);
        for (int i = 0; i < source.entryCharacters.Count; i++)
            target.entryCharacters[i] = source.entryCharacters[i];

        target.leaderCharacter = source.leaderCharacter;

        while (target.battleCharacters.Count < source.battleCharacters.Count)
            target.battleCharacters.Add(null);
        while (target.battleCharacters.Count > source.battleCharacters.Count)
            target.battleCharacters.RemoveAt(target.battleCharacters.Count - 1);
        for (int i = 0; i < source.battleCharacters.Count; i++)
            target.battleCharacters[i] = source.battleCharacters[i];

        target.selectedEnemy = source.selectedEnemy;

        while (target.chapterStates.Count < source.chapterStates.Count)
            target.chapterStates.Add(new ChapterStates());
        while (target.chapterStates.Count > source.chapterStates.Count)
            target.chapterStates.RemoveAt(target.chapterStates.Count - 1);
        for (int i = 0; i < source.chapterStates.Count; i++)
            target.chapterStates[i] = source.chapterStates[i];
    }

    [Serializable]
    public class BattleCharacterData
    {
        public string charID;
        public int level;
        public int currentHP; // 현재 체력
        public int currentATK; // 현재 공격력
        public int currentDEF; // 현재 방어력
        public float currentCritChance; // 현재 치명타 확률
        public float currentCritDamage; // 현재 치명타 피해량
        public float currentPenetration; // 현재 관통력
        public int regularHP;
        public int regularATK;
        public int regularDEF;
        public float regularCritChance;
        public float regularCritDamage;
        public float regularPenetration;
        public bool IsDied;

        // 역직렬화용 기본 생성자
        public BattleCharacterData() { }

        // 변환 생성자(직렬화/역직렬화에는 사용하지 않음, 오직 코드 내 변환용)
        public BattleCharacterData(BattleCharacter bc)
        {
            if (bc == null) return;
            charID = bc.CharID;
            level = bc.Level;
            currentHP = bc.CurrentHP;
            currentATK = bc.CurrentATK;
            currentDEF = bc.CurrentDEF;
            currentCritChance = bc.CurrentCritChance;
            currentCritDamage = bc.CurrentCritDamage;
            currentPenetration = bc.CurrentPenetration;

            regularHP = bc.RegularHP;
            regularATK = bc.RegularATK;
            regularDEF = bc.RegularDEF;
            regularCritChance = bc.RegularCritChance;
            regularCritDamage = bc.RegularCritDamage;
            regularPenetration = bc.Penetration;
            IsDied = bc.IsDied;
        }

        // BattleCharacterData를 BattleCharacter에 복원
        public void LoadBattleCharacter(BattleCharacter bc, BattleCharacterData data)
        {
            bc.DataSetting(data);
        }

    }

    [Serializable]
    public class ItemData
    {
        // 플레이어가 획득한 아이템 정보 (스킬 강화 아이템 등)
        // ItemManager의 Dictionary<string, int> ownedItems를 기반으로 저장
        public string ItemID; // 아이템 ID
        public int Quantity; // 아이템 개수

        // 생성자
        public ItemData() { }
        public ItemData(string itemID, int quantity)
        {
            ItemID = itemID;
            Quantity = quantity;
        }
    }

    /// <summary>
    /// userData, chractarData, stageData, itemData를 포함하는 게임 저장 데이터 클래스
    /// </summary>
    [Serializable]
    public class GameSaveData
    {
        public UserData userData = new UserData();
        public TutorialData tutorialData = new TutorialData();
        public OptionData optionData = new OptionData();
        public List<CharacterData> characters = new List<CharacterData>();
        public List<ItemData> items = new List<ItemData>();
        public StageData stageData;
    }

    private static readonly string SavePath = Path.Combine(Application.persistentDataPath, "save.json");
    public GameSaveData SaveData = new GameSaveData();

    /// <summary>
    /// 캐릭터 정보 동기화
    /// </summary>
    public void SyncCharacterData()
    {
        if (CharacterManager.Instance != null)
        {
            var lobbyCharacters = CharacterManager.Instance.OwnedCharacters;
            SaveData.characters = lobbyCharacters.Select(lobbyChar => new CharacterData
            {
                CharacterID = lobbyChar.CharacterData.charID,
                Level = lobbyChar.Level,
                CurrentExp = lobbyChar.CurrentExp,
                ATK = lobbyChar.RegularATK,
                DEF = lobbyChar.RegularDEF,
                HP = lobbyChar.RegularHP,
                CritChance = lobbyChar.CritChance,
                CritDamage = lobbyChar.CritDamage
            }).ToList();
        }
    }

    /// <summary>
    /// 아이템 정보 동기화
    /// </summary>
    public void SyncItemData()
    {
        if (ItemManager.Instance != null)
        {
            var items = ItemManager.Instance.OwnedItems;
            SaveData.items = items.Select(item => new ItemData(item.Key, item.Value)).ToList();
        }
    }

    private void SyncSoundOptions()
    {
        if (SoundManager.Instance != null)
        {
            SaveData.optionData.masterVolume = SoundManager.Instance.masterVolume;
            SaveData.optionData.bgmVolume = SoundManager.Instance.bgmVolume;
            SaveData.optionData.sfxVolume = SoundManager.Instance.sfxVolume;
        }
    }

    private void SyncTutorialCompletion()
    {
        if (TutorialManager.Instance != null)
        {
            SaveData.tutorialData.isLobbyTutorialCompleted = TutorialManager.Instance.isLobbyTutorialCompleted;
            SaveData.tutorialData.isGameTutorialCompleted = TutorialManager.Instance.isGameTutorialCompleted;
        }
    }

    /// <summary>
    /// UserData 정보 동기화 (UserDataManager → SaveData)
    /// </summary>
    public void SyncUserData()
    {
        if (UserDataManager.Instance != null)
        {
            SaveData.userData.NickName = UserDataManager.Instance.nickname;
            SaveData.userData.Level = UserDataManager.Instance.level;
            SaveData.userData.Exp = UserDataManager.Instance.exp;
            SaveData.userData.Gold = UserDataManager.Instance.gold;
            SaveData.userData.Jewel = UserDataManager.Instance.jewel;
            SaveData.userData.currentStamina = UserDataManager.Instance.currentStamina;
            SaveData.userData.LastQuitTime = DateTime.Now; // 저장 시점의 시간으로 저장
        }
    }

    /// <summary>
    /// 저장된 UserData를 UserDataManager에 적용
    /// </summary>
    public void ApplyUserData()
    {
        if (UserDataManager.Instance != null && SaveData.userData != null)
        {
            UserDataManager.Instance.nickname = SaveData.userData.NickName;
            UserDataManager.Instance.level = SaveData.userData.Level;
            UserDataManager.Instance.exp = SaveData.userData.Exp;
            UserDataManager.Instance.gold = SaveData.userData.Gold;
            UserDataManager.Instance.jewel = SaveData.userData.Jewel;
            UserDataManager.Instance.currentStamina = SaveData.userData.currentStamina;
            UserDataManager.Instance.lastQuit = SaveData.userData.LastQuitTime; // 저장된 종료 시점 적용
        }
    }

    /// <summary>
    /// 게임 데이터 저장(json 파일)
    /// </summary>
    public void Save()
    {
        try
        {
            SyncUserData();
            SyncCharacterData();
            SyncItemData();
            SyncTutorialCompletion();
            SyncSoundOptions();
            // StageSaveData → StageData 변환 및 저장
            if (StageManager.Instance != null && StageManager.Instance.stageSaveData != null)
                SaveData.stageData = new StageData(StageManager.Instance.stageSaveData);

            string json = JsonConvert.SerializeObject(SaveData, Formatting.Indented);
            File.WriteAllText(SavePath, json);
#if UNITY_EDITOR
            Debug.Log($"게임 데이터 저장됨: {SavePath}");
#endif
        }
        catch (Exception ex)
        {
            Debug.LogError($"게임 데이터 저장 실패: {ex.Message}");
        }
    }

    /// <summary>
    /// 개별 캐릭터 저장(추가/갱신)
    /// </summary>
    public void SaveCharacter(LobbyCharacter lobbyChar)
    {
        var charData = new CharacterData
        {
            CharacterID = lobbyChar.CharacterData.charID,
            Level = lobbyChar.Level,
            CurrentExp = lobbyChar.CurrentExp,
            ATK = lobbyChar.RegularATK,
            DEF = lobbyChar.RegularDEF,
            HP = lobbyChar.RegularHP,
            CritChance = lobbyChar.CritChance,
            CritDamage = lobbyChar.CritDamage
        };

        int idx = SaveData.characters.FindIndex(c => c.CharacterID == charData.CharacterID);
        if (idx >= 0)
            SaveData.characters[idx] = charData;
        else
            SaveData.characters.Add(charData);

        Save();
    }

    /// <summary>
    /// 전체 캐릭터 저장
    /// </summary>
    public void SaveAllCharacters(List<LobbyCharacter> lobbyCharacters)
    {
        SaveData.characters = lobbyCharacters.Select(lobbyChar => new CharacterData
        {
            CharacterID = lobbyChar.CharacterData.charID,
            Level = lobbyChar.Level,
            CurrentExp = lobbyChar.CurrentExp,
            ATK = lobbyChar.RegularATK,
            DEF = lobbyChar.RegularDEF,
            HP = lobbyChar.RegularHP,
            CritChance = lobbyChar.CritChance,
            CritDamage = lobbyChar.CritDamage
        }).ToList();

        Save();
    }

    public void SaveItems(Dictionary<string, int> items)
    {
        SaveData.items = items.Select(item => new ItemData(item.Key, item.Value)).ToList();
        Save();
    }

    public void Load()
    {
        string json = "";
        try
        {
            if (File.Exists(SavePath))
            {
                json = File.ReadAllText(SavePath);
                SaveData = JsonConvert.DeserializeObject<GameSaveData>(json);
                EnsureStageDataIntegrity();
                Debug.Log($"stageData null? {SaveData.stageData == null}");
                Debug.Log($"artifactNames null? {SaveData.stageData.artifactNames == null}");
                Debug.Log($"engravingNames null? {SaveData.stageData.engravingNames == null}");
                Debug.Log($"chapterStates null? {SaveData.stageData.chapterStates == null}");
                // SO 복원은 GameManager에서 Addressables 로드 완료 후 호출

                ApplyUserData();
                ApplyTutorialCompletion();
                ApplySoundOptions();
            }
            else
            {
                SaveData = new GameSaveData();
                Save();
                Debug.Log("저장 파일이 없어 새 데이터로 초기화");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"게임 데이터 불러오기 실패: {ex.Message}\n{ex.StackTrace}\njson: {json}");
            SaveData = new GameSaveData();
            Save();
        }
        // SO 복원은 GameManager에서 Addressables 로드 완료 후 호출
    }

    private void ApplyTutorialCompletion()
    {
        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.isLobbyTutorialCompleted = SaveData.tutorialData.isLobbyTutorialCompleted;
            TutorialManager.Instance.isGameTutorialCompleted = SaveData.tutorialData.isGameTutorialCompleted;
            Debug.Log($"튜토리얼 완료 여부 로드됨: Lobby={TutorialManager.Instance.isLobbyTutorialCompleted}, Game={TutorialManager.Instance.isGameTutorialCompleted}");
        }
    }

    private void ApplySoundOptions()
    {
        // OptionData가 null이면 기본값으로 초기화
        if (SaveData.optionData == null)
            SaveData.optionData = new OptionData();

        // 각 필드가 비정상 값일 case 기본값으로 복구
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.masterVolume = (SaveData.optionData.masterVolume > 0f && SaveData.optionData.masterVolume <= 1f)
                ? SaveData.optionData.masterVolume : 1f;
            SoundManager.Instance.bgmVolume = (SaveData.optionData.bgmVolume > 0f && SaveData.optionData.bgmVolume <= 1f)
                ? SaveData.optionData.bgmVolume : 1f;
            SoundManager.Instance.sfxVolume = (SaveData.optionData.sfxVolume > 0f && SaveData.optionData.sfxVolume <= 1f)
                ? SaveData.optionData.sfxVolume : 1f;
        }
    }

    private void EnsureStageDataIntegrity()
    {
        if (SaveData.stageData == null)
            SaveData.stageData = new StageData(new StageSaveData());
        if (SaveData.stageData.artifactNames == null || SaveData.stageData.artifactNames.Count != 12)
            SaveData.stageData.artifactNames = new List<string>(new string[12]);
        if (SaveData.stageData.engravingNames == null || SaveData.stageData.engravingNames.Count != 3)
            SaveData.stageData.engravingNames = new List<string>(new string[3]);
        if (SaveData.stageData.equipedArtifactNames == null || SaveData.stageData.equipedArtifactNames.Count != 4)
            SaveData.stageData.equipedArtifactNames = new List<string>(new string[4]);
        if (SaveData.stageData.entryCharacterIDs == null || SaveData.stageData.entryCharacterIDs.Count != 5)
            SaveData.stageData.entryCharacterIDs = new List<string>(new string[5]);
        if (SaveData.stageData.battleCharacters == null)
            SaveData.stageData.battleCharacters = new List<BattleCharacterData>();
        if (SaveData.stageData.chapterStates == null)
            SaveData.stageData.chapterStates = new List<ChapterStates>();
    }


}
