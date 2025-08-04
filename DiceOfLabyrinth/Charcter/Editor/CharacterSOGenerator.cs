using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;

/// <summary>
/// JSON 파일을 읽어 CharacterSO를 자동으로 생성하는 에디터 윈도우
/// 생성된 SO를 Addressable로 자동 등록
/// </summary>
public class CharacterSOGenerator : EditorWindow
{
    // JSON 파일 경로와 SO 저장 경로
    private string jsonPath = "Assets/Resources/Json/CharData.json";
    private string soOutputPath = "Assets/01. Scripts/Charcter/SO/Generated/";


    /// <summary>
    /// Unity 에디터 메뉴에 CharacterSO Generator를 추가
    /// </summary>
    [MenuItem("Tools/CharacterSO Generator")]
    public static void ShowWindow()
    {
        GetWindow<CharacterSOGenerator>("CharacterSO Generator");
    }

    /// <summary>
    /// 에디터 윈도우의 GUI를 정의
    /// </summary>
    private void OnGUI()
    {
        GUILayout.Label("CharacterSO 자동 생성기", EditorStyles.boldLabel);
        //JSON 파일 경로 입력 필드
        jsonPath = EditorGUILayout.TextField("JSON 경로", jsonPath);

        //SO 저장 경로 입력 필드
        soOutputPath = EditorGUILayout.TextField("SO 저장 경로", soOutputPath);

        // "생성" 버튼 => SO 생성 함수 호출
        if (GUILayout.Button("생성"))
        {
            GenerateCharacterSOs();
        }
    }

