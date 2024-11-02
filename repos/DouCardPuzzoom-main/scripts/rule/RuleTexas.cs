using System.Collections.Generic;
using System.Linq;
using DouCardPuzzoom.scripts.classes;
using DouCardPuzzoom.scripts.enums;
using DouCardPuzzoom.scripts.tools;
using Godot;
using Godot.Collections;

using SCG = System.Collections.Generic;
namespace DouCardPuzzoom.scripts.rule; 

public class RuleTexas : IRule {
    // 实例化本类后，通过DataLoader获取数据
    public readonly Dictionary Data = DataLoader.RuleTexasData;
    public readonly Dictionary TypeCards = DataLoader.RuleTexasTypeCards;
    public readonly Dictionary FlushData = DataLoader.RuleTexasFlushData;
    public readonly Dictionary FlushTypeCards = DataLoader.RuleTexasFlushTypeCards;

    public SCG.Dictionary<string, int> PointWeightMap = new SCG.Dictionary<string, int>() {
        { "A", 12 }, { "2", 0 }, { "3", 1 }, { "4", 2 }, { "5", 3 }, { "6", 4 }, { "7", 5 }, { "8", 6 }, { "9", 7 },
        { "10", 8 }, { "J", 9 }, { "Q", 10 }, { "K", 11 }, { "BJ", 13 }, { "CJ", 14 }
    };

    public string[] PointNames = new[] { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A", "BJ", "CJ" };

    public readonly string[] BombTypes = new[] { "solo", "pair", "two_pair", "trio", "straight", "flush", "house", "four", "flush_straight" };
    
    public readonly SCG.Dictionary<string, int> BombWeights = new SCG.Dictionary<string, int>() {
        {"solo", 0}, {"pair", 1}, {"two_pair", 2}, {"trio", 3}, {"straight", 4}, {"flush", 5}, {"house", 6}, {"four", 7},
        {"flush_straight", 8}
    };
    
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
        
        // 不能用RuleOri，因为需要用到花色（本来就是递归失败的历史遗留问题……）
        
        var cardsInt = RuleList2Int(cardsRule); // 使用针对规则改写的
        var cardsMap = Int2CardMap(cardsInt);

        var combList = new List<CombData>(); // { new CombData(new List<string>(), new SCG.Dictionary<string, int>()) };
        
        var soloList = new List<int>();
        var pairList = new List<int>();
        var trioList = new List<int>();
        var bombList = new List<int>();
        
        // 1.1 单张
        foreach (var kv in cardsMap) {
            if (kv.Value >= 1) {
                soloList.Add(kv.Key);
                var intList = new List<int> { kv.Key };
                combList.Add(IntList2CombData(intList));
            }

            if (kv.Value >= 2 && kv.Key != 13 && kv.Key != 14) { 
                pairList.Add(kv.Key);
                combList.Add(IntList2CombData(new List<int> {kv.Key, kv.Key}));
            }

            if (kv.Value >= 3 && kv.Key != 13 && kv.Key != 14) {
                trioList.Add(kv.Key);
            }

            if (kv.Value >= 4 && kv.Key != 13 && kv.Key != 14) { 
                bombList.Add(kv.Key);
            }
        }

        // 两对
        // 直接使用最底层的代码进行“半角矩阵”优化
        for (int i = 0; i < pairList.Count; i++) {
            for (int j = i + 1; j < pairList.Count; j++) {
                combList.Add(IntList2CombData(new List<int>() {pairList[i], pairList[i], pairList[j], pairList[j]}));
            }
        }

        // 三张
        foreach (var trio in trioList) {
            combList.Add(IntList2CombData(new List<int>() {trio, trio, trio}));
        }
        
        // 顺子（非同花）
        CreateStraight(cardsInt);

        // 同花（不含顺子，但顺子已经返回了）
        var cardsDiamond = myCardsInHand.Where(card => card.SuitNum == SuitNums.Diamond).ToList();
        var diamondStraights = GetAllFlush(cardsDiamond, combList);
        var cardsClub = myCardsInHand.Where(card => card.SuitNum == SuitNums.Club).ToList();
        var clubStraights = GetAllFlush(cardsClub, combList);
        var cardsHeart = myCardsInHand.Where(card => card.SuitNum == SuitNums.Heart).ToList();
        var heartStraights = GetAllFlush(cardsHeart, combList);
        var cardsSpade = myCardsInHand.Where(card => card.SuitNum == SuitNums.Spade).ToList();
        var spadeStraights = GetAllFlush(cardsSpade, combList);
        
        // 葫芦
        foreach (var trio in trioList) {
            foreach (var pair in pairList) {
                if (trio == pair) continue;
                combList.Add(IntList2CombData(new List<int>(){trio, trio, trio, pair, pair}));
            }
        }
        
        // 四张
        foreach (var bomb in bombList) {
            combList.Add(IntList2CombData(new List<int>() {bomb, bomb, bomb, bomb}));
        }

        // 同花顺
        combList.AddRange(diamondStraights);
        combList.AddRange(clubStraights);
        combList.AddRange(heartStraights);
        combList.AddRange(spadeStraights);
        
        return CombTool.CombDistinct(combList);
    }

