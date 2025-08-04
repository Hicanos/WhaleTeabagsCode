using UnityEngine;

/// <summary>
/// 마커가 좌우로 오가며 핀포인트를 찾는 게임
/// </summary>
public class TimingMatch : MonoBehaviour, IPuzzle
{
    //Dagger 에서 상호작용하면 여기로 온다.
    // StartPuzzle 에서 게임 시작.

    [Header("활성 UI 오브젝트")]//외부에서 제대로 설정되었는지 확인하기 위함
    [SerializeField] private GameObject RailObj; //가로길이  1000 (총 1)   
    [SerializeField] private RectTransform marker; //마커 UI 위치 생성 범위 
    [SerializeField] private RectTransform pinPoint; //핀포인트 UI (정답 위치) (0.2~0.8 사이의 위치에 생성됨)

    private PuzzlePlayerController playerController;

    // 필요한 오브젝트를 외부에서 설정할 수 있도록 프로퍼티로 정의
    // RailObj의 하위에 Marker, PinPoint가 자식으로 존재.
    // TimingMatch 아래에 RailObj를 SetActive(true)로 활성화 시켜야 함.

    // 아래는 게임 진행이 원활하게 진행되는 것이 확인하면 제거 [SerializeField] 제거
    [Header("Game 카운터")]
    private int maxCycles = 3; // 왕복 3회 고정, 초과시 1회 실패
    private int maxCount = 3; // 마커가 핀포인트에 도달해야 하는 횟수 (3회 성공 시 퍼즐 해제)
    private int success = 0; // 성공 횟수 = 3회 성공 시 퍼즐 해제
    private int failed = 0; // 실패 횟수 =3회 실패시 게임오버

    private int direction = 1; // 1은 양수 =>오른쪽방향, -1은 Flip. 음수. <= 왼쪽방향
    private int currentCycle = 0; // 현재 진행 중인 사이클 수
    private bool isPlaying = false; // 게임 진행 중인지 여부
    private float markerPos = 0f; // 마커의 현재 위치 (0~1 사이의 값)

    private float railLeftX; // 레일의 왼쪽 끝 X 좌표
    private float railRightX; // 레일의 오른쪽 끝 X 좌표
    private RectTransform railRect; // 레일의 RectTransform
    private float railWidth; // 레일의 너비
    private float moveSpeed = 0.5f; // 마커 이동 속도 (초당 비율 이동)
    private float pinPointRange = 0.05f; // 핀포인트 위치의 허용 오차 범위 (0.05f = 5% 허용 오차)

    private ITiming target; // 타이밍 매치를 부른 객체

    public void Init()
    {
        PuzzleManager.instance.RegisterPuzzle(this); // 퍼즐 매니저에 등록
        RailObj = this.TryFindChild("BackGround");
        marker = this.TryGetChildComponent<RectTransform>("Arrow");
        pinPoint = this.TryGetChildComponent<RectTransform>("Range");
    }

    // 시작 시 세팅
    private void Start()
    {
        if (RailObj != null)
        {
            railRect = RailObj.GetComponent<RectTransform>();
            railLeftX = railRect.rect.xMin; // 레일의 왼쪽 끝 X 좌표
            railRightX = railRect.rect.xMax; // 레일의 오른쪽 끝 X 좌표

            playerController = FindObjectOfType<PuzzlePlayerController>();
            if (playerController == null)
            {
                Service.Log("TimingMatch: PlayerController를 찾을 수 없습니다.");
            }
            else
            {
                playerController.SetPauseState(true);// 시작 시 커서 잠금
            }
        }
    }

    // TimingMatch를 호출한 객체 세팅 (없어도 작동은 되어야함)
    public void SetTarget(ITiming timingtarget)
    {
        target = timingtarget; // 타이밍 매치를 부른 객체 설정
        if(target == null)
        {
            Service.Log("TimingMatch: SetTarget(): 타겟이 null입니다. 타이밍 매치가 정상적으로 작동하지 않을 수 있습니다.");
        }
    }