    /// <summary>
    /// JSON 파일을 읽어 CharacterSO를 생성하고 Addressable로 등록
    /// </summary>
    private void GenerateCharacterSOs()
    {
        // JSON 파일이 존재하는지 확인
        if (!File.Exists(jsonPath))
        {
            Debug.LogError("JSON 파일을 찾을 수 없습니다: " + jsonPath);
            return;
        }

        // JSON 파일 읽기 후 역직렬화
        string json = File.ReadAllText(jsonPath);
        CharDataListWrapper wrapper = JsonConvert.DeserializeObject<CharDataListWrapper>(json);

        // 역직렬화 실패 또는 데이터 없음
        if (wrapper == null || wrapper.Items == null)
        {
            Debug.LogError("JSON 파싱 실패 또는 Items가 비어있음");
            return;
        }

        // SO 저장 경로가 존재하지 않으면 생성
        if (!Directory.Exists(soOutputPath))
            Directory.CreateDirectory(soOutputPath);

        // 각 캐릭터 데이터에 대해 SO 생성 및 Addressable 등록 - 인스턴스 생성 및 데이터 할당
        foreach (var data in wrapper.Items)
        {
            CharacterSO so = ScriptableObject.CreateInstance<CharacterSO>();

            // 반드시 hideFlags를 None으로 설정 (Assertion 방지)
            so.hideFlags = HideFlags.None;

            so.key = data.key;
            so.charID = data.CharID;
            so.nameKr = data.NameKr;
            so.nameEn = data.NameEn;
            so.classType = data.ClassType;
            so.baseATK = data.BaseATK;
            so.plusATK = data.PlusATK;
            so.baseDEF = data.BaseDEF;
            so.plusDEF = data.PlusDEF;
            so.baseHP = data.BaseHP;
            so.plusHP = data.PlusHP;
            so.critChance = data.CritChance;
            so.critDamage = data.CritDamage;
            so.penetration = data.Penetration;
            so.elementDMG = data.ElementDMG;
            so.elementType = data.ElementType;            
            so.description = data.Description;
            so.dialog1 = data.dialog1;
            so.dialog2 = data.dialog2;
            so.diceID = data.DiceID;
            so.activeSkillID = data.ActiveSkill;
            so.passiveSkillID = data.PassiveSkill; // 패시브 스킬 ID 할당


            if (!string.IsNullOrEmpty(data.BattlePrefabPath))
            {
                var BattlePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(data.BattlePrefabPath);
                so.charBattlePrefab = BattlePrefab;
            }

            // 캐릭터 전용 주사위 프리팹 경로가 비어있지 않으면 할당
            if(!string.IsNullOrEmpty(data.DicePrefabPath))
            {
                var dicePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(data.DicePrefabPath);
                so.charDicePrefab = dicePrefab;
            }
            else
            {
                Debug.LogWarning($"캐릭터 {data.CharID}의 주사위 프리팹 경로가 비어있습니다.");
            }

            // 아이콘 및 스탠딩 이미지 할당
            if (!string.IsNullOrEmpty(data.iconPath))
            {
                so.icon = AssetDatabase.LoadAssetAtPath<Sprite>(data.iconPath);
            }
            if (!string.IsNullOrEmpty(data.UpperPath))
            {
                so.Upper = AssetDatabase.LoadAssetAtPath<Sprite>(data.UpperPath);
            }
            if (!string.IsNullOrEmpty(data.StandingPath))
            {
                so.Standing = AssetDatabase.LoadAssetAtPath<Sprite>(data.StandingPath);
            }
            // 신규 경로 필드 Sprite 할당
            if (!string.IsNullOrEmpty(data.ElementalIconPath))
            {
                so.elementIcon = AssetDatabase.LoadAssetAtPath<Sprite>(data.ElementalIconPath);
            }
            if (!string.IsNullOrEmpty(data.RoleIconPath))
            {
                so.RoleIcons = AssetDatabase.LoadAssetAtPath<Sprite>(data.RoleIconPath);
            }
            if (!string.IsNullOrEmpty(data.BackGroundPath))
            {
                so.BackGroundIcon = AssetDatabase.LoadAssetAtPath<Sprite>(data.BackGroundPath);
            }
            if (!string.IsNullOrEmpty(data.DiceNumIconPath))
            {
                so.DiceNumIcon = AssetDatabase.LoadAssetAtPath<Sprite>(data.DiceNumIconPath);
            }

            // 스킬 ID를 통해 액티브 및 패시브 스킬 SO를 할당 - 스킬 아이디에 맞는 SkillSO를 찾아서 할당 (ActiveSO와 PassiveSO는 SkillSO를 상속받음)
            // Addressable은 사용하지 않고, SO를 직접 할당
            // 모든 SkillSO의 경로 "Assets/01. Scripts/Charcter/SO/Skills"에 저장되어있음
            // 스킬 형식(Active,Passive공용) : Skill_1_SO, Skill_2_SO, ...

            if (!string.IsNullOrEmpty(so.activeSkillID))
            {
                string activeSkillPath = $"Assets/01. Scripts/Charcter/SO/Skills/{so.activeSkillID}_SO.asset";
                so.activeSO = AssetDatabase.LoadAssetAtPath<ActiveSO>(activeSkillPath);
                if (so.activeSO == null)
                {
                    Debug.LogWarning($"액티브 스킬 SO를 찾을 수 없습니다: {activeSkillPath}");
                }
            }

            if (!string.IsNullOrEmpty(so.passiveSkillID))
            {
                string passiveSkillPath = $"Assets/01. Scripts/Charcter/SO/Skills/{so.passiveSkillID}_SO.asset";
                so.passiveSO = AssetDatabase.LoadAssetAtPath<PassiveSO>(passiveSkillPath);
                if (so.passiveSO == null)
                {
                    Debug.LogWarning($"패시브 스킬 SO를 찾을 수 없습니다: {passiveSkillPath}");
                }
            }

            // DiceDataLoader를 통해 Dice 데이터 로드
            DiceDataLoader diceLoader = new DiceDataLoader(); // 기본 경로 사용
            CharDiceData diceData = diceLoader.ItemsList.Find(d => d.DiceID == data.DiceID);
            so.charDiceData = diceData;

            // SO 파일 경로 설정
            string assetPath = $"{soOutputPath}{so.nameEn}_SO.asset";

            // 기존 SO 파일이 있으면 삭제
            if (File.Exists(assetPath))
                AssetDatabase.DeleteAsset(assetPath);

            // 반드시 hideFlags가 None인 상태에서 저장 (Assertion 방지)
            so.hideFlags = HideFlags.None;
            AssetDatabase.CreateAsset(so, assetPath);

            // SO를 저장한 후, 프로젝트에서 해당 에셋을 다시 불러옴
            var soAsset = AssetDatabase.LoadAssetAtPath<CharacterSO>(assetPath);

            // CharacterSO 라벨 자동 할당
            AssetDatabase.SetLabels(soAsset, new[] { "CharacterSO" });

            // Addressable 등록
            var Asettings = AddressableAssetSettingsDefaultObject.Settings;
            if (Asettings != null)
            {
                // "Character SO Group" 그룹 찾기 또는 생성
                var groupName = "Character SO Group";
                var group = Asettings.FindGroup(groupName);
                if (group == null)
                {
                    group = Asettings.CreateGroup(groupName, false, false, false, null, typeof(BundledAssetGroupSchema));
                }

                string guid = AssetDatabase.AssetPathToGUID(assetPath);
                var entry = Asettings.CreateOrMoveEntry(guid, group);
                entry.address = so.nameEn;
                entry.SetLabel("CharacterSO", true);
            }

            

            // 에셋 저장 및 데이터베이스 갱신
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // SO를 다시 불러와서 라벨 할당
            soAsset = AssetDatabase.LoadAssetAtPath<CharacterSO>(assetPath);
            if (soAsset != null)
            {
                AssetDatabase.SetLabels(soAsset, new[] { "CharacterSO" });
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.LogError($"SO 에셋을 다시 불러오지 못했습니다: {assetPath}");
            }
        }

        // Addressable 설정 업데이트
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings != null)
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, null, true);

        Debug.Log("CharacterSO 생성 및 Addressable 등록 완료!");
    }

    /// <summary>
    /// 역직렬화를 위한 래퍼(캐릭터 데이터 리스트 보관)
    /// </summary>

    [System.Serializable]
    private class CharDataListWrapper
    {
        public List<CharDataForSO> Items;
    }

    /// <summary>
    /// 
    /// JSON 데이터 구조와 매칭되는 캐릭터 데이터 클래스
    /// </summary>
    [System.Serializable]
    private class CharDataForSO
    {
        public int key;
        public string CharID;
        public string NameKr;
        public string NameEn;
        public DesignEnums.ClassTypes ClassType;
        public int BaseATK;
        public int PlusATK;
        public int BaseDEF;
        public int PlusDEF;
        public int BaseHP;
        public int PlusHP;
        public float CritChance;
        public float CritDamage;
        public int Penetration;
        public float ElementDMG;
        public DesignEnums.ElementTypes ElementType;
        public string DiceID;
        public string ActiveSkill; // 액티브 스킬 ID
        public string PassiveSkill; // 패시브 스킬 ID
        public string Description;
        public string dialog1;
        public string dialog2;
        public string BattlePrefabPath; // 프리팹 경로 추가
        public string iconPath; // 아이콘 경로 추가
        public string UpperPath; // 상체 이미지 경로 추가
        public string StandingPath; // 스탠딩 이미지 경로 추가
        public string DicePrefabPath; // 캐릭터 전용 주사위 프리팹 경로 추가
        public string ElementalIconPath; // 원소 아이콘 경로 추가
        public string RoleIconPath; // 역할 아이콘 경로 추가
        public string BackGroundPath; // 배경 경로 추가
        public string DiceNumIconPath; // 주사위 숫자 아이콘 경로 추가
    }
}