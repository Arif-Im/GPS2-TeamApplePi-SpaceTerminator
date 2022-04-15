using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionalQueue<T>
{
    private List<T> elements = new List<T>();

    public int Count { get => elements.Count; }

    // Enqueue
    public void Enqueue(T item)
    {
        elements.Add(item);
    }

    // Dequeue
    public T Dequeue()
    {
        // lower number = higher priority

        T bestItem = elements[0];
        elements.RemoveAt(0);

        return bestItem;
    }

    public T Peek()
    {
        return elements[0];
    }

    public void Dequeue(T item)
    {
        // lower number = higher priority

        for(int i = 0; i < Count; i++)
        {
            if (elements[i].Equals(item))
            {
                elements.RemoveAt(i);
                return;
            }
        }

        Debug.Log("None in Queue");
    }

    public bool Contains(T item)
    {
        foreach(T element in elements)
        {
            if (element.Equals(item))
                return true;
        }
        return false;
    }
}
