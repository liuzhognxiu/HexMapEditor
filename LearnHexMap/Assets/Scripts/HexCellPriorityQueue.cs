using System.Collections.Generic;

public class HexCellPriorityQueue
{
    List<HexCell> list = new List<HexCell>();

    int minimum = int.MaxValue;

    /// <summary>
    /// 加入队列
    /// </summary>
    /// <param name="cell"></param>
    public void Enqueue(HexCell cell)
    {
        count += 1;
        int priority = cell.SearchPriority;

        if (priority < minimum)
        {
            minimum = priority;
        }

        while (priority >= list.Count)
        {
            list.Add(null);
        }

        cell.NextWithSamePriority = list[priority];
        list[priority] = cell;
    }

    /// <summary>
    /// 移出队列
    /// </summary>
    /// <returns></returns>
    public HexCell Dequeue()
    {
        count -= 1;
        //移出队列的时候通过最小值来遍历，不是从0开始遍历
        for (; minimum < list.Count; minimum++)
        {
            HexCell cell = list[minimum];
            if (cell != null)
            {
                list[minimum] = cell.NextWithSamePriority;
                return cell;
            }
        }

        return null;
    }

    int count = 0;

    /// <summary>
    /// 获取当前队列中有多个cell
    /// </summary>
    public int Count
    {
        get { return count; }
    }

    /// <summary>
    /// 改变格子的优先级
    /// </summary>
    /// <param name="cell"></param>
    /// <param name="oldPriority">当前格子旧的优先级</param>
    public void Change(HexCell cell, int oldPriority)
    {
        HexCell current = list[oldPriority];
        HexCell next = current.NextWithSamePriority;
        if (current == cell) {
            list[oldPriority] = next;
        }
        else {
            //将格子引用的格子优先级全部改变
            while (next != cell) {
                current = next;
                next = current.NextWithSamePriority;
            }
            current.NextWithSamePriority = cell.NextWithSamePriority;
        }
        //修改过单元格优先级之后，必须重新添加这个单元格，并且不改变队列数量
        Enqueue(cell);
        count -= 1;
    }

    public void Clear()
    {
        list.Clear();
        count = 0;
        minimum = int.MaxValue;
    }
}