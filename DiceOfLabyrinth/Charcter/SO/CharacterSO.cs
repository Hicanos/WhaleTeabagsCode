using UnityEngine;
/// <summary>
/// 테스트 캐릭터를 만들기 위한 Scriptable Object
/// </summary>

[CreateAssetMenu(fileName = "NewCharacterSO", menuName = "Character/Create CharacterSO")]
public class CharacterSO : ScriptableObject
{
    [Header("캐릭터 기본 정보")]
    public int key;
    public string charID;
    public string nameKr;
    public string nameEn;
    public DesignEnums.ClassTypes classType;
    public Sprite RoleIcons;
    public int baseATK;
    public int plusATK;
    public int baseDEF;
    public int plusDEF;
    public int baseHP;
    public int plusHP;
    public float critChance;
    public float critDamage;
    public int penetration;

    [Header("캐릭터 속성")]
    public float elementDMG;
    public DesignEnums.ElementTypes elementType;
    public Sprite elementIcon; // 속성 아이콘

    [Header("캐릭터 설명")]
    public string description;
    public string dialog1;
    public string dialog2;

    [Header("캐릭터 스킬 정보")]
    public string activeSkillID;
    public ActiveSO activeSO; // 액티브 스킬 Scriptable Object
    public string passiveSkillID;
    public PassiveSO passiveSO; // 패시브 스킬 Scriptable Object

    [Header("캐릭터 주사위 정보")]
    public string diceID;
    public GameObject charBattlePrefab; // 배틀에서 사용되는 캐릭터 프리팹
    public CharDiceData charDiceData; // 캐릭터 전용 주사위 데이터
    public GameObject charDicePrefab; // 캐릭터 전용 주사위 프리팹
    public Sprite DiceNumIcon; // 주사위 숫자 아이콘

    [Header("캐릭터 이미지")]
    public Sprite icon; // 캐릭터 아이콘
    public Sprite Upper; // 캐릭터 상체 이미지
    public Sprite Standing; // 캐릭터 스탠딩 이미지
    public Sprite BackGroundIcon; // 캐릭터 배경 아이콘
}