using Godot;
using System;
using DouCardPuzzoom.scripts.enums;
using DouCardPuzzoom.scripts.manager;

public partial class AiImage : AnimatedSprite2D {
    public Interactable Area2D;
    public PlacePositions Place;
    [Export] public string AiName;
    public event Action<string> OnAiImageClicked;
    public event Action OnImageAreaEntered;
    
    public override void _Ready() {
        base._Ready();
        Area2D = GetNode<Interactable>("InteractArea");
        Area2D.OnAreaInteracted += AfterInteract;
        Area2D.MouseEntered += ChangeMouse2Click;
        Area2D.MouseExited += ChangeMouse2Arrow;
        // Hide();
        Play(AiName);
    }
    
    public void InitAiImage(string name, PlacePositions place) {
        AiName = name;
        Place = place;
        Play(AiName);
        switch (Place) {
            case PlacePositions.Up:
                Position = new Vector2(0, -20);
                Area2D.ResetRectForUp();
                break;
            case PlacePositions.Right:
                Position = new Vector2(100, -10);
                Area2D.ResetRect();
                break;
            case PlacePositions.Left:
                Position = new Vector2(-100, -10);
                Area2D.ResetRect();
                FlipH = true;
                break;
            case PlacePositions.Down:
                break;
            // throw new ArgumentOutOfRangeException();
        }
        Show();
    }

    public void ChangeMouse2Click() {
        if (!MouseManager.IsInterAreaAble) return; // 变色的逻辑
        Modulate = Colors.Gray;
        Input.SetCustomMouseCursor(MouseManager.Click);
        OnImageAreaEntered?.Invoke();
    }
    
    public void ChangeMouse2Arrow() {
        Modulate = Colors.White;
        Input.SetCustomMouseCursor(MouseManager.Arrow);
    }

    public void AfterInteract() {
        if (!MouseManager.IsInterAreaAble) return; // 交互的逻辑
        
        // 所有的人物点击（无论NPC还是GS中的AI）都在点击时发出声音
        var soundManager = GetNode<SoundManager>("/root/SoundManager");
        soundManager.PlayAreaEffects("high-click");
        // soundManager.AreaSFXPlayer.Play(0.5f);
        
        OnAiImageClicked?.Invoke(AiName);
    }
    
}
