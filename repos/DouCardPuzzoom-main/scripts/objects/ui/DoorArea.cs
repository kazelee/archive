using Godot;
using System;
using DouCardPuzzoom.scripts.manager;

public partial class DoorArea : Area2D {
    public event Action OnDoorOpened;

    public Sprite2D DoorSprite;
    
    public override void _Ready() {
        base._Ready();
        DoorSprite = GetNode<Sprite2D>("DoorSprite");
        MouseEntered += () => {
            if (!MouseManager.IsInterAreaAble) return; // 还是有必要的……
            DoorSprite.Modulate = Colors.Gray;
            Input.SetCustomMouseCursor(MouseManager.Click);
        };
        MouseExited += () => {
            DoorSprite.Modulate = Colors.White;
            Input.SetCustomMouseCursor(MouseManager.Arrow);
        };
    }
    
    public override void _InputEvent(Viewport viewport, InputEvent @event, int shapeIdx) {
        if (!@event.IsActionPressed("interact")) {
            return;
        }
        
        // DoorSprite.Modulate = Colors.White;
        // Input.SetCustomMouseCursor(MouseManager.Arrow);
        
        OnDoorOpened?.Invoke();
    }
}
