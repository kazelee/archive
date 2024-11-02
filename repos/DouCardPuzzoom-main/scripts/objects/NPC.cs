using Godot;
using System;
using DouCardPuzzoom.scripts.manager;
using DouCardPuzzoom.scripts.tools;

// using Label = System.Reflection.Emit.Label;

public partial class NPC : Node2D {
    [Export] public string NPCName;
    // public event Action<string> OnDialoguePressed;
    // public event Action<string> OnPlayCardsPressed;
    
    public AiImage AiImage;
    // public Area2D LeaveArea;

    public HBoxContainer Decisions;
    public Button ToDialogue;
    public Button ToPlayCards;
    public DialogueShower DialogueScreen;
    public CanvasLayer InteractScreen;
    public LevelScreen LevelScreen;
    public TextureRect CharacterIcon;

    // public bool DecisionHided = true;

    public Label CName;
    public Label PName;

    public TextureButton EscapeButton;

    public override void _Ready() {
        base._Ready();
        AiImage = GetNode<AiImage>("AiImage");
        // LeaveArea = GetNode<Area2D>("LeaveArea");
        Decisions = GetNode<HBoxContainer>("InteractScreen/Panel/Decisions");
        ToDialogue = GetNode<Button>("InteractScreen/Panel/Decisions/Dialogue");
        ToPlayCards = GetNode<Button>("InteractScreen/Panel/Decisions/PlayCards");
        DialogueScreen = GetNode<DialogueShower>("DialogueShower");
        EscapeButton = GetNode<TextureButton>("InteractScreen/Panel/Escape");
        LevelScreen = GetNode<LevelScreen>("LevelScreen");
        LevelScreen.InitDecks(NPCName);

        CharacterIcon = GetNode<TextureRect>("InteractScreen/IconPanel/CharacterIcon");
        CharacterIcon.Texture =
            GD.Load<Texture2D>($"res://assets/icons/{DialogueTool.GetNPCFirstIconName(NPCName)}.png");

        InteractScreen = GetNode<CanvasLayer>("InteractScreen");
        CName = GetNode<Label>("InteractScreen/Panel/TName");
        PName = GetNode<Label>("InteractScreen/Panel/PName");
        CName.Text = DialogueTool.GetDialogueNameTranslated(NPCName);
        PName.Text = DialogueTool.GetDialogueJobTranslated(NPCName);
        ToDialogue.Text = TranslationServer.Translate("BTN_DIA");
        ToPlayCards.Text = TranslationServer.Translate("BTN_PC");

        EscapeButton.Pressed += () => {
            InteractScreen.Hide();
            MouseManager.IsInterAreaAble = true; // 通过叉点出界面可以交互
        };
        
        // Decisions.Hide();
        AiImage.AiName = NPCName;
        AiImage.Play(NPCName); // new added
        AiImage.OnAiImageClicked += ShowButtons;
        // AiImage.OnImageAreaEntered += ShowButtons;
        // LeaveArea.MouseExited += HideButtons; // Buttons 似乎会无视 Area2D 的范围，导致进入button即消失
        ToDialogue.Pressed += () => {
            // 停掉背景音乐(tmp 就不用停了？）
            var soundManager = GetNode<SoundManager>("/root/SoundManager");
            soundManager.MusicPlayer.Stop();
            // soundManager.PlayMusicTmp("dialogue");
            
            MouseManager.IsInterAreaAble = false; // 进入对话，当然也不能运行背景交互
            // OnDialoguePressed?.Invoke(AiImage.AiName);
            DialogueScreen.Show();
            DialogueScreen.ShowDefaultContent();
            DialogueScreen.ShowChoiceButtons();
            HideButtons();
        };
        ToPlayCards.Pressed += () => {
            MouseManager.IsInterAreaAble = false;
            // OnPlayCardsPressed?.Invoke(AiImage.AiName);
            LevelScreen.Show();
            
            // 进入levelScreen时记录，通过右上角退出时置空？
            // NPC也获取不到主场景数据，还是需要信号……
            
            // LevelScreen.InitDecks(NPCName);
            HideButtons();
        };
    }

    public void ShowButtons(string _) {
        // if (DecisionHided) {
        MouseManager.IsInterAreaAble = false; // 显示交互菜单时，禁止Area交互和移动
        InteractScreen.Show();
            // Decisions.Show();
            // DecisionHided = false;
        // }
        // else {
            // InteractScreen.Hide();
            // Decisions.Hide();
            // DecisionHided = true;
        // }
    }

    public void HideButtons() {
        InteractScreen.Hide();
        // Decisions.Hide();
        // DecisionHided = true;
    }
    
}
