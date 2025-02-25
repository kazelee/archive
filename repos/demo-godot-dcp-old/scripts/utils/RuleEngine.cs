using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using DouCardPuzzoom.scripts.classes;
using Godot;
using Godot.Collections;

using Array = Godot.Collections.Array;
using SCG = System.Collections.Generic;

namespace DouCardPuzzoom.scripts.utils;

// 该部分为迁移代码，参考：https://github.com/onestraw/doudizhu
// 为了读取方便，内部大量使用 Godot 内置的数据结构 Array 动态数组
// 这部分代码极其混乱，有大量冗余判断，不易维护，如无问题就不要修改了

public static class RuleEngine {
    public static Dictionary Data;
    public static Dictionary TypeCards;
    
    /// <summary>
    /// 【注意】只能在 Godot 编辑器中调用，不能在 Rider 中调试<br/>
    /// 因为使用到 Godot 的 GlobalizePath Dictionary 和 Json
    /// </summary>
    public static void Start() {
        var dataStr = File.ReadAllText(ProjectSettings.GlobalizePath("res://docs/rule/data.json"));
        Data = Json.ParseString(dataStr).AsGodotDictionary();
        
        var typeCardsStr = File.ReadAllText(ProjectSettings.GlobalizePath("res://docs/rule/type_cards.json"));
        TypeCards = Json.ParseString(typeCardsStr).AsGodotDictionary();

        // [Test]
        // foreach (var kv in Str2CardMap("6-6-6-3-3")) {
        //     GD.Print($"(\"{kv.Key}\", {kv.Value})");
        // }
        // GD.Print(CheckCardType(Str2Cards("6-6-6-3-3")));
        // GD.Print(ListGreaterCards("6-6-6-3-3", "CJ-A-A-A-K-Q-J-10-10-10-10-9-7-7-5-5"));
        // foreach (var s in ListGreaterCardsArray("6-6-6-3-3", "CJ-A-A-A-K-Q-J-10-10-10-10-9-7-7-5-5")) {
        //     GD.Print($"\"{s}\"");
        // }
    }
    
    /// [(heart, A), (spade, J)] -> "A-J"，有损不可逆 map
    public static string Cards2Str(List<CardData> cards) {
        List<string> cardPoints = new();
        foreach (var card in cards) {
            cardPoints.Add(SuitPointTool.GetPointName(card.PointNum));
        }

        return string.Join("-", cardPoints);
    }

    /// [(heart, A), (spade, J)] -> ["A", "J"]，有损不可逆 map
    public static List<string> Cards2Points(List<CardData> cards) {
        // Linq 表达式，与上面函数的操作一致
        return cards.Select(card => SuitPointTool.GetPointName(card.PointNum)).ToList();
    }
    
    /// ["A", "J"] -> "A-J"
    public static string Cards2Str(List<string> cards) {
        return string.Join("-", cards);
    }

    /// "A-J" -> ["A", "J"]
    public static List<string> Str2Cards(string str) {
        return new List<string>(str.Split("-"));
    }
    
