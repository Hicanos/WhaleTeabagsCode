using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임 전체의 데이터 로드, 복구, 릴리즈를 총괄하는 매니저
/// - 모든 SO/데이터를 StaticDataManager에서 관리
/// - 데이터 복구는 SO 로드 완료 후에만 진행
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // 중복 인스턴스 제거
        }
    }

    /// <summary>
    /// 게임 시작 시점에 모든 SO/데이터를 StaticDataManager에서 관리 후 복구
    /// </summary>
    private IEnumerator Start()
    {
        while (UserDataManager.Instance == null)
            yield return null; // UserDataManager가 생성될 때까지 대기

        // StageManager.Instance가 생성될 때까지 대기
        while (StageManager.Instance == null)
            yield return null;

        // 캐릭터 SO 로드 대기
        yield return CharacterManager.Instance.LoadAllCharactersAsync();

        // 아이템 SO 로드 대기
        yield return ItemManager.Instance.LoadAllItemSOs();

        // 튜토리얼 매니저 생성대기
        while (TutorialManager.Instance == null)
            yield return null;

        while (SoundManager.Instance == null)
            yield return null;


        // SO 데이터 복구
        RestoreGameData();
        Debug.Log("게임 데이터 로드 및 SO 복구 완료");
    }

    private void RestoreGameData()
    {
        DataSaver.Instance.Load();
        CharacterManager.Instance.LoadOwnedCharactersFromData();
        
        // 기본 캐릭터 획득(중복 획득 방지 처리됨)
        CharacterManager.Instance.AcquireDefaultCharacters();
        ItemManager.Instance.LoadOwnedItemsFromData();
        if (StageManager.Instance != null && DataSaver.Instance.SaveData.stageData != null)
        {
            StageManager.Instance.stageSaveData = DataSaver.Instance.SaveData.stageData.ToStageSaveData();
            StageManager.Instance.InitializeStageStates(StageManager.Instance.chapterData);
        }

        SoundManager.Instance.ApplyVolumes();

        // 로드 완료 되면 타이틀 재생
        StartBGM();
        UserDataManager.Instance.RecoverStaminaFromLastQuit();
    }

    private void StartBGM()
    {
        SoundManager.Instance.PlayBGM(SoundManager.SoundType.BGM_Title);
    }

    /// <summary>
    /// 게임 저장
    /// </summary>
    private void SaveGame()
    {
        DataSaver.Instance.Save();
    }

    /// <summary>
    /// 게임 종료
    /// </summary>
    public void ExitGame()
    {
        SaveGame();
        Debug.Log("게임 종료됨");
        ItemManager.Instance.ReleaseAllItems();
        CharacterManager.Instance.ReleaseAllCharacters();
        Application.Quit();
    }

    /// <summary>
    /// 게임 초기화
    /// </summary>
    public void ResetGame()
    {
        SaveGame();
        Debug.Log("게임 초기화됨");
        // 추가: 데이터 파일 삭제, 씬 재시작 등
    }

    /// <summary>
    /// 애플리케이션 종료 시 자동 저장
    /// </summary>
    private void OnApplicationQuit()
    {
        Debug.Log("애플리케이션 종료됨");
        Debug.Log("게임 데이터 저장 중...");
        SaveGame();
        // 아이템, 캐릭터 릴리즈
        ItemManager.Instance.ReleaseAllItems();
        CharacterManager.Instance.ReleaseAllCharacters();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            Debug.Log("앱이 백그라운드로 전환됨, 데이터 자동 저장");
            SaveGame();
        }
    }
}
