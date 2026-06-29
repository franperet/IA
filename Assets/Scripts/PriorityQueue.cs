using System.Collections.Generic;

public class PriorityQueue<T>
{
    private List<(T item, float priority)> elements = new List<(T, float)>();

    public bool IsEmpty => elements.Count == 0;

    public void Enqueue(T item, float priority)
    {
        elements.Add((item, priority));
    }

    public T Dequeue()
    {
        int best = 0;
        for (int i = 1; i < elements.Count; i++)
            if (elements[i].priority < elements[best].priority)
                best = i;
        T item = elements[best].item;
        elements.RemoveAt(best);
        return item;
    }
}
