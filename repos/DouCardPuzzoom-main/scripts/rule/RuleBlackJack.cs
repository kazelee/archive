using System.Collections.Generic;
using System.Linq;
using DouCardPuzzoom.scripts.classes;
using DouCardPuzzoom.scripts.enums;
using DouCardPuzzoom.scripts.tools;
using Godot;
using Godot.Collections;

using SCG = System.Collections.Generic;
namespace DouCardPuzzoom.scripts.rule; 

public class RuleBlackJack : IRule {
    // 实例化本类后，通过DataLoader获取数据
    public readonly Dictionary Data = DataLoader.RuleBlackJackData;
    public readonly Dictionary TypeCards = DataLoader.RuleBlackJackTypeCards;

    public SCG.Dictionary<string, int> PointWeightMap = new SCG.Dictionary<string, int>() {
        { "A", 0 }, { "2", 1 }, { "3", 2 }, { "4", 3 }, { "5", 4 }, { "6", 5 }, { "7", 6 }, { "8", 7 }, { "9", 8 },
        { "10", 9 }, { "J", 10 }, { "Q", 11 }, { "K", 12 }, { "BJ", 13 }, { "CJ", 14 }
    };

    public string[] PointNames = new[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "BJ", "CJ" };

    public List<int> RuleList2Int(List<string> ruleList) {
        return ruleList.Select(ruleStr => PointWeightMap[ruleStr]).ToList();
    }
    
    public int GetPointWeight(PointNums pointNums) {
        switch (pointNums) {
            case PointNums.A: 
            case PointNums.N2:
                return (int)pointNums - 11;
            case PointNums.CJ:
            case PointNums.BJ:
                return (int)pointNums;
            default:
                return (int)pointNums + 2;
        }
    }
    
    // GetAllComb 和 CanFollow 返回的列表必须 不包含 需要二次筛选的项目！
    // 3 - CJ: 0 - 14 (0 - 12, 13, 14)
    
    public List<CombData> GetAllComb(List<CardData> myCardsInHand) {
        // 正常流程：逐一算牌
        var cardsRule = RuleTool.CardList2RuleList(myCardsInHand);
        return GetAllCombOri(cardsRule);
    }

    // 编写一个针对RuleList的版本
    public List<CombData> GetAllCombOri(List<string> cardsRule) {
        var cardsInt = RuleList2Int(cardsRule); // 使用针对规则改写的
        var cardsMap = Int2CardMap(cardsInt);

        var combList = new List<CombData>(); // { new CombData(new List<string>(), new SCG.Dictionary<string, int>()) };
        
        for (int i = 1; i <= 5; i++) {
            var newList = cardsInt.ToList();
            newList.Sort();
            var allPossLists = CombineTool.GetCombinations(newList.ToList(), i);
            foreach (var possList in allPossLists) {
                if (IntList2CombData(possList) != null) {
                    combList.Add(IntList2CombData(possList));
                }
            }
        }

        return combList;
    }
    
    public bool IsCombValid(List<CardData> tryLeadCards, out CombData toLeadComb) {
        var cards = RuleTool.CardList2RuleList(tryLeadCards);
        // 这里的排序针对点数，最终封装的还是原参数 tryLeadCards
        var sortedCards = SortCards(cards);
        var cardsStr = RuleTool.RuleList2RuleStr(sortedCards);
        // 合法的情况：将 Godot Array 转成 C# Dictionary 格式
        if (Data.TryGetValue(cardsStr, out var value)) {
            var localTypes = new SCG.Dictionary<string, int>();
            // value: [["pair", 4], ["xxx", n], ...]
            // typeAndWeight: ["pair", 4]
            foreach (var typeAndWeight in (Array)value) {
                var tmpType = (string)((Array)typeAndWeight)[0];
                var tmpWeight = (int)((Array)typeAndWeight)[1];
                // 【补充】JSON包括同type多权值，但①情况仅限于三带四带且差值无所谓；②字典覆盖使得自动获得最大值
                localTypes[tmpType] = tmpWeight;
            }

            // 封装的时候就排序，使得comb一定按照当前rule的出牌画面排序
            SortToLead(tryLeadCards);
            toLeadComb = new CombData(tryLeadCards, localTypes);

            // // [Debug] 打印类型
            // var tmp = "[Types] ";
            // foreach (var kv in toLeadComb.Types) {
            //     // GD.Print($"{kv.Key}: {kv.Value}");
            //     tmp += $"{kv.Key}: {kv.Value}; ";
            // }
            // GD.Print(tmp);
            
            return true;
        }
  
        toLeadComb = null;
        return false;
    }

