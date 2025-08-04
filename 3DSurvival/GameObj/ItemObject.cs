using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IInteractable
{
    public string GetInteractPrompt();//상호작용 프롬포트 가져오기
    public void OnInteract(); //상호작용시 데이터
}

public class ItemObject : MonoBehaviour, IInteractable
{
    public ItemData data; //아이템 데이터

    private MouseCursor mouseCursor; // 마우스 커서

    void Start()
    {
        mouseCursor = FindObjectOfType<MouseCursor>();
    }

    void OnMouseEnter()
    {
        mouseCursor.SetPickupCursor();
    }

    void OnMouseExit()
    {
        mouseCursor.SetDefaultCursor();
    }

    public string GetInteractPrompt()
    {
        return $"[E] {data.itemName}\n{data.itemDescription}"; //상호작용 프롬프트
        // 내용은 차후 변경

    }

    public void OnInteract()
    {
        Debug.Log($"Picked up item: {data.itemName}");
        GameManager.Instance.Player.itemQueue.Add(data); //플레이어의 아이템 데이터에 해당 아이템 데이터 저장
        GameManager.Instance.Player.additem?.Invoke(); //아이템 획득 이벤트 발생
        Destroy(gameObject); //아이템 오브젝트 삭제
    }
}
