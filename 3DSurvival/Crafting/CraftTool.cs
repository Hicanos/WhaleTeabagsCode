using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftTool : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject craftingUI; //제작 UI 오브젝트
    private CraftingSystem craftingSystem; //크래프팅 시스템 참조
    private bool craftMode;


    [Header("마우스 커서")]
    [SerializeField] private Texture2D craftingCursor; //제작커서

    private void Start()
    {        
        craftMode = false; //초기 제작 모드 비활성화
        craftingUI.SetActive(false); //제작 UI 비활성화
    }


    //제작대에 다가가서 상호작용을 하면 
    public string GetInteractPrompt()
    {
        return $"제작대와 상호작용 하려면 [E]를 눌러주세요."; //상호작용 프롬프트
    }

    public void OnInteract()
    {
        //제작대 상호작용> 제작 UI 띄우기
        //상호작용 시 UI를 활성화. 다시 한 번 사용 시 UI 비활성. UI가 활성화 되는 동안 캐릭터는 이동 금지
        craftMode = !craftMode; //제작 모드 토글. E를 눌러서 제작 UI를 켜고 끔
        if (craftMode)
        {
            craftingUI.SetActive(true); //제작 UI 활성화
            craftingSystem = FindObjectOfType<CraftingSystem>(); //크래프팅 시스템 참조
            Cursor.SetCursor(craftingCursor, Vector2.zero, CursorMode.Auto); //커서 변경
            Cursor.lockState = CursorLockMode.None; // 커서 자유 이동
            Cursor.visible = true;
            GameManager.Instance.Player.controller.isInputBlocked = true; // 플레이어 입력 차단


        }
        else
        {
            craftingUI.SetActive(false); //제작 UI 비활성화
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto); //커서 초기화
            Cursor.lockState = CursorLockMode.Locked; // 커서 잠금
            Cursor.visible = false; // 커서 숨김
            GameManager.Instance.Player.controller.isInputBlocked = false; // 플레이어 입력 차단 해제

            craftingSystem.Reset(); //크래프팅 시스템 초기화
        }
    }
}
