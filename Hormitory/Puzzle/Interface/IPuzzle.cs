using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 모든 퍼즐 기능 스크립트가 공통적으로 사용하는 인터페이스
/// </summary>
public interface IPuzzle
{
    // 각 퍼즐의 시작 로직
    public void StartPuzzle();

    //각 퍼즐의 해결 완료 로직
    public void IsSolved();

    // 각 퍼즐의 해결 실패 로직
    public void IsFailed();
}
