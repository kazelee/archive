using System.Collections.Generic;
using DouCardPuzzoom.scripts.classes;

namespace DouCardPuzzoom.scripts.managers; 

public static class GameManager {
    /// <summary>
    /// 临时记录：当前牌局 轮次 的最大牌
    /// </summary>
    public static List<CardData> currentGreaterCards = new();


    /// <summary>
    /// 场景记录：当前模式是否是随机发牌的
    /// </summary>
    public static bool IsRandomDeal = true;
}