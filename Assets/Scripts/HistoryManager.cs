using System;
using System.Collections.Generic;
using UnityEngine;

public class HistoryManager : MonoBehaviour
{
    private Queue<KeyValuePair<GameObject, Color32>> history = new();

    public void Push(GameObject pixel, Color32 color)
    {
        history.Enqueue(new KeyValuePair<GameObject, Color32>(pixel, color));
    }

    public KeyValuePair<GameObject, Color32> Pop()
    {
        if (history.Count == 0)
            throw new InvalidOperationException("History is empty");

        return history.Dequeue();
    }

    public int Count 
    {
        get { return history.Count; }
    }
}
