using System;
using DouCardPuzzoom.scripts.enums;
using DouCardPuzzoom.scripts.tools;

namespace DouCardPuzzoom.scripts.classes; 

public class CardData {
    public readonly SuitNums SuitNum;
    public readonly PointNums PointNum;
    
    // 确保构造必须有值，不可能取默认的空值
    public CardData(SuitNums suitNum, PointNums pointNum) {
        SuitNum = suitNum;
        PointNum = pointNum;
    }
    
    public override string ToString() {
        // 形如"spade-A"，"joker-CJ"
        // return CardTool.GetSuitName(SuitNum) + "-" + CardTool.GetPointName(PointNum);
        
        // 形如"sA"，"jCJ"
        return CardTool.GetSuitName(SuitNum)[0] + CardTool.GetPointName(PointNum);
    }
}