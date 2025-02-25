using System.Collections.Generic;
using System.Linq;
using DouCardPuzzoom.scripts.ai;
using DouCardPuzzoom.scripts.classes;
using DouCardPuzzoom.scripts.manager;
using DouCardPuzzoom.scripts.tools;
using Godot;
using Godot.Collections;

using SCG = System.Collections.Generic;

namespace DouCardPuzzoom.scripts.rule; 

public class RuleTwoDeck : IRule{
    public readonly Dictionary Data = DataLoader.RuleTwoDeckData;
    public readonly Dictionary TypeCards = DataLoader.RuleTwoDeckTypeCards;

    public readonly string[] BombTypes = new[] { "bomb", "rocket", "bomb_5", "bomb_6", "rocket_3", "bomb_7", "bomb_8", "rocket_4" };

    public readonly SCG.Dictionary<string, int> BombWeights = new SCG.Dictionary<string, int>() {
        {"bomb", 0}, {"rocket", 1}, {"bomb_5", 2}, {"bomb_6", 3}, {"rocket_3", 4}, {"bomb_7", 5}, {"bomb_8", 6}, {"rocket_4", 7}
    };
    
    // GetAllComb 和 CanFollow 返回的列表必须 不包含 需要二次筛选的项目！
    // 3 - CJ: 0 - 14 (0 - 12, 13, 14)
    
    public List<CombData> GetAllComb(List<CardData> myCardsInHand) {
        // 正常流程：逐一算牌
        var cardsRule = RuleTool.CardList2RuleList(myCardsInHand);
        return GetAllCombOri(cardsRule);
    }

