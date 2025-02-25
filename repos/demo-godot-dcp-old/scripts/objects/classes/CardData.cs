using System;
using DouCardPuzzoom.scripts.enums;

namespace DouCardPuzzoom.scripts.classes; 

public class CardData {
    public SuitNums SuitNum;
    public PointNums PointNum;
    
    public CardData(SuitNums suitNum, PointNums pointNum) {
        this.SuitNum = suitNum;
        this.PointNum = pointNum;
    }
    
    public override string ToString() {
        return SuitNum + "-" + PointNum;
    }
}