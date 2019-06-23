using System.Collections.Generic;

public class FixedSizeQueue<T> : Queue<T>
{
    private int size;

    public FixedSizeQueue(int size) : base(size)
    {
        this.size = size;
    }

    public new void Enqueue(T item)
    {
        while (this.Count >= this.size)
        {
            Dequeue();
        }
        base.Enqueue(item);
    }
}