using System;
using System.Collections.Generic;
using System.Linq;
using DouCardPuzzoom.scripts.classes;
using DouCardPuzzoom.scripts.enums;
using Godot;

namespace DouCardPuzzoom.scripts.utils;

public static class SuitPointTool {
    
    /// <summary>
    /// 全小写，顺序：黑桃、红心、梅花、方片、小丑
    /// </summary>
    public static string[] SuitNameArray = { "spade", "heart", "club", "diamond", "joker" };
    
    /// <summary>
    /// 字母大写，数字前无 N，顺序：3 -> CJ
    /// </summary>
    public static string[] PointNameArray = { "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A", "2", "BJ", "CJ" };

    /// <summary>
    /// e.g. "heart": SuitNums.Heart (int: 1)
    /// </summary>
    public static Dictionary<string, SuitNums> SuitNumsMap = new() {
        {"spade", SuitNums.Spade}, {"heart", SuitNums.Heart}, {"club", SuitNums.Club},
        {"diamond", SuitNums.Diamond}, {"joker", SuitNums.Joker}
    };

    /// <summary>
    /// e.g. "10": PointNums.N10 (int: 7)
    /// </summary>
    public static Dictionary<string, PointNums> PointNumsMap = new() {
        {"A", PointNums.A}, {"2", PointNums.N2}, {"3", PointNums.N3}, {"4", PointNums.N4}, {"5", PointNums.N5},
        {"6", PointNums.N6}, {"7", PointNums.N7}, {"8", PointNums.N8}, {"9", PointNums.N9}, {"10", PointNums.N10},
        {"J", PointNums.J}, {"Q", PointNums.Q}, {"K", PointNums.K}, {"BJ", PointNums.BJ}, {"CJ", PointNums.CJ}
    };
    
    /// <summary>
    /// e.g. SuitNums.Heart -> "heart"
    /// </summary>
    public static string GetSuitName(SuitNums suitNum) {
        return SuitNameArray[(int)suitNum];
    }
    
    /// <summary>
    /// e.g. PointNums.N10 -> "10"
    /// </summary>
    public static string GetPointName(PointNums pointNum) {
        return PointNameArray[(int)pointNum];
    }
    
    /// <summary>
    /// e.g. "heart" -> SuitNums.Heart (int)
    /// </summary>
    public static SuitNums GetSuitNum(string suitName) {
        return SuitNumsMap[suitName];
    }

    /// <summary>
    /// e.g. "10" -> PointNums.N10 (int)
    /// </summary>
    public static PointNums GetPointNum(string pointName) {
        return PointNumsMap[pointName];
    }
    
    
    /// <summary>
    /// 花色字符串是否合法（在花色字符数组中）
    /// </summary>
    /// <param name="suitName">花色</param>
    /// <returns></returns>
    public static bool IsSuitNameValid(string suitName) {
        // Linq 表达式，判断列表中是否有某个值
        // for i in list: if i is target: return true; return false;
        return SuitNameArray.Any(t => t == suitName);
    }
    
    /// <summary>
    /// 点数字符串是否合法
    /// </summary>
    /// <param name="pointName">点数</param>
    /// <returns></returns>
    public static bool IsPointNameValid(string pointName) {
        // Linq 表达式，判断列表中是否有某个值
        // for i in list: if i is target: return true; return false;
        return PointNameArray.Any(t => t == pointName);
    }
    
}