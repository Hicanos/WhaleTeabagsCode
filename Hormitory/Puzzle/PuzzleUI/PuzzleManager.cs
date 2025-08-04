using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  퍼즐 전반을 묶어주는 곳.
///  다른 객체에서 퍼즐을 호출하기 쉽도록 함.
/// </summary>
public class PuzzleManager
{
    public static PuzzleManager instance;

    // 모든 퍼즐을 쉽게 접근 할 수 있도록 Instance하는 역할. 
    // 모든 퍼즐은 IPuzzle 인터페이스를 가진다. -예외 발생: CountMatchController

    static PuzzleManager()
    {
        instance = new PuzzleManager();
    }

    private Dictionary<Type, object> puzzleDict = new Dictionary<Type, object>();

    // Puzzle 등록 메서드
    public void RegisterPuzzle<T>(T puzzle) where T : class
    {
        var type = typeof(T); 

        if(puzzle == null) //해당요소(퍼즐)이 없다면
        {
            Service.Log($"{type.Name}이 존재하지 않습니다.(null)");
            return;
        }
        if(puzzleDict.ContainsKey(type))
        {
            Service.Log($"{type.Name}이 이미 존재하여 덮어씁니다.");
        }
        puzzleDict[type] = puzzle;
    }

    public T GetPuzzle<T>() where T : class
    {
        var type = typeof(T);
        if (puzzleDict.TryGetValue(type, out var obj))
            return obj as T;
        Service.Log($"{type.Name}이 등록되어있지 않습니다,");
        return null;
    }
   
}
