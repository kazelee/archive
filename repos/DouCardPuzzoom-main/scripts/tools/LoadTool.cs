using System.Collections.Generic;
using System.IO;
using System.Linq;
using DouCardPuzzoom.scripts.classes;
using DouCardPuzzoom.scripts.enums;
using Godot;
using Godot.Collections;

namespace DouCardPuzzoom.scripts.tools; 

public static class LoadTool {
    // private static readonly string[] ValidLevelName = {
    //     // 暂时不做检查
    // };
    
    private static readonly string[] ValidRuleNameArray = {
        "RuleOri", "RuleTwoDeck", "RuleSame", "RuleSuit", "RuleMahjong", "RuleBlackJack", "RuleMath",
        "RuleHeart", "RuleKingdom", "RuleTexas"
    };

    private static readonly string[] ValidAiNameArray = {
        "YOU", "Landlord", "Robot", "Housekeeper", "ShuShu", "Madeline", "WangXiang", "Albert", "Student",
        "Geek", "Thief", "Girl"
    };
    
    /// <summary>
    /// 读取关卡文件（Json），使用 Godot 动态读取方法
    /// </summary>
    /// <param name="levelPath">关卡文件的路径，Godot格式，形如"res://levels/example.json"</param>
    /// <param name="levelInfo">引用关卡数据</param>
    public static bool TryLoadLevel(string levelPath, ref LevelInfo levelInfo) {
        var dataStr = File.ReadAllText(ProjectSettings.GlobalizePath(levelPath));
        var currentLevel = Json.ParseString(dataStr).AsGodotDictionary();

        levelInfo = new LevelInfo {
            Name = (string)currentLevel["name"],
            Id = (string)currentLevel["id"],
            Number = (int)currentLevel["number"],
            Landlord = (string)currentLevel["landlord"],
            Rule = (string)currentLevel["rule"],
            PassLimit = (int)currentLevel["pass_limit"],
            LeadLimit = (int)currentLevel["lead_limit"],
            PassRequest = (int)currentLevel["pass_request"],
            LeadRequest = (int)currentLevel["lead_request"]
        };

        var mode = (string)currentLevel["mode"];
        switch (mode) {
            case "Normal": levelInfo.Mode = GameModes.Normal; break;
            case "Rest": levelInfo.Mode = GameModes.Rest; break;
            case "Blind": levelInfo.Mode = GameModes.Blind; break;
            default:
                GD.PrintErr($"读取关卡文件错误：不合法的模式字符串 {mode}");
                return false;
        }

        if (levelInfo.Number != 2 && levelInfo.Number != 3) {
            GD.PrintErr("读取关卡文件错误：人数只能为2-3人");
            return false;
        }
        
        // 由于 Godot 和 C# 数据结构不能互转，只有 string 类型互通，所以将 Array 的每项元素深拷贝到 List 中
        
        var playerArray = (Array)currentLevel["players"];
        if (playerArray.Count != levelInfo.Number) {
            GD.PrintErr("读取关卡文件错误：玩家列表长度应与玩家数相同");
            return false;
        }
        
        levelInfo.Players = new List<string>();
        foreach (var player in playerArray) {
            levelInfo.Players.Add((string)player);
        }

        // DONE: Players中的字符串必须在内容的文件中登记过
        foreach (var player in levelInfo.Players) {
            if (ValidAiNameArray.All(t => t != player)) {
                GD.PrintErr($"读取关卡文件错误：Players 中 {player} 不合法！");
                return false;
            }
        }
        
        // DONE: Rule中的字符串必须在内容的文件中登记过
        var info = levelInfo;
        if (ValidRuleNameArray.All(t => t != info.Rule)) {
            GD.PrintErr($"读取关卡文件错误：规则 {info.Rule} 不合法！");
            return false;
        }
        
        // Landlord 字符串不合法（不在Player列表中）
        if (!levelInfo.Players.Contains(levelInfo.Landlord)) {
            GD.PrintErr($"读取关卡文件错误：{levelInfo.Landlord} 不在 Players 中");
            return false;
        }

        // 玩家（YOU）不在玩家列表中
        if (!levelInfo.Players.Contains("YOU")) {
            GD.PrintErr($"读取关卡文件错误：Players 中没有 YOU");
            return false;
        }
        
        var cardsArray = (Array)currentLevel["cards"];
        if (playerArray.Count != levelInfo.Number) {
            GD.PrintErr("读取关卡文件错误：手牌列表长度应与玩家数相同");
            return false;
        }
        
        levelInfo.Cards = new List<List<CardData>>();
        foreach (var cards in cardsArray) {
            var newCards = new List<CardData>();
            foreach (var card in (Array)cards) {
                if (!CardTool.TryLoadCard((string)card, out CardData result)) {
                    GD.PrintErr("读取关卡文件警告：手牌列表中含有不合法字符串");
                    continue;
                }
                newCards.Add(result);
            }
            levelInfo.Cards.Add(newCards);
        }

        // [Test] 打印输出读取的信息（有toString函数）
        GD.Print($"读取关卡文件成功！\n{levelInfo}");
        
        return true;
    }
}