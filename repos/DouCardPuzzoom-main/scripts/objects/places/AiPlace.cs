using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DouCardPuzzoom.scripts;
using DouCardPuzzoom.scripts.classes;
using DouCardPuzzoom.scripts.enums;
using DouCardPuzzoom.scripts.manager;

public partial class AiPlace : Node2D
{
    public delegate void LeadCards(List<CardData> myCards, List<CardData> target);
    public AiCardPlace AiCardPlace;
    public LeadPlace AiLeadPlace;

    public PlacePositions Place;
    
    public override void _Ready() {
        AiCardPlace = GetNode<AiCardPlace>("AiCardPlace");
        AiLeadPlace = GetNode<LeadPlace>("AiLeadPlace");
    }
    
    public void InitPosition(PlacePositions place) {
        AiCardPlace.Place = place;
        AiLeadPlace.Place = place;
        // AiPlace 与 PlayerPlace 重叠，只改变 card lead 相对于 AiPlace 的相对位置
        // 纵向：出牌4，手牌±70（参考玩家）
        // 横向：出牌±70， 手牌±142（待定！！！）
        switch (place) {
            case PlacePositions.Left:
                AiLeadPlace.Position = new Vector2(-70, 4);
                AiCardPlace.Position = new Vector2(-142, -70);
                break;
            case PlacePositions.Right:
                AiLeadPlace.Position = new Vector2(70, 4);
                AiCardPlace.Position = new Vector2(142, -70);
                break;
            case PlacePositions.Up:
                AiLeadPlace.Position = new Vector2(0, 4);
                AiCardPlace.Position = new Vector2(0, -70);
                break;
            case PlacePositions.Down:
            default:
                GD.PrintErr("AiPlace初始化错误！");
                return;
        }
    }
    
    public void InitCards(List<CardData> cardDatas) {
        // foreach (var cd in cardDatas) {
        //     GD.Print(cd);
        // }
        AiCardPlace.CardsInHand = cardDatas;
        AiCardPlace.InitCards(cardDatas, GameLogic.GameMode == GameModes.Blind);
    }

    public void Pass() {
        // await DelayFunc();
        AiLeadPlace.Pass();
        // async Task 不能用于委托函数？必须是 void？
        // await ToSignal(GetTree().CreateTimer(ConstManager.WaitTime), SceneTreeTimer.SignalName.Timeout);

    }
    
    public void LeadCard(List<CardData> leadCards) {
        // await DelayFunc();
        foreach (var cd in leadCards) {
            AiCardPlace.CardsInHand.Remove(cd);
        }
        AiCardPlace.UpdateCards(AiCardPlace.CardsInHand, GameLogic.GameMode == GameModes.Blind);
        AiLeadPlace.UpdateCards(leadCards, GameLogic.GameMode == GameModes.Blind);
        
        // async Task 不能用于委托函数？必须是 void？
        // await ToSignal(GetTree().CreateTimer(ConstManager.WaitTime), SceneTreeTimer.SignalName.Timeout);
    }
    
    // public async Task DelayFunc() {
    //     await Task.Delay(ConstManager.DelayMs);
    // }
    
}
