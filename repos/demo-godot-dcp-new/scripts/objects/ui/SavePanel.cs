using Godot;
using System;
using DouCardPuzzoom.scripts;
using Array = Godot.Collections.Array;

public partial class SavePanel : Panel {
    public event Action ChosenOver;
    
    public string CurrentChosenButtonId;
    public GridContainer ButtonContainer;

    public Button SelectButton;
    public Button CreateButton;
    public Button DeleteButton;

    public Label HintText;
    public Tween HintTextTween;

    public TextureButton EscapeButton;

    public SaveCreator SaveCreator;

    public Button EnterButton;
    public Button CancelButton;
    public Panel DeleteScreen;
    public TextureButton DelEscape;

    public Label CurrentId;
    
    public override void _Ready() {
        base._Ready();
        ButtonContainer = GetNode<GridContainer>("ScrollContainer/ButtonContainer");
        SelectButton = GetNode<Button>("HBoxContainer/SelectNow");
        CreateButton = GetNode<Button>("HBoxContainer/CreateNew");
        DeleteButton = GetNode<Button>("HBoxContainer/DeleteNow");
        EscapeButton = GetNode<TextureButton>("Escape");
        
        HintText = GetNode<Label>("HintText");
        HintText.Hide();

        SaveCreator = GetNode<SaveCreator>("SaveCreator");
        SaveCreator.Hide();

        EnterButton = GetNode<Button>("DeleteScreen/Decisions/LeftButton");
        EnterButton.Pressed += () => {
            ((Array)DataLoader.UserSettings["saves"]).Remove(CurrentChosenButtonId);
            DataLoader.RemoveSave(CurrentChosenButtonId);
            InitSaves();
            DeleteScreen.Hide();
            CurrentChosenButtonId = "";
        };
        
        CancelButton = GetNode<Button>("DeleteScreen/Decisions/RightButton");
        CancelButton.Pressed += () => {
            DeleteScreen.Hide();
            CurrentChosenButtonId = "";
        };
        
        DeleteScreen = GetNode<Panel>("DeleteScreen");
        DeleteScreen.Hide();
        
        DelEscape = GetNode<TextureButton>("DeleteScreen/DelEscape");
        DelEscape.Pressed += () => { 
            DeleteScreen.Hide();
            CurrentChosenButtonId = "";
        };

        CurrentId = GetNode<Label>("DeleteScreen/CurrentId");
        
        SelectButton.Pressed += () => {
            if (CurrentChosenButtonId == "") {
                ShowHintTextWithKey("ERR_SELECTONE");
            }
            else {
                DataLoader.UserSettings["current_save"] = CurrentChosenButtonId;
                DataLoader.LoadCurrentSave();
                ChosenOver?.Invoke();
                // HideSelf();
            }
        };

        CreateButton.Pressed += ShowCreateScreen;
        SaveCreator.OnCreated += () => {
            InitSaves();
        };

        DeleteButton.Pressed += () => {
            if (CurrentChosenButtonId == "") {
                ShowHintTextWithKey("ERR_SELECTONE");
            }
            else if (((Array)DataLoader.UserSettings["saves"]).Count <= 1) {
                ShowHintTextWithKey("ERR_KEEPONESAVE");
                CurrentChosenButtonId = "";
            }
            else {
                ShowDeleteScreen();
            }
        };

        EscapeButton.Pressed += () => {
            ChosenOver?.Invoke();
        };
        
        InitSaves(); // 忘了加上了……
    }

    public void InitSaves() {
        foreach (var node in ButtonContainer.GetChildren()) {
            node.QueueFree();
        }
        
        foreach (var saveStr in (Array)DataLoader.UserSettings["saves"]) {
            var button = new Button();
            button.Text = $"No. {saveStr}";
            button.Pressed += () => {
                // GD.Print($"text: {button.Text}");
                var nameList = button.Text.Split(" ");
                // GD.Print($"list: {nameList}");
                CurrentChosenButtonId = nameList[1];
            };
            ButtonContainer.AddChild(button); // 忘了加上了……
        }
    }
    
    public void ShowHintTextWithKey(string key) {
        var soundManager = GetNode<SoundManager>("/root/SoundManager");
        soundManager.PlaySoundEffects("bell");
        
        HintTextTween?.Kill();
        HintText.Text = TranslationServer.Translate(key);
        HintText.Modulate = new Color(1, 1, 1, 0);
        HintText.Show();
        
        HintTextTween = GetTree().CreateTween();
        HintTextTween.TweenProperty(HintText, "modulate:a", 1.0, 0.3);
        HintTextTween.TweenProperty(HintText, "modulate:a", 0.0, 1.0).SetDelay(1);
        HintTextTween.TweenCallback(Callable.From(HintText.Hide));
    }

    public void ShowCreateScreen() {
        SaveCreator.Show();
    }

    public void ShowDeleteScreen() {
        CurrentId.Text = $"No. {CurrentChosenButtonId}";
        DeleteScreen.Show();
    }

    // public void HideSelf() {
    //     CurrentChosenButtonId = "";
    //     Hide();
    // }
}
