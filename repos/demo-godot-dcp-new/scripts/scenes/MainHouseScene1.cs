using Godot;
using System;
using DouCardPuzzoom.scripts.manager;
using DouCardPuzzoom.scripts.scenes;

public partial class MainHouseScene1 : MainHouseScene {
    public NPC Landlord;
    // public Player Player;
    
    public InterArea Outdoor;
    public InterArea LandlordDoor;
    public InterArea HousekeeperDoor;
    public InterArea MyRoomDoor;
    public InterArea Stairs;

    public QuestionScreen QuestionScreen;
    public TempDialogueShower TempDialogueShower;
    
    // public Camera2D Camera2D;
    public override void _Ready() {
        base._Ready();
        // Camera2D = GetNode<Camera2D>("Player/Camera2D");
        var soundManager = GetNode<SoundManager>("/root/SoundManager");
        soundManager.PlayMusic("Main_floor1");
        
        Landlord = GetNode<NPC>("Landlord");
        Landlord.AiImage.Area2D.ResetForLandlord();
        
        Player = GetNode<Player>("Player");
        Player.Position = SaveManager.GetPosition(1);
        SaveManager.ResetPosition(1);

        Outdoor = GetNode<InterArea>("Doors/OutDoor");
        LandlordDoor = GetNode<InterArea>("Doors/LandlordDoor");
        HousekeeperDoor = GetNode<InterArea>("Doors/HouseKeeperDoor");
        MyRoomDoor = GetNode<InterArea>("Doors/MyDoor");
        Stairs = GetNode<InterArea>("Doors/Stairs");

        QuestionScreen = GetNode<QuestionScreen>("QuestionScreen");
        TempDialogueShower = GetNode<TempDialogueShower>("TempDialogueShower");

        Outdoor.OnInteracted += () => {
            TempDialogueShower.InitContent("T_ITSNOTIME");
            TempDialogueShower.Show();
            
            // var tmpDia = new TempDialogueShower();
            // if (GD.Load<PackedScene>("res://scenes/ui/TempDialogueShower.tscn").Instantiate() is TempDialogueShower tmpDia) {
            //     tmpDia.InitContent("T_ITSNOTIME");
            //     AddChild(tmpDia);
            // }
            // else {
            //     GD.PrintErr("加载 TempDialogueShower 失败！");
            // }

            MouseManager.IsInterAreaAble = false;
            
            // QuestionScreen.InitContent("Door-Outdoor");
            // QuestionScreen.Show();
            // Input.SetCustomMouseCursor(MouseManager.Arrow);
            // MouseManager.IsInterAreaAble = false;
        };
        
        LandlordDoor.OnInteracted += () => {
            QuestionScreen.InitContent("Door-C_LL");
            QuestionScreen.Show();
            Input.SetCustomMouseCursor(MouseManager.Arrow);
            MouseManager.IsInterAreaAble = false;
        };
        
        HousekeeperDoor.OnInteracted += () => {
            QuestionScreen.InitContent("Door-C_HK");
            QuestionScreen.Show();
            Input.SetCustomMouseCursor(MouseManager.Arrow);
            MouseManager.IsInterAreaAble = false;
        };
        
        MyRoomDoor.OnInteracted += () => {
            QuestionScreen.InitContent("Room-MyRoom");
            QuestionScreen.Show();
            Input.SetCustomMouseCursor(MouseManager.Arrow);
            MouseManager.IsInterAreaAble = false;
        };
        
        Stairs.OnInteracted += () => {
            QuestionScreen.InitContent("Stairs-1");
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