    private List<CombData> GetAllFlush(List<CardData> cards, List<CombData> combList, bool isStraight = false) {
        var cardsRule = RuleTool.CardList2RuleList(cards);
        var cardsInt = RuleList2Int(cardsRule);
        var newList = cardsInt.ToList();
        newList.Sort();
        var tmpList = new List<CombData>();
        var allPossLists = CombineTool.GetCombinations(newList.ToList(), 5);
        foreach (var possList in allPossLists) {
            // 同花必须不能为顺子……反而是为null的时候？
            var combData = IntList2CombData(possList, true);
            if (combData == null) continue; // 理论上不会出现的
            
            var visited = new List<CardData>();
            foreach (var cdStr in combData.RuleList) {
                foreach (var cd in cards) {
                    if (cd.PointNum != CardTool.GetPointNumUnsafe(cdStr) || visited.Contains(cd)) continue;
                    visited.Add(cd);
                    break;
                }
            }

            if (combData.Types.ContainsKey("straight")) {
                tmpList.Add(new CombData(visited, combData.Types));
            }
            else {
                combList.Add(new CombData(visited, combData.Types));
            }
        }

        return tmpList;
    }
    
    private List<List<int>> CreateStraight(List<int> list) {
        var newList = list.Where(card => card <= 12).ToList();
        newList.Sort();
        var len = newList.Count;
        if (len < 5) return new List<List<int>>(); // null
        
        var possIntLists = new List<List<int>>(); 
        // 滑动窗口 [0, min - 1]
        for (int beg = 0; beg < len - 4; beg++) {
            // for (int end = beg + 3 - 1; end < len; end++) {
            if (newList[beg + 4] - newList[beg] != 4) break;
            var tmpList = new List<int>(); // 只记录一个
            for (int i = beg; i <= beg + 4; i++) {
                tmpList.Add(newList[i]);
            }

            // combList.Add(IntList2CombData(tmpList));
            possIntLists.Add(tmpList);
            // }
        }

        return possIntLists;
    }
    