    // 퍼즐 시작 로직
    public void StartPuzzle()
    {
        // 1. 게임의 정보 초기화
        Start(); // 게임 시작 메서드 호출
        success = 0;
        failed = 0;
        currentCycle = 0;
        direction = 1;
        markerPos = 0f; // 마커 위치 초기화

        isPlaying = true; // 게임을 시작할 수 있도록 bool 변경
        UiManager.Instance.Show<TimingMatchUi>(true); // TimingMatchUi 활성화
        MonsterStateMachine.OnPuzzle();
        UpdateMarkerPosition(); // 마커 위치 업데이트

    }

    // 게임은 매 프레임 업데이트 되며 마커가 좌우로 움직임.
    // 단, isPlaying이 true일 때만 업데이트.
    private void Update()
    {
        if (!isPlaying || RailObj == null || marker == null || pinPoint == null)
        {
            return; // 게임이 진행 중이 아니거나 필요한 오브젝트가 할당되지 않은 경우
        }
        GamePlaying(); // 게임 진행 로직 실행
    }

    private void GamePlaying()
    {
        //마커 이동
        markerPos += direction * moveSpeed * Time.deltaTime; // 마커 위치 업데이트 (0~1 사이의 값으로 이동)
        markerPos = Mathf.Clamp01(markerPos); // 마커 위치를 0~1 사이로 제한

        //방향 전환 및 사이클 체크
        if (markerPos >= 1f)
        {
            markerPos = 1f; // 마커가 오른쪽 끝에 도달하면 위치를 1로 고정
            direction = -1; // 방향을 왼쪽으로 변경
        }
        else if (markerPos <= 0f)
        {
            markerPos = 0f; // 마커가 왼쪽 끝에 도달하면 위치를 0으로 고정
            direction = 1; // 방향을 오른쪽으로 변경
            currentCycle++; // 사이클 증가

            if (currentCycle >= maxCycles) // 최대 사이클 수에 도달한 경우
            {
                IsFail(); // 실패 처리
                return; // 실패처리를 따름
            }
        }

        UpdateMarkerPosition(); // 마커 위치 업데이트
        // Space 입력 시 판정
        if (Input.GetKeyDown(KeyCode.F)) // E 키 입력 시
        {
            Judge(); // 마커-핀포인트 타이밍을 판단
        }

    }

    // 마커-핀포인트 타이밍을 판단하는 메서드
    private void Judge()
    {
        float pinRatio = GetPinPointRatio(); // 핀포인트 위치 비율을 가져옴
        float distance = Mathf.Abs(markerPos - pinRatio); // 마커 위치와 핀포인트 위치 비율의 차이 계산

        if (distance < pinPointRange) // 마커 위치가 핀포인트 위치와 가까운 경우
        {
            IsSuccess(); // 성공 처리
        }
        else
        {
            IsFail(); // 실패 처리
        }
    }

    //핀 포인트 위치 비율 반환(0.2~0.8), 전체 비율은 0~1 사이의 값
    private float GetPinPointRatio()
    {
        if (RailObj == null || pinPoint == null)
        {
            return 0.5f; // 중간 위치 반환
        }

        railWidth = railRect.rect.width; // 레일의 너비
        float pinPointX = pinPoint.localPosition.x - railRect.localPosition.x; // 레일의 왼쪽 끝에서 핀포인트 위치까지의 X 좌표 계산
        // 핀포인트 위치를 0~1 사이의 비율로 변환
        float pinPointRatio = (pinPointX - railRect.rect.xMin) / railWidth; // 0~1 사이의 비율로 변환

        // 핀포인트 위치가 레일의 범위를 벗어나지 않도록 여유를 두고 클램프
        pinPointRatio = Mathf.Clamp(pinPointRatio, 0.2f, 0.8f); // 핀포인트 위치 비율을 0.2~0.8 사이로 제한
        return pinPointRatio; // 핀포인트 위치 비율 반환

    }


