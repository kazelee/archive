using Godot;
using System;
using DouCardPuzzoom.scripts.enums;
using DouCardPuzzoom.scripts.managers;

public partial class LeadCardBar : HBoxContainer {
    public event Action<LeadStates> HasLedCards;
    public Button PassButton;
    public Button HintButton;
    public Button PlayButton;

    public override void _Ready() {
        base._Ready();
        PassButton = GetNode<Button>("PassButton");
        HintButton = GetNode<Button>("HintButton");
        PlayButton = GetNode<Button>("PlayButton");

        PassButton.Pressed += OnPassButtonPressed;
        HintButton.Pressed += OnHintButtonPressed;
        PlayButton.Pressed += OnPlayButtonPressed;
    }

    public void InitButtons() {
        switch (StateManager.CurrentState) {
            case StateManager.States.First:
                PassButton.Hide();
                HintButton.Hide();
                PlayButton.Show();
                break;
            case StateManager.States.None:
                PassButton.Show();
                PlayButton.Hide();
                HintButton.Hide();
                break;
            case StateManager.States.Follow:
                PassButton.Show();
                PlayButton.Show();
                HintButton.Show();
                break;
        }
    }

    public void OnPassButtonPressed() {
        HasLedCards(LeadStates.Pass);
    }
    public void OnHintButtonPressed() {
        HasLedCards(LeadStates.Hint);
    }
    public void OnPlayButtonPressed() {
        HasLedCards(LeadStates.Play);
    }
    
}
