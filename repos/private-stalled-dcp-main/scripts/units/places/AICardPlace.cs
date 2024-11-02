using Godot;
using System;
using System.Collections.Generic;
using DouCardPuzzoom.scripts.classes;
using DouCardPuzzoom.scripts.enums;
using DouCardPuzzoom.scripts.managers;
using DouCardPuzzoom.scripts.utils;

public partial class AICardPlace : Node2D
{
    public List<CardData> CardsInHand = new();

    public override void _Ready() {
        base._Ready();
        var test = new List<CardData> {
            new(SuitNums.Heart, PointNums.N5),
            new(SuitNums.Diamond, PointNums.K),
            new(SuitNums.Joker, PointNums.CJ),
            new(SuitNums.Club, PointNums.N10),
            new(SuitNums.Spade, PointNums.N6),
            new(SuitNums.Club, PointNums.N7),
            new(SuitNums.Heart, PointNums.A),
            new(SuitNums.Heart, PointNums.Q),
            new(SuitNums.Diamond, PointNums.N10),
        };
        test = CardTool.Sorted(test);
        CardsInHand = test;
        UpdateCards(test);
    }
    
    // 在 MyCardPlace 上更改，修改 CardVisual 为 Sprite2D
    public void UpdateCards(List<CardData> cardDatas) {
        foreach (var node in GetChildren()) {
            if (node.Owner == null) {
                node.QueueFree();
            }
        }

        if (cardDatas.Count == 0) {
            return;
        }

        for (int i = 0; i < cardDatas.Count; i++) {
            var suitName = SuitPointTool.GetSuitName(cardDatas[i].SuitNum);
            var pointName = SuitPointTool.GetPointName(cardDatas[i].PointNum);
            var card = new Sprite2D();
            card.Texture = GD.Load<Texture2D>($"res://assets/pokers/{suitName}-{pointName}.png");
            card.Scale = Vector2.One * 0.5f * ConstManager.AICardScale;
            AddChild(card);
            // 卡牌相对 place 的位置
            var newPosition = Vector2.Right *
                              (i - (cardDatas.Count - 1) / 2f) * ConstManager.CardSideDistance * ConstManager.AICardScale;
            card.Position = newPosition;
        }
    }
}