    // 编写一个针对RuleList的版本
    public List<CombData> GetAllCombOri(List<string> cardsRule) {
        var soloList = new List<int>();
        var pairList = new List<int>();
        var trioList = new List<int>();
        var bombList = new List<int>();
        var bomb5List = new List<int>();
        var bomb6List = new List<int>();
        var bomb7List = new List<int>();
        var bomb8List = new List<int>();

        var cardsInt = RuleTool.RuleList2Int(cardsRule);
        var cardsMap = Int2CardMap(cardsInt);

        var combList = new List<CombData>(); // { new CombData(new List<string>(), new SCG.Dictionary<string, int>()) };

        // 最终决定保留这部分逻辑，不再更改，选牌大小按照TypeCards的顺序！！！
        // 即：单张、单顺、对子、连对、三张、三一顺、三二顺、四带一、四带二、炸弹、王炸
        // 这样地主的逻辑也直接排序了，不必再写逻辑；后面如果有非单张最小的规则也好复用
        // 【补充】三带、四带的逻辑都是单双合并的，没有确保先单后双！

        // 1.1 单张
        foreach (var kv in cardsMap) {
            if (kv.Value >= 1) {
                soloList.Add(kv.Key);
                var intList = new List<int> { kv.Key };
                combList.Add(IntList2CombData(intList));
            }

            if (kv.Value >= 2) { // 枚举时不知道大小王可以有多张
                pairList.Add(kv.Key);
                // var intList = new List<int>() { kv.Key, kv.Key };
                // combList.Add(IntList2CombData(intList));
            }

            if (kv.Value >= 3 && kv.Key != 13 && kv.Key != 14) {
                trioList.Add(kv.Key);
                // var intList = new List<int>() { kv.Key, kv.Key, kv.Key };
                // combList.Add(IntList2CombData(intList));
            }

            if (kv.Value >= 4 && kv.Key != 13 && kv.Key != 14) { // 你说得对，但是我可以整很多牌
                bombList.Add(kv.Key);
            }

            if (kv.Value >= 5 && kv.Key != 13 && kv.Key != 14) { // 你说得对，但是我可以整很多牌
                bomb5List.Add(kv.Key);
            }

            if (kv.Value >= 6 && kv.Key != 13 && kv.Key != 14) { // 你说得对，但是我可以整很多牌
                bomb6List.Add(kv.Key);
            }

            if (kv.Value >= 7 && kv.Key != 13 && kv.Key != 14) { // 你说得对，但是我可以整很多牌
                bomb7List.Add(kv.Key);
            }

            if (kv.Value >= 8 && kv.Key != 13 && kv.Key != 14) { // 你说得对，但是我可以整很多牌
                bomb8List.Add(kv.Key);
            }

        }

        // 1.2 单顺

        // 2.1 对子
        foreach (var pair in pairList) {
            combList.Add(IntList2CombData(new List<int> { pair, pair }));
        }

        // 2.2 连对
        CreateStraight(pairList, 3, combList, 2);

        // 3.1 三张（合并在下）
        // 3.2 三带
        foreach (var trio in trioList) {
            combList.Add(IntList2CombData(new List<int> { trio, trio, trio }));
            // 三带二
            foreach (var card in pairList) {
                if (card == trio) continue;
                combList.Add(IntList2CombData(new List<int>() { trio, trio, trio, card, card }));
            }
        }

        // 3.3 飞机
        var trioChainList = CreateStraight(trioList, 2, combList, 3);
        // 三连顺 带牌逻辑：只要带牌不是三连顺中的牌，什么牌都可以
        // 注意：trioChainList是只记录几连，无重复
        // GD.Print(trioChainList == null);
        if (DebugManager.AllowPlane) {
            if (trioChainList != null) {
                // GD.Print($"trio_count: {trioChainList.Count}");

                foreach (var intList in trioChainList) {
                    // var intListStr = intList.Aggregate("intList: ", (current, solo) => current + $"{solo}, ");
                    // GD.Print(intListStr);

                    // 实际上重复后的chain
                    var chain = Repeat(intList, 3);

                    // 对子：和单张是一个逻辑，不另外设置函数了
                    var tmpPair = pairList.Where(card => !intList.Contains(card)).ToList();
                    // 单张不够用了，直接退出，对子肯定更没戏
                    // if (tmpSolo.Count < intList.Count) break;

                    var allPair = new List<int>();
                    foreach (var pair in tmpPair) {
                        if (pair == 13 || pair == 14) continue; // 去掉带牌里面可能有的大小王
                        // 所有除了三连以外的单牌
                        allPair = new List<int>();
                        for (int i = 0; i < cardsMap[pair]; i++) {
                            allPair.Add(pair);
                        }
                    }

                    // 所有可能的带牌逻辑
                    var possWithsTwo = CombineTool.GetCombinations(allPair, intList.Count);
                    foreach (var possWith in possWithsTwo) {
                        var tmpList = new List<int>(chain);
                        tmpList.AddRange(Repeat(possWith, 2));
                        combList.Add(IntList2CombData(tmpList));
                    }
                }
            }
        }

        // 炸弹
        foreach (var bomb in bombList) {
            var intList = new List<int> { bomb, bomb, bomb, bomb };
            combList.Add(IntList2CombData(intList));
        }

        // 王炸的情况
        if (cardsInt.Contains(13) && cardsInt.Contains(14)) {
            combList.Add(IntList2CombData(new List<int> { 13, 14 }));
        }

        foreach (var bomb in bomb5List) {
            var intList = new List<int> { bomb, bomb, bomb, bomb, bomb };
            combList.Add(IntList2CombData(intList));
        }

        foreach (var bomb in bomb6List) {
            var intList = new List<int> { bomb, bomb, bomb, bomb, bomb, bomb };
            combList.Add(IntList2CombData(intList));
        }

        if (pairList.Contains(13) && cardsInt.Contains(14)) {
            combList.Add(IntList2CombData(new List<int> { 13, 13, 14 }));
        }
        if (pairList.Contains(14) && cardsInt.Contains(13)) {
            combList.Add(IntList2CombData(new List<int> { 13, 14, 14 }));
        }
        
        foreach (var bomb in bomb7List) {
            var intList = new List<int> { bomb, bomb, bomb, bomb, bomb, bomb, bomb };
            combList.Add(IntList2CombData(intList));
        }
        foreach (var bomb in bomb8List) {
            var intList = new List<int> { bomb, bomb, bomb, bomb, bomb, bomb, bomb, bomb };
            combList.Add(IntList2CombData(intList));
        }

        if (pairList.Contains(13) && pairList.Contains(14)) {
            combList.Add(IntList2CombData(new List<int> { 13, 13, 14, 14 }));
        }
        
        return CombTool.CombDistinct(combList); // 索性现在的AI出牌不会优先出对子，逃过一劫？
        // 哦，之前列举的时候把大小王从非solo list中剔除了……原来如此……
    }
    
    private List<int> Repeat(List<int> list, int times) {
        var newList = new List<int>();
        foreach (var card in list) {
            for (int i = 0; i < times; i++) {
                newList.Add(card);
            }
        }

        return newList;
    }
    