    /// "A-J" -> {"A": 1, "J": 1}
    public static SCG.Dictionary<string, int> Str2CardMap(string str) {
        var cards = Str2Cards(str);
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

    /// ["A", "J"] -> ["J", "A"]
    public static List<string> SortCards(List<string> cards) {
        var newCards = cards;
        // (x, y): x < y
        newCards.Sort((x, y) => SuitPointTool.GetPointNum(x).CompareTo(SuitPointTool.GetPointNum(y)));
        return newCards;
    }
    
    /// e.g. ["A", "A"] -> [["pair", 11], ...]
    public static Array CheckCardType(List<string> cards) {
        var sortedCards = SortCards(cards);
        var typeStr = Cards2Str(sortedCards);
        // 代替 if contains 判断
        Data.TryGetValue(typeStr, out var value);
        return (Array)value;
    }
    
    /// return typeX > typeY (if == return false)
    public static bool TypeGreater(Array typeX, Array typeY) {
        if ((string)typeX[0] == (string)typeY[0]) {
            return (int)typeX[1] > (int)typeY[1];
        }
        // X != Y, X rocket => X > Y
        if ((string)typeX[0] == "rocket" && (int)typeX[1] != -1) {
            return true;
        }
        // X != Y, Y rocket => Y > X
        if ((string)typeY[0] == "rocket" && (int)typeY[1] != -1) {
            return false;
        }
        // X != Y, X bomb => X > Y
        if ((string)typeX[0] == "bomb" && (int)typeX[1] != -1) {
            return true;
        }
        
        return false;
    }

    /// <summary>
    /// return cardsX > cardsY
    /// </summary>
    /// <param name="cardsX">["3", "3"]</param>
    /// <param name="cardsY">["A", "A"]</param>
    /// <returns>false</returns>
    public static bool CardsGreater(List<string> cardsX, List<string> cardsY) {
        var typeX = CheckCardType(cardsX);
        var typeY = CheckCardType(cardsY);
        // typeX invalid
        if (typeX.Count == 0 && typeY.Count != 0) {
            return false;
        }
        // typeY invalid
        if (typeX.Count != 0 && typeY.Count == 0) {
            return true;
        }
        // all invalid
        if (typeX.Count == 0 && typeY.Count == 0) {
            return false;
        }
        // compare, at least one greater
        foreach (var tx in typeX) {
            foreach (var ty in typeY) {
                if (TypeGreater((Array)tx, (Array)ty)) {
                    return true;
                }
            }
        }

        return false;
    }

    /// mine: {"A": 3, "10": 2}, others: {"K": 3, "5": 1} => return true;
    public static bool CardsContain(SCG.Dictionary<string, int> candidateCardmap, SCG.Dictionary<string, int> cardmap) {
        foreach (var k in cardmap.Keys) {
            // 从自己的牌数字典中获取这个点数，如果没有就过
            if (!candidateCardmap.TryGetValue(k, out var value)) {
                return false;
            }
            // 如果有这个点数，但数量不够，过
            if (value < cardmap[k]) {
                return false;
            }
        }

        return true;
    }

    /// "K-K-K-10", "A-A-A-A-5-5" => {"trio_solo": ["A-A-A-5"], "bomb": ["A-A-A-A"]}
    public static Dictionary ListGreaterCards(string cardsTarget, string cardsCandidate) {
        var targetType = CheckCardType(Str2Cards(cardsTarget));
        if (targetType.Count == 0) {
            GD.PrintErr("Target is null!");
        }
        // 对 targetType 去重，保留同 type 中 weight 最大的
        var tempDict = new Dictionary();
        foreach (var tw in targetType) {
            var typeAndWeight = (Array)tw;
            var cardType = (string)typeAndWeight[0];
            var weight = (int)typeAndWeight[1];
            if (!tempDict.ContainsKey(cardType) || weight > (int)tempDict[cardType]) {
                tempDict[cardType] = weight;
            }
        }
        
        foreach (var key in tempDict.Keys) {
            targetType.Add(new Array {key, tempDict[key]});
        }

        // 如果目标牌型为 rocket，则一定打不过，直接返回空
        if ((string)((Array)targetType[0])[0] == "rocket") {
            return new Dictionary();
        }

        // 按牌型大小依次判断是否可用bomb, rocket
        // 这里置 -1 的原因是方便比较，如果有炸弹，直接比较 weight 判断可跟牌
        // 所以实际上，上面的 CardsGreater 无需再判断 -1，不过结果是一样的
        if ((string)((Array)targetType[0])[0] != "rocket") {
            if ((string)((Array)targetType[0])[0] != "bomb") {
                targetType.Add(new Array {"bomb", -1});
            }
            targetType.Add(new Array {"rocket", -1});
        }
        else if ((string)((Array)targetType[0])[0] != "bomb") {
            targetType.Add(new Array {"bomb", -1});
        }

        var candidateCardmap = Str2CardMap(cardsCandidate);
        var cardsGt = new Dictionary();

        foreach (var ctw in targetType) {
            var cardTypeAndWeight = (Array)ctw;
            var cardType = (string)cardTypeAndWeight[0];
            var weight = (int)cardTypeAndWeight[1];
            var weightGt = new List<string>();
            foreach (var w in ((Dictionary)TypeCards[cardType]).Keys) {
                var wStr = (string)w;
                if (wStr.ToInt() > weight) {
                    weightGt.Add(wStr);
                }

                if (!cardsGt.ContainsKey(cardType)) {
                    cardsGt[cardType] = new Array();
                }
            }
            weightGt.Sort((x, y) => x.ToInt().CompareTo(y.ToInt()));

            foreach (var wStr in weightGt) {
                foreach (var wc in (Array)((Dictionary)TypeCards[cardType])[wStr]) {
                    var wCards = (string)wc;
                    var wCardmap = Str2CardMap(wCards);
                    if (CardsContain(candidateCardmap, wCardmap)
                        && !((Array)cardsGt[cardType]).Contains(wCards)) {
                        ((Array)cardsGt[cardType]).Add(wCards);
                    }
                } 
            }

            if (((Array)cardsGt[cardType]).Count == 0) {
                cardsGt.Remove(cardType);
            }
        }

        return cardsGt;
    }

    /// "K-K-K-10", "A-A-A-A-5-5" => ["A-A-A-5", "A-A-A-A"]
    public static List<string> ListGreaterCardsArray(string cardsTarget, string cardsCandidate) {
        // "K-K-K-10", "A-A-A-A-5-5" => {"trio_solo": ["A-A-A-5"], "bomb": ["A-A-A-A"]}
        var cardsGt = ListGreaterCards(cardsTarget, cardsCandidate);

        var result = new List<string>();
        // {"trio_solo": ["A-A-A-5"], "bomb": ["A-A-A-A"]} => ["A-A-A-5", "A-A-A-A"]
        foreach (var kw in cardsGt) {
            foreach (var s in (Array)kw.Value) {
                var str = (string)s;
                result.Add(str);
            }
        }

        return result;
    }
}