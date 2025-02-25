using Godot;
using System;
using DouCardPuzzoom.scripts.manager;
using DouCardPuzzoom.scripts.scenes;

public partial class MainHouseScene3 : MainHouseScene
{
    // public Player Player;
    
    public InterArea Stairs;

    public QuestionScreen QuestionScreen;
    
    public override void _Ready() {
        base._Ready();
        var soundManager = GetNode<SoundManager>("/root/SoundManager");
        soundManager.PlayMusic("Main_floor3");
        
        Player = GetNode<Player>("Player");
        Player.Position = SaveManager.GetPosition(3);
        SaveManager.ResetPosition(3);
        
        Stairs = GetNode<InterArea>("Doors/Stairs");

        QuestionScreen = GetNode<QuestionScreen>("QuestionScreen");
        
        Stairs.OnInteracted += () => {
            QuestionScreen.InitContent("Stairs-3");
            QuestionScreen.Show();
            Input.SetCustomMouseCursor(MouseManager.Arrow);
            MouseManager.IsInterAreaAble = false;
        };
        // Landlord.OnDialoguePressed += ShowDialogue;
    }

    // public void ShowDialogue(string name) {
    //     var dialogueScreen = GD.Load<PackedScene>("res://scenes/ui/DialogueShower.tscn").Instantiate();
    //     AddChild(dialogueScreen);
    // }

    public override void _UnhandledInput(InputEvent @event) {
        if (@event.IsActionPressed("escape")) {
            var sceneChanger = GetNode<SceneChanger>("/root/SceneChanger");
            sceneChanger.ChangeScene("res://scenes/TitleScreen.tscn");
        }
    }
}