    public bool CanBeat(CombData comb1, CombData comb2) {
        // types: {"xxx": 10, "yyy": 5}, {"yyy": 6}
        var types1 = comb1.Types;
        var types2 = comb2.Types;
        
        // 【补充】原版规则下炸弹只能是炸弹，只要对手牌的某一个type能压，就能压
        foreach (var kv in types2) {
            if (types1.TryGetValue(kv.Key, out var weight) && weight > kv.Value) {
                return true;
            }
        }

        return false;
    }

    public bool CanFollow(CombData target, List<CardData> myCardsInHand, out List<CombData> possibleCombs) {
        var myCardList = RuleTool.CardList2RuleList(myCardsInHand);
        if (CanFollowOri(target, myCardList, out var pcs)) {
            possibleCombs = pcs;
            return true;
        }

        possibleCombs = null;
        return false;
    }
    
    // 保留原版的的函数
    public bool CanFollowOri(CombData target, List<string> myCardList, out List<CombData> possibleCombs) {
        // var targetTypes = target.Types;

        var myCardStr = RuleTool.RuleList2RuleStr(myCardList);
        // GD.PrintErr(target == null);
        // 必须使用深度拷贝！！！
        var targetTypes = target.Types.ToDictionary(kv => kv.Key, kv => kv.Value);
        
        var myCardMap = Str2CardMap(myCardStr);
        
        // 该部分逻辑与废弃代码类似，使用 Godot 数据结构
        // cardsGt: {"trio_solo": ["A-A-A-5"], "bomb": ["A-A-A-A"]}
        var cardsGt = new Dictionary();
        foreach (var kv in targetTypes) {
            var weightGt = new List<string>();
            // targetTypes是封装好的CombData中的，无需判断合法，直接调用
            // w = weight，
            foreach (var w in ((Dictionary)TypeCards[kv.Key]).Keys) {
                var wStr = (string)w;
                if (wStr.ToInt() > kv.Value) {
                    weightGt.Add(wStr);
                }

                // 对方牌type不存在，就先设置为 空array
                if (!cardsGt.ContainsKey(kv.Key)) {
                    cardsGt[kv.Key] = new Array();
                }
            }
            weightGt.Sort((x, y) => x.ToInt().CompareTo(y.ToInt()));
            
            foreach (var wStr in weightGt) {
                foreach (var wc in (Array)((Dictionary)TypeCards[kv.Key])[wStr]) {
                    var wCards = (string)wc;
                    var wCardmap = Str2CardMap(wCards);
                    if (CardsContain(myCardMap, wCardmap)
                        && !((Array)cardsGt[kv.Key]).Contains(wCards)) {
                        ((Array)cardsGt[kv.Key]).Add(wCards);
                    }
                } 
            }
            
            if (((Array)cardsGt[kv.Key]).Count == 0) {
                cardsGt.Remove(kv.Value);
            }
        }

        // 一个能打的都没有
        if (cardsGt.Count == 0) {
            possibleCombs = new List<CombData>();
            return false;
        }
        
        // return cardsGt;
        
        // 将 cardsGt 转换成 List<CombData>
        // 【待办】这部分逻辑需要优化，暂时搁置（无限期搁置……）
        possibleCombs = new List<CombData>();
        foreach (var kv in cardsGt) {
            foreach (var cardsStr in (Array)kv.Value) {
                // cardsStr就是通过TypeCards得到的，无需再检查
                // 这里直接借用 IsCombValid 的代码
                if (Data.TryGetValue(cardsStr, out var value)) {
                    var localTypes = new SCG.Dictionary<string, int>();
                    // value: [["pair", 4], ["xxx", n], ...]
                    // typeAndWeight: ["pair", 4]
                    foreach (var typeAndWeight in (Array)value) {
                        var tmpType = (string)((Array)typeAndWeight)[0];
                        var tmpWeight = (int)((Array)typeAndWeight)[1];
                        // 【补充】JSON包括同type多权值，但①情况仅限于三带四带且差值无所谓；②字典覆盖使得自动获得最大值
                        localTypes[tmpType] = tmpWeight;
                    }
                    // 去除-1的简便情况（实为想保留canBeat纯净性的无奈之举）

                    // 注：List<string>的排序遵从Rule字典排序，即完全依赖点数大小
                    var cards = RuleTool.RuleStr2RuleList((string)cardsStr);
                    possibleCombs.Add(new CombData(cards, localTypes));
                }
            }
        }

        // 再检查一遍，有备无患
        if (possibleCombs.Count == 0) {
            return false;
        }
        
        return true;
    }
    
