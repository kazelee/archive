using Godot;
using System;
using DouCardPuzzoom.scripts;
using DouCardPuzzoom.scripts.manager;
using DouCardPuzzoom.scripts.tools;
using Godot.Collections;
using Array = Godot.Collections.Array;

public partial class DialogueShower : CanvasLayer {
    /// <summary>
    /// 直接写成对应的翻译key值？（不行，因为还有State文件？）
    /// </summary>
    [Export] public string CharaName;

    public Button SkipButton;
    public Button LeaveButton;
    public DialoguePanel DialoguePanel;
    public ScrollContainer Subjects;
    public VBoxContainer ChoiceButtons;
    public AnimationPlayer AnimationPlayer;
    public Panel Panel;
    public TextureRect CharacterIcon;

    public Label CName;
    public Label PInfo;
    
    public Dictionary AllDialogues;
    public Dictionary CurrentDialogueState;
    public DialogueContent Content;

    public Array CurrentDialogueLines;
    // public Dictionary CurrentDialogueLine;
    public int CurrentLineIndex = -1;
    
    public override void _Ready() {
        base._Ready();
        SkipButton = GetNode<Button>("JumpFromThis/SkipThat");
        LeaveButton = GetNode<Button>("JumpFromThis/JumpOut");
        DialoguePanel = GetNode<DialoguePanel>("DialoguePanel");
        Subjects = GetNode<ScrollContainer>("Subjects");
        ChoiceButtons = GetNode<VBoxContainer>("Subjects/VBoxContainer");
        AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

        Panel = GetNode<Panel>("DialoguePanel/Panel");
        CharacterIcon = GetNode<TextureRect>("DialoguePanel/Panel/CharacterIcon");

        CName = GetNode<Label>("DialoguePanel/CharacterInfo/Name");
        PInfo = GetNode<Label>("DialoguePanel/CharacterInfo/Info");
        Content = GetNode<DialogueContent>("DialoguePanel/DialogueContent");

        DialoguePanel.DialogueKeepGoing += GetNext;
        Content.DialogueKeepGoing += GetNext; // UI 边缘和文字都可以点，icon和名称不行
        
        LeaveButton.Pressed += HideSelf;
        SkipButton.Pressed += EndDialogue;
        LeaveButton.Text = TranslationServer.Translate("BTN_LEAVEDIA");
        SkipButton.Text = TranslationServer.Translate("BTN_JUMPDIA");
        SkipButton.Hide();

        AllDialogues = DialogueTool.LoadDialogue(CharaName); // 引用传值，可以直接修改到原字典
        // GD.Print(AllDialogues);
        InitDialogue();
        ShowDefaultContent(true);
    }
    
    // public void AfterOneDialogueEnd() {
    //     // 就是初始化函数罢了
    // }
    
    public void InitDialogue() { // arg: string name
        // AllDialogues = DialogueTool.LoadDialogue(name);
        // [Test]
        // var readFile = FileAccess.Open("res://scripts/test/001.json", FileAccess.ModeFlags.Read);
        // var dataStr = readFile.GetAsText();
        // var dataDir = Json.ParseString(dataStr).AsGodotDictionary();
        var dataDir = DataLoader.CurrentSave;
        var dialogueState = (Dictionary)dataDir["dialogue"];
        CurrentDialogueState = (Dictionary)dialogueState[CharaName];

        ShowChoiceButtons();
    }

    public void ShowChoiceButtons() {
        foreach (var button in ChoiceButtons.GetChildren()) {
            button.QueueFree();
        }
        
        foreach (var buttonStr in (Array)CurrentDialogueState["show"]) {
            var button = new Button();
            button.Text = TranslationServer.Translate((string)buttonStr);
            button.Pressed += () => {
                LoadDialogue((string)buttonStr);
                ((Array)CurrentDialogueState["show"]).Remove(buttonStr);
                ((Array)CurrentDialogueState["read"]).Add(buttonStr);
                // [TRY] Load Back
                ((Dictionary)DataLoader.CurrentSave["dialogue"])[CharaName] = CurrentDialogueState;
            };
            ChoiceButtons.AddChild(button);
        }

        foreach (var buttonStr in (Array)CurrentDialogueState["read"]) {
            var button = new Button();
            button.Text = TranslationServer.Translate((string)buttonStr);
            button.Theme = GD.Load<Theme>("res://assets/PurpleButton.tres");
            button.Pressed += () => {
                LoadDialogue((string)buttonStr);
            };
            ChoiceButtons.AddChild(button);
        }
    }
    