    // 内部使用函数（直接add，所以无所谓ref）
    private List<List<int>> CreateStraight(List<int> list, int min, List<CombData> combList, int num) {
        // 2，BJ，CJ不能作为连顺的组成部分
        var newList = list.Where(card => card < 12).ToList();
        newList.Sort();
        var len = newList.Count;
        if (len < min) return new List<List<int>>(); // null
        
        var possIntLists = new List<List<int>>(); 
        // 滑动窗口 [0, min - 1]
        for (int beg = 0; beg < len; beg++) {
            for (int end = beg + min - 1; end < len; end++) {
                if (newList[end] - newList[beg] != end - beg) break;
                // 肯定符合min要求，无需判断
                var tmpList = new List<int>(); // 只记录一个
                var intList = new List<int>();
                for (int i = beg; i <= end; i++) {
                    tmpList.Add(newList[i]); // NEW ADDED CODE: or tmpList always EMPTY!
                    for (int n = 0; n < num; n++) {
                        intList.Add(newList[i]); // 重复添加 num 次
                    }
                }
                combList.Add(IntList2CombData(intList));
                if (num == 3) possIntLists.Add(tmpList); // 只有3张的时候考虑带牌
            }
        }

        return num == 3 ? possIntLists : new List<List<int>>(); // null
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
            // 该部分借用canFollow的代码，出于保险起见也加判断条件（没用）
            if (localTypes.Contains(new KeyValuePair<string, int>("rocket", -1))) {
                localTypes.Remove("rocket");
            }
            if (localTypes.Contains(new KeyValuePair<string, int>("bomb", -1))) {
                localTypes.Remove("bomb");
            }
            if (localTypes.Contains(new KeyValuePair<string, int>("bomb_5", -1))) {
                localTypes.Remove("bomb_5");
            }
            if (localTypes.Contains(new KeyValuePair<string, int>("bomb_6", -1))) {
                localTypes.Remove("bomb_6");
            }
            if (localTypes.Contains(new KeyValuePair<string, int>("bomb_7", -1))) {
                localTypes.Remove("bomb_7");
            }
            if (localTypes.Contains(new KeyValuePair<string, int>("bomb_8", -1))) {
                localTypes.Remove("bomb_8");
            }
            if (localTypes.Contains(new KeyValuePair<string, int>("rocket_3", -1))) {
                localTypes.Remove("rocket_3");
            }
            if (localTypes.Contains(new KeyValuePair<string, int>("rocket_4", -1))) {
                localTypes.Remove("rocket_4");
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
        
        // 先判断炸弹和火箭（该逻辑下不能添加 "rocket": -1）
        // 最终尝试添加-1的判断，莫名其妙地成功了，原因不明！！！！！
        if (types2.ContainsKey("rocket_4") && types2["rocket_4"] != -1) {
            return false;
        }

        // TMD 今天（2024年3月21日）才发现原来自己的王炸是打不出来的……
        // 好在canbeat只有玩家用，只要玩家的牌没有王炸就不受影响……赶紧改过来……
        if (types1.ContainsKey("rocket_4")) {
            return true;
        }

        var isType1Bomb = types1.Keys.Any(key => BombTypes.Contains(key) && types1[key] != -1);
        var isType2Bomb = types2.Keys.Any(key => BombTypes.Contains(key) && types2[key] != -1);
        
        // 我的牌能算炸弹而对手牌不算，能压
        if (isType1Bomb && !isType2Bomb) { // [1, 0] 0 1, 1 1, 0 0
            return true;
        }
        // 反之，压不了
        if (!isType1Bomb && isType2Bomb) { // [0, 1] 1 1, 0 0
            return false;
        }

        if (isType1Bomb && isType2Bomb) { // 嗯？应该不会有其他牌型吧……
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
        }
        
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

    public void TryAddType(SCG.Dictionary<string, int> targetTypes) {
        // 如果大的牌型存在，就不应该允许添加小的牌型
        // 否则，小炸弹就能打大炸弹了……
        if (!targetTypes.TryAdd("bomb_8", -1)) return;
        if (!targetTypes.TryAdd("bomb_7", -1)) return;
        if (!targetTypes.TryAdd("rocket_3", -1)) return;
        
        if (!targetTypes.TryAdd("bomb_6", -1)) return;
        if (!targetTypes.TryAdd("bomb_5", -1)) return;
        
        if (!targetTypes.TryAdd("rocket", -1)) return;
        if (!targetTypes.TryAdd("bomb", -1)) return;
    }
    
    // 保留原版的的函数
    public bool CanFollowOri(CombData target, List<string> myCardList, out List<CombData> possibleCombs) {
        // var targetTypes = target.Types;

        var myCardStr = RuleTool.RuleList2RuleStr(myCardList);
        // GD.PrintErr(target == null);
        // 必须使用深度拷贝！！！
        var targetTypes = target.Types.ToDictionary(kv => kv.Key, kv => kv.Value);
        
        // 如果目标牌型为 rocket 4，则一定打不过，直接返回空
        if (targetTypes.ContainsKey("rocket_4") && targetTypes["rocket_4"] != -1) {
            possibleCombs = new List<CombData>();
            return false;
        }
        // 目标牌型不是 rocket，可能是 bomb，出于方便可以临时添加 -1 便于比较
        targetTypes.Add("rocket_4", -1);
        // 如果不是 bomb，加入 -1 便于比较
        TryAddType(targetTypes);
        
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
                    if (localTypes.Contains(new KeyValuePair<string, int>("rocket", -1))) {
                        localTypes.Remove("rocket");
                    }
                    if (localTypes.Contains(new KeyValuePair<string, int>("bomb", -1))) {
                        localTypes.Remove("bomb");
                    }
                    if (localTypes.Contains(new KeyValuePair<string, int>("bomb_5", -1))) {
                        localTypes.Remove("bomb_5");
                    }
                    if (localTypes.Contains(new KeyValuePair<string, int>("bomb_6", -1))) {
                        localTypes.Remove("bomb_6");
                    }
                    if (localTypes.Contains(new KeyValuePair<string, int>("bomb_7", -1))) {
                        localTypes.Remove("bomb_7");
                    }
                    if (localTypes.Contains(new KeyValuePair<string, int>("bomb_8", -1))) {
                        localTypes.Remove("bomb_8");
                    }
                    if (localTypes.Contains(new KeyValuePair<string, int>("rocket_3", -1))) {
                        localTypes.Remove("rocket_3");
                    }
                    if (localTypes.Contains(new KeyValuePair<string, int>("rocket_4", -1))) {
                        localTypes.Remove("rocket_4");
                    }
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
        // 1. Point 大的在前；2. Suit 小的在前；3. 大小王在最前面
        // 直接比较 PointNum * 100 - SuitNum 即可
        cardsInHand.Sort((x, y) =>
            (100 * (int)y.PointNum - (int)y.SuitNum) - 
            (100 * (int)x.PointNum - (int)x.SuitNum));
    }

    public void SortToLead(List<CardData> combToLead, SCG.Dictionary<string, int> type = null) {
        var cardsStr = RuleTool.CardList2RuleStr(combToLead);
        var cardsMap = Str2CardMap(cardsStr);
        // cardsMap 的数量作为放在前面的依据（大者在前），如果数量一致，再比较点数和花色
        combToLead.Sort((x, y) =>
            cardsMap[CardTool.GetPointName(x.PointNum)] == cardsMap[CardTool.GetPointName(y.PointNum)]
                ? (100 * (int)y.PointNum - (int)y.SuitNum) - (100 * (int)x.PointNum - (int)x.SuitNum)
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

    /// <summary>
    /// 封装intList得到Combdata，无花色的strList！理论上不会为空<br/>
    /// 不必轻易调用，理论上只有极其复杂的三连顺才有多types的可能……
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    public CombData IntList2CombData(List<int> cards) {
        var strList = RuleTool.Int2StrList(cards);
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

            // 该部分借用canFollow的代码，出于保险起见也加判断条件（没用）
            if (localTypes.Contains(new KeyValuePair<string, int>("rocket", -1))) {
                localTypes.Remove("rocket");
            }
            if (localTypes.Contains(new KeyValuePair<string, int>("bomb", -1))) {
                localTypes.Remove("bomb");
            }
            if (localTypes.Contains(new KeyValuePair<string, int>("bomb_5", -1))) {
                localTypes.Remove("bomb_5");
            }
            if (localTypes.Contains(new KeyValuePair<string, int>("bomb_6", -1))) {
                localTypes.Remove("bomb_6");
            }
            if (localTypes.Contains(new KeyValuePair<string, int>("bomb_7", -1))) {
                localTypes.Remove("bomb_7");
            }
            if (localTypes.Contains(new KeyValuePair<string, int>("bomb_8", -1))) {
                localTypes.Remove("bomb_8");
            }
            if (localTypes.Contains(new KeyValuePair<string, int>("rocket_3", -1))) {
                localTypes.Remove("rocket_3");
            }
            if (localTypes.Contains(new KeyValuePair<string, int>("rocket_4", -1))) {
                localTypes.Remove("rocket_4");
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
        cards.Sort((x, y) => CardTool.GetPointNumUnsafe(x).CompareTo(CardTool.GetPointNumUnsafe(y)));
        return cards;
    }
}