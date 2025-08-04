using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceArea : MonoBehaviour
{
    public bool IsBlocked { get; private set; } // 자원 지역이 차단되었는지 여부
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            IsBlocked = true; // 플레이어나 적이 들어오면 차단
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            IsBlocked = false; // 플레이어나 적이 나가면 차단 해제
        }
    }
}
