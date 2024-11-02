using DouCardPuzzoom.scripts.manager;
using DouCardPuzzoom.scripts.tools;
using Godot;
using Godot.Collections;

public partial class TempDialogueShower : CanvasLayer
{
    public DialoguePanel DialoguePanel;
    
    public Label CName;
    public Label PInfo;
    public DialogueContent Content;
    public AnimationPlayer AnimationPlayer;
    public Panel Panel;
    public TextureRect CharacterIcon;
    
    public Dictionary AllDialogues;
    public Array CurrentDialogueLines = new Array();
    public int CurrentLineIndex = -1;

    public override void _Ready() {
        base._Ready();
        CName = GetNode<Label>("DialoguePanel/CharacterInfo/Name");
        PInfo = GetNode<Label>("DialoguePanel/CharacterInfo/Info");
        Content = GetNode<DialogueContent>("DialoguePanel/DialogueContent");
        AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

        Panel = GetNode<Panel>("DialoguePanel/Panel");
        CharacterIcon = GetNode<TextureRect>("DialoguePanel/Panel/CharacterIcon");
        DialoguePanel = GetNode<DialoguePanel>("DialoguePanel");
        
        DialoguePanel.DialogueKeepGoing += GetNext;
        Content.DialogueKeepGoing += GetNext; // UI 边缘和文字都可以点，icon和名称不行

        AllDialogues = DialogueTool.LoadTempDialogue();
        Hide();
    }

    public void InitContent(string contentKey) {
        Show();
        CurrentDialogueLines = (Array)AllDialogues[contentKey];
        GetNext();
    }
    
    public void GetNext() {
        if (CurrentDialogueLines == null) return;
        
        if (CurrentLineIndex + 1 < CurrentDialogueLines.Count) {
            CurrentLineIndex += 1;
            ShowDialogueOneLine((Dictionary)CurrentDialogueLines[CurrentLineIndex]);
            // AnimationPlayer.Play("TextEdit");
            // Subjects.Hide();
            // SkipButton.Show();
        }
        else {
            EndDialogue();
        }
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
    
    public void EndDialogue() {
        CurrentLineIndex = -1;
        CurrentDialogueLines = null;
        Hide();
        MouseManager.IsInterAreaAble = true;
        // QueueFree();
    }
}