    public void SortInHand(List<CardData> cardsInHand) {
        // 1. Point 小的在前；2. Suit 小的在前；3. 大小王在最后面
        // 直接比较 PointNum * 100 + SuitNum 即可
        cardsInHand.Sort((x, y) => 
            (100 * GetPointWeight(x.PointNum) + (int)x.SuitNum) - 
            (100 * GetPointWeight(y.PointNum) + (int)y.SuitNum));
    }

    public void SortToLead(List<CardData> combToLead, SCG.Dictionary<string, int> type = null) {
        SortInHand(combToLead);
    }
    
    // ------ 内部函数部分 ------

    /// <summary>
    /// 计数，相当于 Python 的 Counter
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    public SCG.Dictionary<int, int> Int2CardMap(List<int> cards) {
        var cardmap = new SCG.Dictionary<int, int>();
        foreach (var card in cards) {
            if (cardmap.ContainsKey(card)) {
                cardmap[card] += 1;
            }
            else {
                cardmap[card] = 1;
            }
        }

        return cardmap;
    }

    public List<string> Int2StrList(List<int> cards) {
        var res = new List<string>();
        foreach (var cardInt in cards) {
            res.Add(PointNames[cardInt]);
        }

        return res;
    }
    
    /// <summary>
    /// 封装intList得到Combdata，无花色的strList！理论上不会为空<br/>
    /// 不必轻易调用，理论上只有极其复杂的三连顺才有多types的可能……
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    public CombData IntList2CombData(List<int> cards) {
        var strList = Int2StrList(cards); // 使用自己的函数！！！！！
        // 借用 isCombValid 的代码（不能直接用，因为有 toLeadCards)
        
        // 这里的排序针对点数，最终封装的还是原参数 tryLeadCards
        var sortedCards = SortCards(strList);
        var cardsStr = RuleTool.RuleList2RuleStr(sortedCards);
        // 合法的情况：将 Godot Array 转成 C# Dictionary 格式
        if (Data.TryGetValue(cardsStr, out var value)) {
            var localTypes = new SCG.Dictionary<string, int>();
            // value: [["pair", 4], ["xxx", n], ...]
            // typeAndWeight: ["pair", 4]
            foreach (var typeAndWeight in (Array)value) {
                var tmpType = (string)((Array)typeAndWeight)[0];
                var tmpWeight = (int)((Array)typeAndWeight)[1];
                // 【补充】JSON包括同type多权值，但①情况仅限于三带四带且差值无所谓；②字典覆盖使得自动获得最大值
                localTypes[tmpType] = tmpWeight;
            }

            // 注意，这里是没有花色的！！！
            return new CombData(strList, localTypes);
        }

        // 理论上不会到达这里
        return null;
    }
    
    /// 计数："A-A-J" -> {"A": 2, "J": 1}
    public SCG.Dictionary<string, int> Str2CardMap(string str) {
        var cards = RuleTool.RuleStr2RuleList(str);
        var cardmap = new SCG.Dictionary<string, int>();
        foreach (var card in cards) {
            if (cardmap.ContainsKey(card)) {
                cardmap[card] += 1;
            }
            else {
                cardmap[card] = 1;
            }
        }

        return cardmap;
    }
    
    /// mine: {"A": 3, "10": 2}, others: {"K": 3, "5": 1} => return true;
    public bool CardsContain(SCG.Dictionary<string, int> candidateCardmap, SCG.Dictionary<string, int> cardmap) {
        foreach (var kv in cardmap) {
            // 从自己的牌数字典中获取这个点数，如果没有就过
            if (!candidateCardmap.TryGetValue(kv.Key, out var value)) {
                return false;
            }
            // 如果有这个点数，但数量不够，过
            if (value < kv.Value) {
                return false;
            }
        }

        return true;
    }
    
    /// 排序：["A", "J"] -> ["J", "A"]
    public List<string> SortCards(List<string> cards) {
        // (x, y): x < y
        cards.Sort((x, y) => PointWeightMap[x].CompareTo(PointWeightMap[y]));
        return cards;
    }
}