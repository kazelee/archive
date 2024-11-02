using System.Collections.Generic;
using DouCardPuzzoom.scripts.classes;
using DouCardPuzzoom.scripts.manager;
using DouCardPuzzoom.scripts.test;
using DouCardPuzzoom.scripts.tools;
using Godot;

namespace DouCardPuzzoom.scripts.ai; 

// 临时写一个 弱智Ai（确保原程序可行的情况下，完成静态数据的保存）
public class AiRobot : IAi {
    public CombData ChooseToLead(List<CombData> possibleCombs, string name) {
        // GD.Print("Lead:");
        // CombTool.PrintAllCombs(possibleCombs);
        // [Test] 弱智AI测试：永远只出列表第一个
        // [Debug] 返回的CombData必须是suit sensitive的！！！
        // return possibleCombs[0];
        if (possibleCombs[0].IsSuitSensitive) {
            return possibleCombs[0];
        }

        var visited = new List<CardData>();
        foreach (var cdStr in possibleCombs[0].RuleList) {
            var myCards = DebugManager.IsDebugMode ? GameTest.GetWhoseCards(name) : GameLogic.GetWhoseCards(name);
            foreach (var cd in myCards) {
                if (cd.PointNum != CardTool.GetPointNumUnsafe(cdStr) || visited.Contains(cd)) continue;
                visited.Add(cd);
                break;
            }
        }

        if (DebugManager.IsDebugMode) {
            GameTest.CurrentRule.IsCombValid(visited, out var finalComb);
            return finalComb;
        }
        else {
            GameLogic.CurrentRule.IsCombValid(visited, out var finalComb);
            return finalComb;
        }
    }

    public CombData ChooseToFollow(List<CombData> possibleCombs, string name) {
        // GD.Print("Follow:");
        // CombTool.PrintAllCombs(possibleCombs);
        // [Test] 弱智AI测试：永远只出列表第一个
        // return possibleCombs[0];
        if (possibleCombs[0].IsSuitSensitive) {
            return possibleCombs[0];
        }

        var visited = new List<CardData>();
        foreach (var cdStr in possibleCombs[0].RuleList) {
            var myCards = DebugManager.IsDebugMode ? GameTest.GetWhoseCards(name) : GameLogic.GetWhoseCards(name);
            foreach (var cd in myCards) {
                if (cd.PointNum != CardTool.GetPointNumUnsafe(cdStr) || visited.Contains(cd)) continue;
                visited.Add(cd);
                break;
            }
        }

        if (DebugManager.IsDebugMode) {
            GameTest.CurrentRule.IsCombValid(visited, out var finalComb);
            return finalComb;
        }
        else {
            GameLogic.CurrentRule.IsCombValid(visited, out var finalComb);
            return finalComb;
        }
    }
}