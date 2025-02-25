using Godot;
using System;
using DouCardPuzzoom.scripts.enums;
using DouCardPuzzoom.scripts.managers;
using DouCardPuzzoom.scripts.utils;

public partial class PlayerLeadUnit : Node2D
{
    public MyCardPlace MyCardPlace;
    public LeadCardBar LeadCardBar;
    public LeadPlace LeadPlace;
    
    public override void _Ready() {
        MyCardPlace = GetNode<MyCardPlace>("MyCardPlace");
        LeadCardBar = GetNode<LeadCardBar>("LeadCardBar");
        LeadPlace = GetNode<LeadPlace>("LeadPlace");
        
        LeadCardBar.InitButtons();
        LeadCardBar.HasLedCards += UploadCards;
    }

    public override void _UnhandledInput(InputEvent @event) {
        if (@event is InputEventMouseButton { ButtonIndex: MouseButton.Left } button) {
            if (button.IsPressed()) {
                MouseManager.Dragging = true;
            }
            else if (button.IsReleased()) {
                MouseManager.Dragging = false;
                MyCardPlace.AfterSelectCards();
            }
        }
    }

    public void UploadCards(LeadStates leadState) {
        switch (leadState) {
            case LeadStates.Play:
                var leadCards = MyCardPlace.WillBeLeadCardDatas;
                // 出牌不允许不选牌
                if (leadCards.Count == 0) {
                    return;
                }
                
                leadCards = CardTool.SortedForLead(leadCards);
                // GD.PrintS(leadCards);
                LeadPlace.UpdateCards(leadCards);
                MyCardPlace.AfterUploadCards();
                LeadCardBar.Hide();
                break;
            
            case LeadStates.Hint:
                break;
            
            case LeadStates.Pass:
                LeadPlace.Pass();
                LeadCardBar.Hide();
                break;
        }
    }
    
}
