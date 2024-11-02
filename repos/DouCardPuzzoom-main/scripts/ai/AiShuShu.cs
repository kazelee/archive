using System.Collections.Generic;
using System.Linq;
using DouCardPuzzoom.scripts.classes;
using DouCardPuzzoom.scripts.enums;
using DouCardPuzzoom.scripts.manager;
using DouCardPuzzoom.scripts.test;
using DouCardPuzzoom.scripts.tools;

namespace DouCardPuzzoom.scripts.ai; 

public class AiShuShu : IAi {
    public string[] GirlNameArray = new[] { "Madeline", "Girl", "Housekeeper" };
    
    public CombData ChooseToLead(List<CombData> possibleCombs, string name) {
        // 更讨厌小丑，两个小丑（一定会优先出王炸，一个就算了？但麻将没有王炸，md）
        // if (possibleCombs.Any(comb => comb.Types.ContainsKey("rocket") && comb.Types["rocket"] != -1)) { }
        // foreach (var comb in possibleCombs) {
        //     if (comb.IsSuitSensitive) {
        //         
        //     }
        // }
        
        // 优先出掉含有Q和K的牌（想起了自己的父母）
        // 如果单牌都没有就不用看了……
        CombData willLead = null;
        foreach (var cb in possibleCombs) {
            if (cb.IsSuitSensitive) {
                if (cb.Cards.Any(card => card.SuitNum is SuitNums.Joker)) {
                    return cb;
                }
                if (cb.Cards.Any(card => card.PointNum is PointNums.Q or PointNums.K)) {
                    willLead = cb;
                    continue;
                }

                willLead ??= cb; // if null =
                if (cb.Cards.Count > 1) return willLead;
            }
            else {
                var visited = new List<CardData>();
                foreach (var cdStr in cb.RuleList) {
                    var myCards = DebugManager.IsDebugMode ? GameTest.GetWhoseCards(name) : GameLogic.GetWhoseCards(name);
                    foreach (var cd in myCards) {
                        if (cd.PointNum != CardTool.GetPointNumUnsafe(cdStr) || visited.Contains(cd)) continue;
                        visited.Add(cd);
                        break;
                    }
                }

                if (DebugManager.IsDebugMode) {
                    GameTest.CurrentRule.IsCombValid(visited, out var finalComb);
                    if (visited.Any(card => card.SuitNum is SuitNums.Joker)) {
                        return finalComb;
                    }
                    if (visited.Any(card => card.PointNum is PointNums.Q or PointNums.K)) {
                        willLead = finalComb;
                    }

                    willLead ??= finalComb;
                    if (finalComb.Cards.Count > 1) return willLead;
                }
                else {
                    GameLogic.CurrentRule.IsCombValid(visited, out var finalComb);
                    if (visited.Any(card => card.PointNum is PointNums.Q or PointNums.K)) {
                        return finalComb;
                    }

                    willLead ??= finalComb;
                    if (finalComb.Cards.Count > 1) return willLead;
                }
            }
        }

        return willLead;
    }

    public CombData ChooseToFollow(List<CombData> possibleCombs, string name) {
        // Girls help girls
        if (DebugManager.IsDebugMode) {
            // 三人且next turn是女性
            if (GameTest.CurrentLevel.Number == 3 && GirlNameArray.Any(girl => girl == GameTest.NextTurnFirst)) {
                return null;
            }
        }
        else {
            if (GameLogic.CurrentLevel.Number == 3 && GirlNameArray.Any(girl => girl == GameTest.NextTurnFirst)) {
                return null;
            }
        }
        
        // 和出牌的逻辑基本相同
        CombData willLead = null;
        foreach (var cb in possibleCombs) {
            if (cb.IsSuitSensitive) {
                if (cb.Cards.Any(card => card.SuitNum is SuitNums.Joker)) {
                    return cb;
                }
                if (cb.Cards.Any(card => card.PointNum is PointNums.Q or PointNums.K)) {
                    willLead = cb;
                    continue;
                }

                willLead ??= cb; // if null =
                // if (cb.Cards.Count > 1) return willLead;
            }
            else {
                var visited = new List<CardData>();
                foreach (var cdStr in cb.RuleList) {
                    var myCards = DebugManager.IsDebugMode ? GameTest.GetWhoseCards(name) : GameLogic.GetWhoseCards(name);
                    foreach (var cd in myCards) {
                        if (cd.PointNum != CardTool.GetPointNumUnsafe(cdStr) || visited.Contains(cd)) continue;
                        visited.Add(cd);
                        break;
                    }
                }

                if (DebugManager.IsDebugMode) {
                    GameTest.CurrentRule.IsCombValid(visited, out var finalComb);
                    if (visited.Any(card => card.SuitNum is SuitNums.Joker)) {
                        return finalComb;
                    }
                    if (visited.Any(card => card.PointNum is PointNums.Q or PointNums.K)) {
                        willLead = finalComb;
                    }

                    willLead ??= finalComb;
                    if (finalComb.Cards.Count > 1) return willLead;
                }
                else {
                    GameLogic.CurrentRule.IsCombValid(visited, out var finalComb);
                    if (visited.Any(card => card.PointNum is PointNums.Q or PointNums.K)) {
                        return finalComb;
                    }

                    willLead ??= finalComb;
                    // if (finalComb.Cards.Count > 1) return willLead;
                }
            }
        }

        return willLead;
    }
}