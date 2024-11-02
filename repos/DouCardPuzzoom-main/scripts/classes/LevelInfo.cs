using System;
using System.Collections.Generic;
using DouCardPuzzoom.scripts.enums;

namespace DouCardPuzzoom.scripts.classes; 

public class LevelInfo {
    public string Name;
    public string Id;
    public string Rule;
    public GameModes Mode;
    public int PassLimit;
    public int LeadLimit;
    public int PassRequest;
    public int LeadRequest;
    
    public int Number;
    public List<string> Players;
    public string Landlord;
    public List<List<CardData>> Cards;
    
    public override string ToString() {
        var content = $"Name: {Name}\nID: {Id}\nRule: {Rule}\nMode: {Mode}\n" +
                      $"PassLimit: {PassLimit}\nLeadLimit: {LeadLimit}\nNumber: {Number}\n" +
                      $"PassRequest: {PassRequest}\nLeadRequest: {LeadRequest}\nPlayers: [";
        foreach (var player in Players) {
            content += $"{player}, ";
        }
        // 拆分成逐个元素，也是为了兼容 Godot 和 C# 的不同输出结果
        content += $"]\nLandlord: {Landlord}\nCards:\n";
        foreach (var cards in Cards) {
            content += "\t[";
            foreach (var card in cards) {
                content += $"{card}, ";
            }

            content += "]\n";
        }

        return content;
    }
}