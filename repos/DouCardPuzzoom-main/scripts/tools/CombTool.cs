#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using DouCardPuzzoom.scripts.classes;
using Godot;

namespace DouCardPuzzoom.scripts.tools; 

class DistinctCombComparer : IEqualityComparer<CombData> {
    public bool Equals(CombData? x, CombData? y) {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        // return x.Capacity == y.Capacity && x.Count == y.Count;

        if (x.IsSuitSensitive != y.IsSuitSensitive) return false;
        if (x.IsSuitSensitive) {
            if (x.Cards.Count != y.Cards.Count) return false;
            // 不存在对应index元素不相等的情况，即相等
            return !x.Cards.Where((t, i) => t != y.Cards[i]).Any();
        }
        
        if (x.RuleList.Count != y.RuleList.Count) return false;
        return !x.RuleList.Where((t, i) => t != y.RuleList[i]).Any();
    }

    public int GetHashCode(CombData obj) {
        return HashCode.Combine(obj.Cards, obj.RuleList);
    }
}

public static class CombTool {
    public static List<CombData> CombDistinct(List<CombData> combs) {
        // combs.RemoveAll(null!); // 如果为null，全部去掉？
        return combs.Distinct(new DistinctCombComparer()).ToList();
    }

    public static void PrintAllCombs(List<CombData> combs) {
        // List<CombData> can't be null?
        if (combs.Count == 0) return;
        GD.Print("[CombList]");
        foreach (var cb in combs) {
            // CardTool.PrintCardDataList(cb.Cards);
            CardTool.PrintCombData(cb);
        }
    }
}