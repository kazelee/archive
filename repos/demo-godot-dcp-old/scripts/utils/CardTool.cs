using System.Collections.Generic;
using DouCardPuzzoom.scripts.classes;

namespace DouCardPuzzoom.scripts.utils; 

public static class CardTool {
    /// <summary>
    /// 将 花色-点数 字符串转换成 CardData List
    /// </summary>
    /// <param name="cardStr">形如：sA, h5, cK, d10, jC</param>
    /// <returns>CardData List</returns>
    public static List<CardData> GetCards(string cardStr) {
        foreach (var ch in cardStr) {
            // pass
        }

        return new();
    }
    
    public static List<CardData> Sorted(List<CardData> cardDatas) {
        // SuitNum: 0 - 4, PointNum: 0 - 12, 13, 14
        // 先比较 PointNum，再比较 SuitNum；Point 大的在前，Suit 小的在前
        // 大小王除外，不过 Point 足够大，无需额外考虑
        // 所以直接比较 PointNum * 100 - SuitNum 即可
        cardDatas.Sort((x, y) =>
            (100 * (int)y.PointNum - (int)y.SuitNum) - 
            (100 * (int)x.PointNum - (int)x.SuitNum));
        return cardDatas;
    }

    public static List<CardData> SortedForLead(List<CardData> cardDatas) {
        var cardsStr = RuleEngine.Cards2Str(cardDatas);
        var cardsMap = RuleEngine.Str2CardMap(cardsStr);
        // cardsMap 的数量作为放在前面的依据（大者在前），如果数量一致，再比较花色
        cardDatas.Sort((x, y) =>
            cardsMap[SuitPointTool.GetPointName(x.PointNum)] == cardsMap[SuitPointTool.GetPointName(y.PointNum)]
                ? (100 * (int)y.PointNum - (int)y.SuitNum) - (100 * (int)x.PointNum - (int)x.SuitNum)
                : cardsMap[SuitPointTool.GetPointName(y.PointNum)] - cardsMap[SuitPointTool.GetPointName(x.PointNum)]);
        return cardDatas;
    }
}