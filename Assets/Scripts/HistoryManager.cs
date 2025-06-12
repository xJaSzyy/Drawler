using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HistoryManager : MonoBehaviour
{
    private Queue<KeyValuePair<Vector3Int, Color32>> history = new Queue<KeyValuePair<Vector3Int, Color32>>();

    public void Push(Vector3Int pos, Color32 color)
    {
        history.Enqueue(new KeyValuePair<Vector3Int, Color32>(pos, color));
    }

    public KeyValuePair<Vector3Int, Color32> Pop()
    {
        if (history.Count == 0)
            throw new InvalidOperationException("History is empty");

        return history.Dequeue();
    }

    public int Count()
    {
        return history.Count;
    }
}
