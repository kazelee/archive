#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

namespace DouCardPuzzoom.scripts.tools; 

// 手写比较器
class DistinctIntListComparer : IEqualityComparer<List<int>> {
    public bool Equals(List<int>? x, List<int>? y) {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        // return x.Capacity == y.Capacity && x.Count == y.Count;
        
        if (x.Count != y.Count) return false;
        // 不存在对应index元素不相等的情况，即相等
        return !x.Where((t, i) => t != y[i]).Any();
        
    }

    public int GetHashCode(List<int> obj) {
        return HashCode.Combine(obj.Capacity, obj.Count);
    }
}

public static class CombineTool {
    public static List<List<int>> GetCombinations(List<int> array, int length)
    {
        List<List<int>> result = new List<List<int>>();
        GenerateCombinations(array, length, 0, new List<int>(), result);
        return result.Distinct(new DistinctIntListComparer()).ToList();
    }

    private static void GenerateCombinations(List<int> array, int length, int start, List<int> current, List<List<int>> result)
    {
        if (current.Count == length)
        {
            result.Add(new List<int>(current));
            return;
        }

        for (int i = start; i < array.Count; i++)
        {
            current.Add(array[i]);
            GenerateCombinations(array, length, i + 1, current, result);
            current.RemoveAt(current.Count - 1);
        }
    }
}