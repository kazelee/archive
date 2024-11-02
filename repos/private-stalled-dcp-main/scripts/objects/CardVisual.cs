using Godot;
using System;
using System.Runtime.InteropServices;
using DouCardPuzzoom.scripts.classes;
using DouCardPuzzoom.scripts.managers;
using DouCardPuzzoom.scripts.utils;

public partial class CardVisual : Area2D {
    public event Action<CardVisual> CardSelectedEvent;
    
    public bool IsLastOne = false;
    public bool IsSelected = false; // 用于卡牌位置移动
    
    public CardData Data;
    private Texture2D _texture;

    public override void _Ready() {
        MouseEntered += OnMouseEntered;
    }
    
    public void SetCardData(CardData cardData) {
        Data = cardData;
        var suitName = SuitPointTool.GetSuitName(Data.SuitNum);
        var pointName = SuitPointTool.GetPointName(Data.PointNum);
        var suitPoint = suitName + "-" + pointName;
        
        SetTexture(GD.Load<Texture2D>($"res://assets/pokers/{suitPoint}.png"));
    }
    
    private void SetTexture(Texture2D texture) {
        _texture = texture;
        
        foreach (var node in GetChildren()) {
            if (node.Owner == null) {
                node.QueueFree();
            }
        }

        if (_texture == null) {
            return;
        }

        // 新建 sprite 图案 和 碰撞体
        var sprite = new Sprite2D();
        sprite.Texture = _texture;
        sprite.Scale = Vector2.One * 0.5f; // 图片素材为默认的一半：200p
        AddChild(sprite);
        
        // 卡牌碰撞体的设置
        var rect = new RectangleShape2D();
        var collider = new CollisionShape2D();

        // 如果卡牌是右数第一张，碰撞体大小等于卡牌大小，否则等于卡牌间距大小
        if (IsLastOne) {
            rect.Size = _texture.GetSize() / 2;
        }
        else {
            rect.Size = new Vector2(ConstManager.CardSideDistance, _texture.GetSize().Y / 2f);
            collider.Position = Vector2.Left * (_texture.GetSize().X / 4 - ConstManager.CardSideDistance / 2f);
        }
        collider.Shape = rect;
        AddChild(collider);
    }
    
    /// <summary>
    /// 监听输入回调函数<br/>
    /// 主要用于处理第一下点击时的卡牌选中（此时 mouse 不会 entered）<br/>
    /// 操作与 OnMouseEntered 相同
    /// </summary>
    public override void _InputEvent(Viewport viewport, InputEvent @event, int shapeIdx) {
        if (!@event.IsActionPressed("interact")) {
            return;
        }
        var sprite = GetChild<Sprite2D>(0);
        sprite.Modulate = Colors.Gray;
        
        CardSelectedEvent?.Invoke(this);
    }

    /// <summary>
    /// 鼠标选中卡牌的操作<br/>
    /// 鼠标进入卡牌，且左键被按住，则添加灰色蒙版，并发送选中信号
    /// </summary>
    public void OnMouseEntered() {
        if (!MouseManager.Dragging) return;
        var sprite = GetChild<Sprite2D>(0);
        sprite.Modulate = Colors.Gray;
        
        CardSelectedEvent?.Invoke(this);
    }
    
    /// <summary>
    /// 卡牌选中/不选择后的处理：清除灰色蒙版，改变位置
    /// </summary>
    public void AfterSelected() {
        var sprite = GetChild<Sprite2D>(0);
        sprite.Modulate = Colors.White;
        Position = IsSelected ? new Vector2(Position.X, -ConstManager.CardSelectShift)
            : new Vector2(Position.X, 0);
    }
}