    public void LoadDialogue(string id) {
        CurrentDialogueLines = (Array)AllDialogues[id];
        GetNext();
    }

    public void ShowDialogueOneLine(Dictionary oneLine) {
        if ((string)oneLine["name"] == "YOU") {
            CName.Text = TranslationServer.Translate("YOU");
            PInfo.Text = "";
            CName.Show();
            PInfo.Hide();
        }
        else if ((string)oneLine["name"] == "") { // 为空表示旁白/叙述
            CName.Hide();
            PInfo.Hide();
        }
        else {
            CName.Text = TranslationServer.Translate((string)oneLine["name"] + "_N");
            PInfo.Text = TranslationServer.Translate((string)oneLine["name"]);
            CName.Show();
            PInfo.Show();
        }

        if ((string)oneLine["icon"] == "") { // 为空表示旁白/叙述
            Panel.Hide();
        }
        else {
            CharacterIcon.Texture = GD.Load<Texture2D>($"res://assets/icons/{oneLine["icon"]}");
            Panel.Show();
        }
        Content.Text = TranslationServer.Translate((string)oneLine["content"]);

        var soundManager = GetNode<SoundManager>("/root/SoundManager");
        // 7.5 - 8.5, 长 -> 短
        var soundTime = 8.5f - Content.Text.Length / 100f;
        soundManager.PlaySoundEffects("text", soundTime);
        
        AnimationPlayer.Play("TextEdit");
    }

    public void ShowDefaultContent(bool isReady = false) {
        CName.Text = TranslationServer.Translate("YOU");
        PInfo.Text = "";
        CName.Show();
        PInfo.Hide();
        CharacterIcon.Texture = GD.Load<Texture2D>("res://assets/icons/I_ME_1.png");
        if (((Array)CurrentDialogueState["show"]).Count == 0) {
            Content.Text = TranslationServer.Translate("NOQUS");
        }
        else {
            Content.Text = TranslationServer.Translate("ANYQUS");
        }

        if (!isReady) {
            var soundManager = GetNode<SoundManager>("/root/SoundManager");
            // 7.5 - 8.5, 长 -> 短
            var soundTime = 8.5f - Content.Text.Length / 100f;
            soundManager.PlaySoundEffects("text", soundTime);
        }
        
        AnimationPlayer.Play("TextEdit");
    }
    
    public void HideSelf() {
        Hide();
        // EndDialogue();
        CurrentLineIndex = -1;
        CurrentDialogueLines = null;
        // ShowDefaultContent();
        ShowChoiceButtons();
        Subjects.Show();
        SkipButton.Hide();
        
        MouseManager.IsInterAreaAble = true;
        
        var soundManager = GetNode<SoundManager>("/root/SoundManager");
        // soundManager.MusicPlayer.Play();
        soundManager.PlayCurrent();
        
        // 每次退出对话的时候，保存一次存档
        DataLoader.StoreCurrentSave();
    }

    public void GetNext() {
        if (CurrentDialogueLines == null) return;
        
        if (CurrentLineIndex + 1 < CurrentDialogueLines.Count) {
            CurrentLineIndex += 1;
            ShowDialogueOneLine((Dictionary)CurrentDialogueLines[CurrentLineIndex]);
            // AnimationPlayer.Play("TextEdit");
            Subjects.Hide();
            SkipButton.Show();
        }
        else {
            EndDialogue();
        }
    }

    public void EndDialogue() {
        CurrentLineIndex = -1;
        CurrentDialogueLines = null;
        ShowDefaultContent();
        ShowChoiceButtons();
        Subjects.Show();
        SkipButton.Hide();
    }
    
}