    public bool IsCombValid(List<CardData> tryLeadCards, out CombData toLeadComb) {
        if (tryLeadCards == null || tryLeadCards.Count == 0) {
            toLeadComb = null;
            return false;
        }

        var cards = RuleTool.CardList2RuleList(tryLeadCards);
        var sortedCards = SortCards(cards);
        var cardsStr = RuleTool.RuleList2RuleStr(sortedCards);
        
        var suitList = new List<int>();
        foreach (var cd in tryLeadCards) {
            if (cd.SuitNum == SuitNums.Joker) {
                suitList = new List<int>(); // 同花有王就不行
                break;
            }
            if (suitList.Contains((int)cd.SuitNum)) continue;
            suitList.Add((int)cd.SuitNum);
        }

        // 同花的情况
        if (tryLeadCards.Count == 5 && suitList.Count == 1) {
            if (FlushData.TryGetValue(cardsStr, out var value)) {
                var localTypes = new SCG.Dictionary<string, int>();

                foreach (var typeAndWeight in (Array)value) {
                    var tmpType = (string)((Array)typeAndWeight)[0];
                    var tmpWeight = (int)((Array)typeAndWeight)[1];
                    // 【补充】JSON包括同type多权值，但①情况仅限于三带四带且差值无所谓；②字典覆盖使得自动获得最大值
                    localTypes[tmpType] = tmpWeight;
                }

                SortToLead(tryLeadCards);
                toLeadComb = new CombData(tryLeadCards, localTypes);
            
                return true;
            }
        }
        else {
            if (Data.TryGetValue(cardsStr, out var value)) {
                var localTypes = new SCG.Dictionary<string, int>();

                foreach (var typeAndWeight in (Array)value) {
                    var tmpType = (string)((Array)typeAndWeight)[0];
                    var tmpWeight = (int)((Array)typeAndWeight)[1];
                    // 【补充】JSON包括同type多权值，但①情况仅限于三带四带且差值无所谓；②字典覆盖使得自动获得最大值
                    localTypes[tmpType] = tmpWeight;
                }

                SortToLead(tryLeadCards);
                toLeadComb = new CombData(tryLeadCards, localTypes);
            
                return true;
            }
        }
  
        toLeadComb = null;
        return false;
    }

    public bool CanBeat(CombData comb1, CombData comb2) {
        // types: {"xxx": 10, "yyy": 5}, {"yyy": 6}
        var types1 = comb1.Types;
        var types2 = comb2.Types;
        
        // 都是“炸弹”
        foreach (var kv in types2) {
            foreach (var kv1 in types1) {
                if (BombWeights[kv1.Key] > BombWeights[kv.Key]) {
                    return true;
                } 
                if (BombWeights[kv1.Key] == BombWeights[kv.Key]) {
                    return kv1.Value > kv.Value;
                }
            }
        }

        return false;
    }

    public void TryAddType(SCG.Dictionary<string, int> targetTypes) {
        // 如果大的牌型存在，就不应该允许添加小的牌型
        // 否则，小炸弹就能打大炸弹了……
        if (!targetTypes.TryAdd("flush_straight", -1)) return;
        if (!targetTypes.TryAdd("four", -1)) return;
        if (!targetTypes.TryAdd("house", -1)) return;
        
        if (!targetTypes.TryAdd("flush", -1)) return;
        if (!targetTypes.TryAdd("straight", -1)) return;
        
        if (!targetTypes.TryAdd("trio", -1)) return;
        if (!targetTypes.TryAdd("two_pair", -1)) return;
        if (!targetTypes.TryAdd("pair", -1)) return;
        if (!targetTypes.TryAdd("solo", -1)) return;
    }

    private void TryRemove(SCG.Dictionary<string, int> localTypes, string name) {
        if (localTypes.Contains(new KeyValuePair<string, int>(name, -1))) {
            localTypes.Remove(name);
        }
    }
    
    private List<CombData> AddFollowForSuit(SCG.Dictionary<string, int> targetTypes, List<CardData> cardsInHand, bool isFlush = false) {
        var myCardList = RuleTool.CardList2RuleList(cardsInHand);
        var myCardStr = RuleTool.RuleList2RuleStr(myCardList);
        var myCardMap = Str2CardMap(myCardStr);
        
        var cardsGt = new Dictionary();
        var typeCards = new Dictionary();
        foreach (var kv in targetTypes) {
            // 先考虑不是同花的情况
            if (!isFlush) {
                if (kv.Key == "flush" || kv.Key == "flush_straight") continue;
                typeCards = TypeCards;
            }
            else {
                if (kv.Key != "flush" && kv.Key != "flush_straight") continue;
                typeCards = FlushTypeCards;
            }
            
            var weightGt = new List<string>();
            // targetTypes是封装好的CombData中的，无需判断合法，直接调用
            // w = weight，
            foreach (var w in ((Dictionary)typeCards[kv.Key]).Keys) {
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
                foreach (var wc in (Array)((Dictionary)typeCards[kv.Key])[wStr]) {
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
        
        var possibleCombs = new List<CombData>();
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
                    foreach (var name in BombTypes) {
                        TryRemove(localTypes, name);
                    }
                    // 注：List<string>的排序遵从Rule字典排序，即完全依赖点数大小
                    var cards = RuleTool.RuleStr2RuleList((string)cardsStr);
                    possibleCombs.Add(new CombData(cards, localTypes));
                }
            }
        }

        return possibleCombs;
    }
    
