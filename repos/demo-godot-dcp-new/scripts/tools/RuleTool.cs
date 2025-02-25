using System.Collections.Generic;
using System.Linq;
using DouCardPuzzoom.scripts.classes;
using DouCardPuzzoom.scripts.enums;
using DouCardPuzzoom.scripts.rule;
using Godot;

namespace DouCardPuzzoom.scripts.tools; 

// (tmp)DONE

/// <summary>
/// 与Rule有关的工具类（包括rule类型转换，以及字符串获取rule接口）
/// </summary>
public static class RuleTool {
    public static Dictionary<GameModes, string> ModeKeyMap = new Dictionary<GameModes, string>() {
        { GameModes.Normal, "M_NORMAL" }, { GameModes.Rest, "M_REST" }, { GameModes.Blind, "M_BLIND" }
    };
    
    public static Dictionary<string, string> RuleKeyMap = new Dictionary<string, string>() {
        { "RuleOri", "R_ORI" }, { "RuleTwoDeck", "R_TWODECK" }, { "RuleSame", "R_SAME" }, { "RuleSuit", "R_SUIT" },
        { "RuleMahjong", "R_MA" }, { "RuleTexas", "R_TEXAS" }, { "RuleBlackJack", "R_BLACKJACK" },
        { "RuleMath", "R_MATH" }
    };

    public static string GetRuleKey(string ruleName) {
        return RuleKeyMap[ruleName];
    }

    public static string GetModeKey(GameModes modeName) {
        return ModeKeyMap[modeName];
    }
    
    /// <summary>
    /// 自行编写：将cd列表，有损转换成int列表（有排序）
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    public static List<int> Point2Int(List<CardData> cards) {
        var newCards = cards.Select(card => (int)card.PointNum).ToList();
        // 自己写的逻辑，保险起见排个序
        newCards.Sort();
        return newCards;
    }

    /// <summary>
    /// 不需要额外排序？因为传进来的carddata已经排过了？<br/>
    /// 不对，int的类型终究是不对的，还是要重新写一个逻辑，这部分本来应该写在Rule里面的<br/>
    /// 甚至应该设计接口类继承的，这样可以省很多代码很多事情，可惜了……
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    public static List<string> Int2StrList(List<int> cards) {
        // 方法组：替代 card => Func(card)
        return cards.Select(CardTool.GetPointNameUnsafe).ToList();
    }

    public static List<int> RuleList2Int(List<string> list) {
        var newCards = list.Select(card => (int)CardTool.GetPointNumUnsafe(card)).ToList();
        newCards.Sort();
        return newCards;
    }
    
    /// [(heart, A), (spade, J)] -> "A-J"
    public static string CardList2RuleStr(List<CardData> cards) {
        var cardPoints = cards.Select(card => CardTool.GetPointName(card.PointNum)).ToList();
        return string.Join("-", cardPoints);
    }
    
    /// [(heart, A), (spade, J)] -> ["A", "J"]
    public static List<string> CardList2RuleList(List<CardData> cards) {
        return cards.Select(card => CardTool.GetPointName(card.PointNum)).ToList();
    }
    
    /// ["A", "J"] -> "A-J"
    public static string RuleList2RuleStr(List<string> cards) {
        return string.Join("-", cards);
    }
    
    /// "A-J" -> ["A", "J"]
    public static List<string> RuleStr2RuleList(string cards) {
        return new List<string>(cards.Split("-"));
    }

    /// <summary>
    /// 通过字符串返回对应的rule接口类
    /// </summary>
    /// <param name="ruleName"></param>
    /// <returns></returns>
    public static IRule GetRule(string ruleName) {
        switch (ruleName) {
            case "RuleOri": return new RuleOri();
            case "RuleSame": return new RuleSame();
            case "RuleSuit": return new RuleSuit();
            case "RuleTwoDeck": return new RuleTwoDeck();
            case "RuleMahjong": return new RuleMahjong();
            case "RuleMath": return new RuleMath();
            case "RuleBlackJack": return new RuleBlackJack();
            case "RuleTexas": return new RuleTexas();
            default:
                GD.PrintErr($"规则名称 {ruleName} 不合法！");
                return null;
        }
    }
}