using Godot;
using System;
using System.Collections.Generic;
using DouCardPuzzoom.scripts.classes;
using DouCardPuzzoom.scripts.managers;
using DouCardPuzzoom.scripts.utils;

public partial class LeadPlace : Node2D {
    public Label Label;

    public override void _Ready() {
        base._Ready();
        Label = GetNode<Label>("Label");
    }

    public void Clear() {
        Label.Hide();
        foreach (var node in GetChildren()) {
            if (node.Owner == null) {
                node.QueueFree();
            }
        }
    }

    // 【补充】不需要用一个函数处理两种情况
    // public void ShowLeadCards(List<CardData> cards) {
    //     Clear();
    //     if (cards.Count == 0) {
    //         Label.Show();
    //     }
    //     else {
    //         UpdateCards(cards);
    //     }
    // }

    public void Pass() {
        Label.Show();
    }
    
    /// <summary>
    /// 生成卡牌的方法，后续根据不同位置需要重写
    /// </summary>
    /// <param name="cards">卡牌数据 List（非图像）</param>
    public virtual void UpdateCards(List<CardData> cards) {
        Clear(); // ShowLeadCards 函数遗留
        var s = cards.Count;
        for(int i = 0; i < s; i++) {
            var suitName = SuitPointTool.GetSuitName(cards[i].SuitNum);
            var pointName = SuitPointTool.GetPointName(cards[i].PointNum);
            var card = new Sprite2D();
            card.Texture = GD.Load<Texture2D>($"res://assets/pokers/{suitName}-{pointName}.png");
            card.Scale = Vector2.One * 0.5f * ConstManager.LeadCardScale;
            AddChild(card);

            card.Position = new Vector2(
                (i - (s - 1) / 2f) * ConstManager.CardSideDistance * ConstManager.LeadCardScale, 0);
        }
    }
    
}