    public bool CanFollow(CombData target, List<CardData> myCardsInHand, out List<CombData> possibleCombs) {
        // 也合并ORI（历史遗留问题）
        var targetTypes = target.Types.ToDictionary(kv => kv.Key, kv => kv.Value);
        TryAddType(targetTypes);

        possibleCombs = new List<CombData>();
        possibleCombs.AddRange(AddFollowForSuit(targetTypes, myCardsInHand));

        var diamondList = myCardsInHand.Where(card => card.SuitNum == SuitNums.Diamond).ToList();
        possibleCombs.AddRange(AddFollowForSuit(targetTypes, diamondList, true));
        var clubList = myCardsInHand.Where(card => card.SuitNum == SuitNums.Club).ToList();
        possibleCombs.AddRange(AddFollowForSuit(targetTypes, clubList, true));
        var heartList = myCardsInHand.Where(card => card.SuitNum == SuitNums.Heart).ToList();
        possibleCombs.AddRange(AddFollowForSuit(targetTypes, heartList, true));
        var spadeList = myCardsInHand.Where(card => card.SuitNum == SuitNums.Spade).ToList();
        possibleCombs.AddRange(AddFollowForSuit(targetTypes, spadeList, true));
        
        // 再检查一遍，有备无患
        if (possibleCombs.Count == 0) {
            return false;
        }
        
        return true;
    }
    
    public void SortInHand(List<CardData> cardsInHand) {
        // 1. Point 大的在前；2. Suit 小的在前；3. 大小王在最左面
        // 直接比较 PointNum * 100 - SuitNum 即可
        cardsInHand.Sort((x, y) => 
            (100 * GetPointWeight(y.PointNum) - (int)y.SuitNum) -
            (100 * GetPointWeight(x.PointNum) - (int)x.SuitNum));
    }

    public void SortToLead(List<CardData> combToLead, SCG.Dictionary<string, int> type = null) {
        var cardsStr = RuleTool.CardList2RuleStr(combToLead);
        var cardsMap = Str2CardMap(cardsStr);
        // cardsMap 的数量作为放在前面的依据（大者在前），如果数量一致，再比较点数和花色
        combToLead.Sort((x, y) =>
            cardsMap[CardTool.GetPointName(x.PointNum)] == cardsMap[CardTool.GetPointName(y.PointNum)]
                ? (100 * GetPointWeight(y.PointNum) - (int)y.SuitNum) - (100 * GetPointWeight(x.PointNum) - (int)x.SuitNum)
                : cardsMap[CardTool.GetPointName(y.PointNum)] - cardsMap[CardTool.GetPointName(x.PointNum)]);
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
    /// <param name="isFlush"></param>
    /// <returns></returns>
    public CombData IntList2CombData(List<int> cards, bool isFlush = false) {
        var strList = Int2StrList(cards); // 使用自己的函数！！！！！
        // 借用 isCombValid 的代码（不能直接用，因为有 toLeadCards)
        
        // 这里的排序针对点数，最终封装的还是原参数 tryLeadCards
        var sortedCards = SortCards(strList);
        var cardsStr = RuleTool.RuleList2RuleStr(sortedCards);
        // 合法的情况：将 Godot Array 转成 C# Dictionary 格式
        if (!isFlush) {
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
        }
        else {
            if (FlushData.TryGetValue(cardsStr, out var value)) {
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