    //마커의 실제 위치를 UI에 반영시킴
    private void UpdateMarkerPosition()
    {
        if (RailObj == null || marker == null)
        {
            return;
        }
        railWidth = railRect.rect.width; // 레일의 너비
        float x = railRect.rect.xMin + (railWidth * markerPos); // 레일의 왼쪽 끝에서 마커 위치까지의 X 좌표 계산
        marker.localPosition = new Vector3(x, marker.localPosition.y, marker.localPosition.z); // 마커의 위치 업데이트
    }


    private void IsSuccess()
    {
        success++; // 성공 횟수 증가
        if (success >= maxCount)
        {
            MonsterStateMachine.OffPuzzle();
            IsSolved(); // 성공 횟수가 최대치에 도달하면 퍼즐 해결
        }
        else
        {
            markerPos = 0f; // 마커 위치 초기화
            direction = 1;
            currentCycle = 0; // 사이클 초기화
            UpdateMarkerPosition(); // 마커 위치 업데이트
        }
    }

    private void IsFail()
    {
        failed++; // 실패 횟수 증가
        SoundManager.PlaySfx(SoundCategory.Interaction, "LockDoor");
        if (failed == 2)
        {
            UiManager.Instance.Get<TalkUi>().Popup("하하 하하 하하 하 하하");
            SoundManager.PlaySfx(SoundCategory.Movement, "PuzzleMonster4"); // 실패 사운드 재생
        }
        else if (failed >= maxCycles)
        {
            IsFailed(); // 실패 횟수가 최대치에 도달하면 퍼즐 실패
        }
        else
        {
            markerPos = 0f; // 마커 위치 초기화
            direction = 1;
            currentCycle = 0; // 사이클 초기화
            UpdateMarkerPosition(); // 마커 위치 업데이트
        }
    }


    // 게임 종료 후 성공 여부를 판단하는 메서드
    public void IsSolved()
    {
        isPlaying = false; // 게임 종료

        UiManager.Instance.Show<TimingMatchUi>(false); // TimingMatchUi 비활성화

        playerController.SetPauseState(false); // 플레이어 컨트롤러의 입력 잠금 해제
        SoundManager.PlaySfx(SoundCategory.Interaction, "UnLockDoor"); // 성공 사운드 재생
        //성공 연출 및 로직 처리

        if(target != null)
        {
            target.isSolved = true;
        }
        else
        {
            Service.Log("TimingMatch: IsSolved(): 타겟이 null입니다. 퍼즐이 성공했지만 타겟이 설정되지 않았습니다.");
        }       

        ResetSetting(); // 차후 다른 위치에서 게임이 처음부터 시작되도록 초기화
    }

    public void IsFailed()
    {
        isPlaying = false; // 게임 종료

        UiManager.Instance.Show<TimingMatchUi>(false); // TimingMatchUi 비활성화
        MonsterStateMachine.OffPuzzle();

        playerController.SetPauseState(false); // 플레이어 컨트롤러의 입력 잠금 해제
        // 실패 연출 및 로직 처리
        UiManager.Instance.Get<TalkUi>().Popup("하하 하하 하하 하 하하");
        SoundManager.PlaySfx(SoundCategory.Movement, "PuzzleMonster4"); // 실패 사운드 재생
        playerController.Die(); // 플레이어 사망 처리
        ResetSetting();
    }

    // 게임을 초기화 하는 메서드
    private void ResetSetting()
    {
        // 게임 초기화 메서드
        success = 0;
        failed = 0;
        currentCycle = 0;
        direction = 1;
        markerPos = 0f; // 마커 위치 초기화
        isPlaying = false; // 게임을 중지 상태로 변경
        target = null; // 타겟 초기화
        UpdateMarkerPosition(); // 마커 위치 업데이트
    }
}