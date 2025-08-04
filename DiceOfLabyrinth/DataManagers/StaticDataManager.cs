using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임에서 사용되는 정적데이터(아티팩트, 각인, 적 등)의 데이터 매니저
/// Artifact, Engraving, Enemy 등의 SO를 들고다니며 이름 기반으로 일관 관리
/// </summary>
public class StaticDataManager : MonoBehaviour
{
    // 싱글톤 인스턴스. 게임 내에서 정적 데이터에 접근할 때 사용
    public static StaticDataManager Instance { get; private set; }

    // 인스펙터에서 할당하는 아티팩트 SO 리스트
    [SerializeField] private List<ArtifactData> artifacts;
    // 인스펙터에서 할당하는 각인 SO 리스트
    [SerializeField] private List<EngravingData> engravings;
    // 인스펙터에서 할당하는 적 SO 리스트
    [SerializeField] private List<EnemyData> enemies;

    // 이름(string)으로 SO를 빠르게 찾기 위한 딕셔너리
    private Dictionary<string, ArtifactData> artifactDict = new Dictionary<string, ArtifactData>();
    private Dictionary<string, EngravingData> engravingDict = new Dictionary<string, EngravingData>();
    private Dictionary<string, EnemyData> enemyDict = new Dictionary<string, EnemyData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitDictionaries(); // SO 리스트를 이름 기반 딕셔너리에 등록
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// SO 리스트를 이름(string) 기반 딕셔너리에 등록하여 빠른 조회 가능하게 함
    /// </summary>
    private void InitDictionaries()
    {
        artifactDict.Clear();
        foreach (var so in artifacts)
            if (so != null && !string.IsNullOrEmpty(so.ArtifactName))
                artifactDict[so.ArtifactName] = so;

        engravingDict.Clear();
        foreach (var so in engravings)
            if (so != null && !string.IsNullOrEmpty(so.EngravingName))
                engravingDict[so.EngravingName] = so;

        enemyDict.Clear();
        foreach (var so in enemies)
            if (so != null && !string.IsNullOrEmpty(so.EnemyName))
                enemyDict[so.EnemyName] = so;
    }

    /// <summary>
    /// 이름(string)으로 아티팩트 SO를 반환
    /// </summary>
    public ArtifactData GetArtifact(string name) =>
        artifactDict.TryGetValue(name, out var so) ? so : null;
    /// <summary>
    /// 이름(string)으로 각인 SO를 반환
    /// </summary>
    public EngravingData GetEngraving(string name) =>
        engravingDict.TryGetValue(name, out var so) ? so : null;
    /// <summary>
    /// 이름(string)으로 적 SO를 반환
    /// </summary>
    public EnemyData GetEnemy(string name) =>
        enemyDict.TryGetValue(name, out var so) ? so : null;

    /// <summary>
    /// 전체 아티팩트 SO 리스트 반환
    /// </summary>
    public List<ArtifactData> GetAllArtifacts() => artifacts;
    /// <summary>
    /// 전체 각인 SO 리스트 반환
    /// </summary>
    public List<EngravingData> GetAllEngravings() => engravings;
    /// <summary>
    /// 전체 적 SO 리스트 반환
    /// </summary>
    public List<EnemyData> GetAllEnemies() => enemies;
}
