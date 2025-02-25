using Godot;
using System;

public partial class Interactable : Area2D {
    public event Action OnAreaInteracted;
    public CollisionShape2D CollisionShape2D;

    public override void _Ready() {
        base._Ready();
        CollisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");
    }

    public override void _InputEvent(Viewport viewport, InputEvent @event, int shapeIdx) {
        if (!@event.IsActionPressed("interact")) {
            return;
        }
        
        OnAreaInteracted?.Invoke();
    }

    // 下面的函数只为 GS 中的人物形象服务，其他Area2D节点也会应用此脚本，但不调用下面的函数
    
    public void ResetRect() {
        var rect = new RectangleShape2D();
        rect.Size = new Vector2(26, 36);
        CollisionShape2D.Shape = rect;
        CollisionShape2D.Position = Vector2.Zero;
    }
    
    public void ResetRectForUp() {
        var rect = new RectangleShape2D();
        rect.Size = new Vector2(26, 22);
        CollisionShape2D.Shape = rect;
        CollisionShape2D.Position = new Vector2(0, -7);
    }

    public void ResetForLandlord() {
        var rect = new RectangleShape2D();
        rect.Size = new Vector2(27.25f, 27.375f);
        CollisionShape2D.Shape = rect;
        CollisionShape2D.Position = new Vector2(0, -5);
    }
}
