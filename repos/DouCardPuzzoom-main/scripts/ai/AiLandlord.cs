using System.Collections.Generic;
using System.Linq;
using DouCardPuzzoom.scripts.classes;
using DouCardPuzzoom.scripts.enums;
using DouCardPuzzoom.scripts.manager;
using DouCardPuzzoom.scripts.test;
using DouCardPuzzoom.scripts.tools;

namespace DouCardPuzzoom.scripts.ai; 

public class AiLandlord : IAi{
    public CombData ChooseToLead(List<CombData> possibleCombs, string name) {
        // 返回所有牌中长度最大的牌，如果长度一样比较牌型
        // CombTool.PrintAllCombs(possibleCombs);
        
        if (possibleCombs.Count >= 2) {
            possibleCombs.Sort((x, y) =>
                -(x.IsSuitSensitive ? x.Cards.Count : x.RuleList.Count)
                + (y.IsSuitSensitive ? y.Cards.Count : y.RuleList.Count));
        }
        
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
        // 检查对手的牌里有没有红心6（直接访问GameLogic）
        if (DebugManager.IsDebugMode) {
            // if (GameTest.CurrentBiggest.Cards.Contains(new CardData(SuitNums.Heart, PointNums.N6))) {
            //     return null;
            // }
            // Contains 对于对象的判等必须是同一地址？又要写判断器？
            if (GameTest.CurrentBiggest.Cards.Any(card =>
                    card.SuitNum == SuitNums.Heart && card.PointNum == PointNums.N6)) {
                return null;
            }
        }
        else {
            if (GameLogic.CurrentBiggest.Cards.Any(card =>
                    card.SuitNum == SuitNums.Heart && card.PointNum == PointNums.N6)) {
                return null;
            }
        }
        
        // 检查是不是队友的出牌
        if (DebugManager.IsDebugMode) {
            // 房东不是地主，而且当前获得牌权的人也不是（只可能是三人的时候，无所谓）
            if (GameTest.CurrentLevel.Landlord != "Landlord" && GameTest.CurrentLevel.Landlord != GameTest.NextTurnFirst) {
                return null;
            }
        }
        else {
            // 房东不是地主，而且当前获得牌权的人也不是（只可能是三人的时候，无所谓）
            if (GameLogic.CurrentLevel.Landlord != "Landlord" && GameLogic.CurrentLevel.Landlord != GameLogic.NextTurnFirst) {
                return null;
            }
        }
        
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