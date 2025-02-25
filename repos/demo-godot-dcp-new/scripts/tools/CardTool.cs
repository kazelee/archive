using System.Collections.Generic;
using System.Linq;
using DouCardPuzzoom.scripts.classes;
using DouCardPuzzoom.scripts.enums;
using Godot;

namespace DouCardPuzzoom.scripts.tools; 

// DONE

/// <summary>
/// 对单张牌的工具类（不受到任何规则影响）
/// </summary>
public static class CardTool {
    private static readonly string[] SuitNameArray = { "spade", "heart", "club", "diamond", "joker" };
    private static readonly string[] PointNameArray = { "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A", "2", "BJ", "CJ" };
    
    private static readonly Dictionary<string, SuitNums> SuitNumsMap = new() {
        {"spade", SuitNums.Spade}, {"heart", SuitNums.Heart}, {"club", SuitNums.Club},
        {"diamond", SuitNums.Diamond}, {"joker", SuitNums.Joker}
    };
    
    private static readonly Dictionary<string, PointNums> PointNumsMap = new() {
        {"A", PointNums.A}, {"2", PointNums.N2}, {"3", PointNums.N3}, {"4", PointNums.N4}, {"5", PointNums.N5},
        {"6", PointNums.N6}, {"7", PointNums.N7}, {"8", PointNums.N8}, {"9", PointNums.N9}, {"10", PointNums.N10},
        {"J", PointNums.J}, {"Q", PointNums.Q}, {"K", PointNums.K}, {"BJ", PointNums.BJ}, {"CJ", PointNums.CJ}
    };
    
    // 重载一个int的函数？不过不保险，还是换个名字吧
    public static string GetPointNameUnsafe(int point) {
        // point = [0, 14]
        return PointNameArray[point];
    }
    
    public static string GetSuitName(SuitNums suitNum) {
        return SuitNameArray[(int)suitNum];
    }
    
    public static string GetPointName(PointNums pointNum) {
        return PointNameArray[(int)pointNum];
    }
    
    /// e.g. "spade" -> SuitNums.Spade（不安全，需要确保合法）
    public static SuitNums GetSuitNumUnsafe(string suitName) {
        return SuitNumsMap[suitName];
    }
    
    /// e.g. "10" -> PointNums.N10（不安全，需要确保合法）
    public static PointNums GetPointNumUnsafe(string pointName) {
        return PointNumsMap[pointName];
    }
    
    private static bool IsSuitNameValid(string suitName) {
        return SuitNameArray.Any(t => t == suitName);
    }
    
    private static bool IsPointNameValid(string pointName) {
        return PointNameArray.Any(t => t == pointName);
    }
    
    /// <summary>
    /// "spade-A" -> true, out (Spade, A)<br/>
    /// "joker-A" -> false, out null
    /// </summary>
    /// <param name="suitAndPoint">e.g. "spade-A"</param>
    /// <param name="result">输出的CardData结果，false时不使用</param>
    /// <returns></returns>
    public static bool TryLoadCard(string suitAndPoint, out CardData result) {
        var suitPointPair = suitAndPoint.Split("-");
        // 字符串列表必须长度为2，而且都是合法的花色/点数字符串
        if (suitPointPair.Length != 2 || !IsSuitNameValid(suitPointPair[0]) || !IsPointNameValid(suitPointPair[1])) {
            GD.PrintErr($"读取 {suitAndPoint} 到CardData失败：字符串不合法！");
            result = null;
            return false;
        }
        
        var suitNum = GetSuitNumUnsafe(suitPointPair[0]);
        var pointNum = GetPointNumUnsafe(suitPointPair[1]);
        
        // Joker 只能有 BJ/CJ 两种点数；反之，BJ/CJ 花色必须是 Joker
        if (suitNum == SuitNums.Joker && pointNum is not (PointNums.BJ or PointNums.CJ) ||
            suitNum != SuitNums.Joker && pointNum is PointNums.BJ or PointNums.CJ) {
            GD.PrintErr($"读取 {suitAndPoint} 到CardData失败：花色点数不匹配！");
            result = null;
            return false;
        }

        result = new CardData(suitNum, pointNum);
        return true;
    }

    /// <summary>
    /// 便于打印card data list
    /// </summary>
    /// <param name="cardDatas"></param>
    /// <param name="name">备用名称，默认为空</param>
    public static void PrintCardDataList(List<CardData> cardDatas, string name = "") {
        var tmp = name == "" ? $"{name}: " : "";
        foreach (var cd in cardDatas) {
            tmp += $"{cd} ";
        }
        GD.Print(tmp);
    }

    public static void PrintCombData(CombData combData) {
        if (combData == null) return;
        if (combData.IsSuitSensitive) {
            PrintCardDataList(combData.Cards);
        }
        else {
            var tmp = "";
            foreach (var card in combData.RuleList) {
                tmp += $"{card} ";
            }
            GD.Print(tmp);
        }
    }
    
}