using System.Collections.Generic;
using System.Linq;
using DouCardPuzzoom.scripts.classes;
using DouCardPuzzoom.scripts.manager;
using DouCardPuzzoom.scripts.test;
using DouCardPuzzoom.scripts.tools;

namespace DouCardPuzzoom.scripts.ai; 

public class AiWangXiang : IAi{
    public CombData ChooseToLead(List<CombData> possibleCombs, string name) {
        if (possibleCombs.Count >= 2) {
            possibleCombs.Sort((x, y) =>
                -(x.IsSuitSensitive ? x.Cards.Count : x.RuleList.Count)
                + (y.IsSuitSensitive ? y.Cards.Count : y.RuleList.Count));
        }

        foreach (var variPossibleComb in possibleCombs) {
            if (variPossibleComb.Types.ContainsKey("bomb") ||
                variPossibleComb.Types.Keys.Any(key => key.Split("_").Contains("bomb"))) {
                
            }
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
        throw new System.NotImplementedException();
    }
}