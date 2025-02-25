using Godot;
using System;
using System.Collections.Generic;
using DouCardPuzzoom.scripts.classes;

public partial class AILeadUnit : Node2D
{
    public delegate void LeadCards(List<CardData> myCards, List<CardData> target);
    public AICardPlace AiCardPlace;
    public AILeadPlace AiLeadPlace;

    public override void _Ready() {
        AiCardPlace = GetNode<AICardPlace>("AICardPlace");
        AiLeadPlace = GetNode<AILeadPlace>("AILeadPlace");
    }
    
    
}
