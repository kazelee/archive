using System.Collections.Generic;
using DouCardPuzzoom.scripts.enums;

namespace DouCardPuzzoom.scripts.classes;

/// <summary>
/// 一个回合的记录：从第一个出牌，到最后连续两人PASS
/// </summary>
public class OneTurnInfo {
    // 每Turn最后的状态记录（撤回时看列表的最后一项）
    public TurnStates CurrentState;
    public CombData CurrentBiggest;
    public string NextTurnFirst; // 除了第一个TurnInfo外，没啥用
    public int PassTimes;
    public int PassLimitNum;
    public int PlayLimitNum;
    public int PassRequestNum;
    public int PlayRequestNum;
    
    /// <summary>
    /// 出牌人名称列表（没有YOU就不能撤回，只需要判断第一个）
    /// </summary>
    public List<string> Turns; // 除了第一个TurnInfo外，没啥用
    
    /// <summary>
    /// 每个回合的出牌列表，null表示PASS
    /// </summary>
    public List<CombData> Combs;

    public override string ToString() {
        var res = $"[OneTurn]\nState: {CurrentState}\tNext: {NextTurnFirst}\tPass: {PassTimes}\n" +
                  $"PassLimit: {PassLimitNum}\tPlayLimit: {PlayLimitNum}\n" +
                  $"PassRequest: {PassRequestNum}\tPlayRequest: {PlayRequestNum}\nBigComb: [";
        if (CurrentBiggest == null) {
            res += "null";
        }
        else {
            foreach (var cd in CurrentBiggest.Cards) {
                res += $"{cd}, ";
            }
        }

        res += "]\nTurns: ";
        // 放弃Types的记录……
        // foreach (var kv in CurrentBiggest.Types) {
        //     res += $"{kv.Key}: {kv.Value}; ";
        // }

        foreach (var t in Turns) {
            res += $"{t}, ";
        }

        res += "\nCombs: ";
        foreach (var cb in Combs) {
            res += "[";
            if (cb == null) {
                res += "null";
            }
            else {
                foreach (var cd in cb.Cards) {
                    res += $"{cd}, ";
                }
            }
            
            res += "], ";
        }

        return res;
    }
}