using System.Collections.Generic;
using System.Linq;
using DouCardPuzzoom.scripts.classes;
using DouCardPuzzoom.scripts.enums;
using DouCardPuzzoom.scripts.manager;
using DouCardPuzzoom.scripts.test;
using DouCardPuzzoom.scripts.tools;
using Godot;

namespace DouCardPuzzoom.scripts.ai; 

// 管家出牌的逻辑就不改了，还是尽可能出长牌和多花色牌，规则挺混沌的
// 反正不影响跟牌，跟牌还是取最小的能跟牌……

public class AiHousekeeper : IAi {
    public CombData ChooseToLead(List<CombData> possibleCombs, string name) {
        // 返回所有牌中长度最大的牌，如果长度一样比较牌型
        // CombTool.PrintAllCombs(possibleCombs);
        
        // possibleCombs 用完就没了，可以直接原地sort
        if (possibleCombs.Count >= 2) {
            possibleCombs.Sort((x, y) =>
                -(x.IsSuitSensitive ? x.Cards.Count : x.RuleList.Count)
                + (y.IsSuitSensitive ? y.Cards.Count : y.RuleList.Count));
        }
        
        CombData willLead = null;
        var biggestRate = 0f;
        foreach (var cb in possibleCombs) {
            if (cb.IsSuitSensitive) {
                var typeList = new List<int>();
                foreach (var cd in cb.Cards) {
                    if (cd.SuitNum != SuitNums.Joker && !typeList.Contains((int)cd.SuitNum)) {
                        typeList.Add((int)cd.SuitNum);
                    }
                }
                
                if (typeList.Count == 0) continue;
                if (cb.Types.ContainsKey("bomb") || cb.Types.Keys.Any(key => key.Split("_").Contains("bomb"))) continue;

                var tmpRate = (float)cb.Cards.Count / typeList.Count;
                if (tmpRate > biggestRate) {
                    biggestRate = tmpRate;
                    willLead = cb;
                }
            }
            else { // rule list
                // 挑的时候就从底往上挑吧…… 复杂的算法没法写，太复杂，而且运算量也太大了
                var visited = new List<CardData>();
                foreach (var cdStr in cb.RuleList) {
                    var myCards = DebugManager.IsDebugMode ? GameTest.GetWhoseCards(name) : GameLogic.GetWhoseCards(name);
                    foreach (var cd in myCards) {
                        if (cd.PointNum != CardTool.GetPointNumUnsafe(cdStr) || visited.Contains(cd)) continue;
                        visited.Add(cd);
                        break;
                    }
                }
                
                var typeList = new List<int>();
                foreach (var cd in visited) {
                    if (cd.SuitNum != SuitNums.Joker && !typeList.Contains((int)cd.SuitNum)) {
                        typeList.Add((int)cd.SuitNum);
                    }
                }

                CombData newComb = null;
                if (DebugManager.IsDebugMode) {
                    GameTest.CurrentRule.IsCombValid(visited, out var finalComb);
                    newComb = finalComb;
                }
                else {
                    GameLogic.CurrentRule.IsCombValid(visited, out var finalComb);
                    newComb = finalComb;
                }
                
                if (typeList.Count == 0) continue;
                if (newComb.Types.ContainsKey("bomb") || newComb.Types.Keys.Any(key => key.Split("_").Contains("bomb"))) continue;

                var tmpRate = (float)newComb.Cards.Count / typeList.Count;
                if (tmpRate > biggestRate) {
                    biggestRate = tmpRate;
                    willLead = newComb;
                }
            }
        }

        if (willLead == null) {
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
        
        return willLead;
    }

    public CombData ChooseToFollow(List<CombData> possibleCombs, string name) {
        // 检查最大牌是不是全部为 K 或 J
        // 得保证和管家打牌的时候，管家没有能打过K和J的牌……
        if (DebugManager.IsDebugMode) {
            if (GameTest.CurrentBiggest.Cards.All(card => card.PointNum is PointNums.J or PointNums.K)) {
                return null;
            }
        }
        else {
            if (GameLogic.CurrentBiggest.Cards.All(card => card.PointNum is PointNums.J or PointNums.K)) {
                return null;
            }
        }
        
        // 检查当前获得牌权者的位置
        if (DebugManager.IsDebugMode) {
            // 上家默认跟，下家获得牌权时过牌（需要考虑三人的情况）
            if (GameTest.CurrentLevel.Number == 3 && GameTest.GetWhoseName("Housekeeper", 2) == GameTest.NextTurnFirst) {
                return null;
            }
        }
        else {
            // 上家默认跟，下家获得牌权时过牌
            if (GameLogic.CurrentLevel.Number == 3 && GameLogic.GetWhoseName("Housekeeper", 2) == GameLogic.NextTurnFirst) {
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