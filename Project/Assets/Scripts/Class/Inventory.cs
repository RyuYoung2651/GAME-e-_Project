using System.Collections.Generic;
using UnityEngine;

// 참조 3개 (Reference 3)
public class Inventory : MonoBehaviour
{
    // 참조 7개 (Reference 7)
    public Dictionary<BlockType, int> items = new();

    // 참조 1개 (Reference 1)
    public void Add(BlockType type, int count = 1)
    {
        if (!items.ContainsKey(type)) items[type] = 0;
        items[type] += count;
        Debug.Log($"[Inventory] +{count} {type} (총 {items[type]})");
    }

    // 참조 0개 (Reference 0)
    public bool Consume(BlockType type, int count = 1)
    {
        if (!items.TryGetValue(type, out var have) || have < count) return false;
        items[type] = have - count;
        Debug.Log($"[Inventory] -{count} {type} (총 {items[type]})");
        return true;
    }
}