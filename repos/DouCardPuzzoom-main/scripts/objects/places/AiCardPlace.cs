using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DouCardPuzzoom.scripts;
using DouCardPuzzoom.scripts.classes;
using DouCardPuzzoom.scripts.enums;
using DouCardPuzzoom.scripts.manager;
using DouCardPuzzoom.scripts.tools;

public partial class AiCardPlace : Node2D {
    public PlacePositions Place;
    public List<CardData> CardsInHand = new();

    public override void _Ready() {
        base._Ready();
    }

    public async void InitCards(List<CardData> cardDatas, bool isBlind = false) {
        foreach (var node in GetChildren()) {
            if (node.Owner == null) {
                node.QueueFree();
            }
        }

        if (cardDatas.Count == 0) {
            return;
        }

        // 两排卡牌的上下距离差取15f（为了看清花色）
        // y: -(i/10) => if y <= 10 then 0 else -1        
        var upDownDis = 15f;
        var s = cardDatas.Count;
        for (int i = 0; i < cardDatas.Count; i++) {
            var suitName = CardTool.GetSuitName(cardDatas[i].SuitNum);
            var pointName = CardTool.GetPointName(cardDatas[i].PointNum);
            var card = new Sprite2D();
            if (isBlind) {
                card.Texture = GD.Load<Texture2D>("res://assets/pokers/back-orange.png");
            }
            else {
                card.Texture = GD.Load<Texture2D>($"res://assets/pokers/{suitName}-{pointName}.png");    
            }
            card.Scale = Vector2.One * ConstManager.AiCardScale;
            AddChild(card);
            
            card.Scale = new Vector2(0.6f, 0.6f);

            var newPosition = new Vector2();
            // 卡牌相对 place 的位置
            switch (Place) {
                case PlacePositions.Up:
                    card.Position = new Vector2(0, 97); // 70 + 27
                    newPosition = new Vector2(
                        (i - (s - 1) / 2f) * ConstManager.CardSideDistance * ConstManager.AiCardScale, 0);
                    break;
                case PlacePositions.Left:
                    card.Position = new Vector2(142, 97); // 70 + 27
                    newPosition = new Vector2(
                        (i % 10) * ConstManager.CardSideDistance * ConstManager.AiCardScale,
                        (i / 10) * upDownDis);
                    break;
                case PlacePositions.Right:
                    card.Position = new Vector2(-142, 97); // 70 + 27
                    if (s > 10) {
                        // 第二牌靠右侧
                        if (i < 10) {
                            newPosition = new Vector2(
                                (i - 10 + 1) * ConstManager.CardSideDistance * ConstManager.AiCardScale, 0);
                        }
                        else {
                            newPosition = new Vector2(
                                (i % 10 - s + 10 + 1) * ConstManager.CardSideDistance * ConstManager.AiCardScale, upDownDis);
                        }
                    }
                    else {
                        newPosition = new Vector2(
                            (i - s + 1) * ConstManager.CardSideDistance * ConstManager.AiCardScale, 0);
                    }
                    break;
            }
            
            var tween = GetTree().CreateTween(); // 的确可以去除警告，但过快的切换还是会导致错位，算了
            tween.SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Quart);
            tween.TweenProperty(card, "position", newPosition, 0.3);
            tween.TweenProperty(card, "scale", Vector2.One * 0.8f, 0.1);
            // tween.TweenCallback()

            await DelayFunc(10); // 50 -> 10；调快一点，曲线救国了
        }
    }
    
    // 在 MyCardPlace 上更改，修改 CardVisual 为 Sprite2D
    public void UpdateCards(List<CardData> cardDatas, bool isBlind = false) {
        foreach (var node in GetChildren()) {
            if (node.Owner == null) {
                node.QueueFree();
            }
        }

        if (cardDatas.Count == 0) {
            return;
        }

        // 两排卡牌的上下距离差取15f（为了看清花色）
        // y: -(i/10) => if y <= 10 then 0 else -1        
        var upDownDis = 15f;
        var s = cardDatas.Count;
        for (int i = 0; i < cardDatas.Count; i++) {
            var suitName = CardTool.GetSuitName(cardDatas[i].SuitNum);
            var pointName = CardTool.GetPointName(cardDatas[i].PointNum);
            var card = new Sprite2D();
            if (isBlind) {
                card.Texture = GD.Load<Texture2D>("res://assets/pokers/back-orange.png");
            }
            else {
                card.Texture = GD.Load<Texture2D>($"res://assets/pokers/{suitName}-{pointName}.png");    
            }
            card.Scale = Vector2.One * ConstManager.AiCardScale;
            AddChild(card);
            
            var newPosition = new Vector2();
            // 卡牌相对 place 的位置
            switch (Place) {
                case PlacePositions.Up:
                    newPosition = new Vector2(
                        (i - (s - 1) / 2f) * ConstManager.CardSideDistance * ConstManager.AiCardScale, 0);
                    break;
                case PlacePositions.Left:
                    newPosition = new Vector2(
                        (i % 10) * ConstManager.CardSideDistance * ConstManager.AiCardScale,
                        (i / 10) * upDownDis);
                    break;
                case PlacePositions.Right:
                    if (s > 10) {
                        // 第二牌靠右侧
                        if (i < 10) {
                            newPosition = new Vector2(
                                (i - 10 + 1) * ConstManager.CardSideDistance * ConstManager.AiCardScale, 0);
                        }
                        else {
                            newPosition = new Vector2(
                                (i % 10 - s + 10 + 1) * ConstManager.CardSideDistance * ConstManager.AiCardScale, upDownDis);
                        }
                    }
                    else {
                        newPosition = new Vector2(
                            (i - s + 1) * ConstManager.CardSideDistance * ConstManager.AiCardScale, 0);
                    }
                    break;
            }

            card.Position = newPosition;
        }
    }
    
    public async Task DelayFunc(int delayMs) {
        await Task.Delay(delayMs);
    }
}
