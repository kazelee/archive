using System.Collections.Generic;
using System.IO;
using System.Net;
using DouCardPuzzoom.scripts.manager;
using DouCardPuzzoom.scripts.tools;
using Godot;
using Godot.Collections;
using FileAccess = Godot.FileAccess;
using SCG = System.Collections.Generic;

namespace DouCardPuzzoom.scripts; 

/// <summary>
/// JSON格式数据加载部分，数据暴露给其他类调用
/// </summary>
public static class DataLoader {
	public static Dictionary UserSettings; // 始终在设置时即时保存到磁盘
	public static Dictionary CurrentSave; // 仅在退回页面或强退程序时，保存到磁盘
	// public static SCG.Dictionary<string, bool> CurrentLevels = new SCG.Dictionary<string, bool>();
	
	// RuleOri 的 Data 和 TypeCards
	public static Dictionary RuleOriData;
	public static Dictionary RuleOriTypeCards;

	public static Dictionary RuleTwoDeckData;
	public static Dictionary RuleTwoDeckTypeCards;

	// public static Dictionary RuleMahjongFollow;
	public static Dictionary RuleMahjongData;
	public static Dictionary RuleMahjongTypeCards;

	public static Dictionary RuleMathData;
	public static Dictionary RuleMathTypeCards;

	public static Dictionary RuleBlackJackData;
	public static Dictionary RuleBlackJackTypeCards;

	public static Dictionary RuleTexasData;
	public static Dictionary RuleTexasTypeCards;
	public static Dictionary RuleTexasFlushData;
	public static Dictionary RuleTexasFlushTypeCards;
	
	public static void Ready() {
		LoadUserSettings();
		
		LoadRuleOri();
		LoadRuleTwoDeck();
		LoadRuleMahjong();
		LoadRuleMath();
		LoadRuleBlackJack();
		LoadRuleTexas();
		
		GD.Print("Load DONE!");
	}

	public static void SetValueToGame() {
		SettingManager.Language = (string)UserSettings["locate"];
		// GD.Print(SettingManager.Language);
		TranslationServer.SetLocale(SettingManager.Language);
	}
	
	public static void LoadUserSettings() {
		// file:///C:\Users\LENOVO\AppData\Roaming\Godot\app_userdata\DouCardPuzzoom\setting.json
		
		if (FileAccess.FileExists("user://settings.json")) {
			using var readFile = FileAccess.Open("user://settings.json", FileAccess.ModeFlags.Read);
			var dataStr = readFile.GetAsText();
			UserSettings = Json.ParseString(dataStr).AsGodotDictionary();
		}
		else {
			using var writeFile = FileAccess.Open("user://settings.json", FileAccess.ModeFlags.Write);
			
			UserSettings = new Dictionary() {
				{ "locate", "en" },
				{ "sound", 0.5f },
				{ "music", 0.5f },
				{ "saves", new Array() { "001" } },
				{ "current_save", "001" }
			};
			var dataStr = Json.Stringify(UserSettings);
			writeFile.StoreString(dataStr);
			// writeFile.Close();
		}
		
		SetValueToGame();
		LoadUserSave((string)UserSettings["current_save"]);
	}

	// 便于其他类调用
	public static void LoadCurrentSave() {
		LoadUserSave((string)UserSettings["current_save"]);
	}
	
	public static void LoadUserSave(string saveId) {
		if (FileAccess.FileExists($"user://{saveId}.json")) {
			using var readFile = FileAccess.Open($"user://{saveId}.json", FileAccess.ModeFlags.Read);
			var dataStr = readFile.GetAsText();
			CurrentSave = Json.ParseString(dataStr).AsGodotDictionary();
		}
		else {
			using var writeFile = FileAccess.Open($"user://{saveId}.json", FileAccess.ModeFlags.Write);

			CurrentSave = new Dictionary() {
				{ "gained_objects", new Array() },
				{"last_level", CreateNewLastLevelDir()},
				{"pass_level", CreateNewPassLevelDir()},
				{"state", new Array()},
				{"dialogue", CreateNewDialogueDir()}
			};

			var dataStr = Json.Stringify(CurrentSave);
			writeFile.StoreString(dataStr);
			// writeFile.Close();
		}
	}

	// 包含添加到usersetting的部分
	public static void CreateSave(string saveId) {
		((Array)UserSettings["saves"]).Add(saveId); // 用户设置中添加
		using var writeFile = FileAccess.Open($"user://{saveId}.json", FileAccess.ModeFlags.Write);

		var saveData = new Dictionary() {
			{ "gained_objects", new Array() },
			{"last_level", CreateNewLastLevelDir()},
			{"pass_level", CreateNewPassLevelDir()},
			{"state", new Array()},
			{"dialogue", CreateNewDialogueDir()}
		};

		var dataStr = Json.Stringify(saveData);
		writeFile.StoreString(dataStr);
	}
	
	public static void RemoveSave(string saveId) {
		// 确保remove的时候，saves已经没有这个字符串了（这里不会再做删除）
		// 默认保存到回收站而不是直接删除
		OS.MoveToTrash(ProjectSettings.GlobalizePath($"user://{saveId}.json"));
	}
	
	public static Dictionary CreateNewLastLevelDir() {
		var newDic = new Dictionary();
		for (int i = 1; i <= 10; i++) {
			for (int j = 1; j <= 4; j++) {
				var levelName = $"L_{i}_{j}";
				newDic[levelName] = $"{i}{j}001";
			}
		}

		return newDic;
	}
	
	public static Dictionary CreateNewPassLevelDir() {
		var newDic = new Dictionary();
		for (int i = 1; i <= 10; i++) {
			for (int j = 1; j <= 4; j++) {
				var levelName = $"L_{i}_{j}";
				newDic[levelName] = new Array();
			}
		}

		return newDic;
	}

	public static Dictionary CreateNewDialogueDir() {
		var newDic = new Dictionary();
		newDic["Landlord"] = new Dictionary() {
			{
				"show", new Array() {
					"D_LL_HI", "D_LL_HOTEL", "D_LL_DOU", "D_LL_RULES", "D_LL_DIA", "D_LL_LL0"
				}
			},
			{ "read", new Array() }
		};
		newDic["Housekeeper"] = new Dictionary() {
			{
				"show", new Array() {
					"D_HK_HI", "D_HK_HOTEL", "D_HK_LL0"
				}
			},
			{ "read", new Array() }
		};
		newDic["ShuShu"] = new Dictionary() {
			{
				"show", new Array() {
					
				}
			},
			{ "read", new Array() }
		};
		newDic["Madeline"] = new Dictionary() {
			{
				"show", new Array() {
					
				}
			},
			{ "read", new Array() }
		};
		newDic["WangXiang"] = new Dictionary() {
			{
				"show", new Array() {
					
				}
			},
			{ "read", new Array() }
		};
		newDic["Albert"] = new Dictionary() {
			{
				"show", new Array() {
					
				}
			},
			{ "read", new Array() }
		};
		newDic["Student"] = new Dictionary() {
			{
				"show", new Array() {
					
				}
			},
			{ "read", new Array() }
		};
		newDic["Girl"] = new Dictionary() {
			{
				"show", new Array() {
					
				}
			},
			{ "read", new Array() }
		};
		newDic["Thief"] = new Dictionary() {
			{
				"show", new Array() {
					
				}
			},
			{ "read", new Array() }
		};
		newDic["Geek"] = new Dictionary() {
			{
				"show", new Array() {
					
				}
			},
			{ "read", new Array() }
		};
		
		// foreach (var npcName in DialogueTool.AllNPCName) {
		// 	// 每个人物初始的对话都不一样，必须逐一填写……
		// }
		return newDic;
	}

	public static void StoreUserSettings() {
		// C# 中必须使用 using var file 代替 var file？
		// 参考：https://docs.godotengine.org/zh-cn/4.x/classes/class_fileaccess.html#class-fileaccess-method-close
		// 这不就相当于是不用手动close了吗？但close有问题啊……
		using var writeFile = FileAccess.Open("user://settings.json", FileAccess.ModeFlags.Write);
		var dataStr = Json.Stringify(UserSettings);
		writeFile.StoreString(dataStr);
		// writeFile.Close(); // 只有关闭后才会flush读到磁盘中？反正反复读取必须要close？
	}

	public static void StoreCurrentSave() {
		GD.Print("SAVE!");
		var saveId = (string)UserSettings["current_save"];
		using var writeFile = FileAccess.Open($"user://{saveId}.json", FileAccess.ModeFlags.Write);

		var dataStr = Json.Stringify(CurrentSave);
		writeFile.StoreString(dataStr);
	}
	
	private static void LoadRuleOri() {
		var dataStr = File.ReadAllText(ProjectSettings.GlobalizePath("res://rule/RuleOri/Data.json"));
		RuleOriData = Json.ParseString(dataStr).AsGodotDictionary();
		
		var typeCardsStr = File.ReadAllText(ProjectSettings.GlobalizePath("res://rule/RuleOri/TypeCards.json"));
		RuleOriTypeCards = Json.ParseString(typeCardsStr).AsGodotDictionary();
	}

	public static void LoadRuleTwoDeck() {
		var dataStr = File.ReadAllText(ProjectSettings.GlobalizePath("res://rule/RuleTwoDeck/Data.json"));
		RuleTwoDeckData = Json.ParseString(dataStr).AsGodotDictionary();
		
		var typeCardsStr = File.ReadAllText(ProjectSettings.GlobalizePath("res://rule/RuleTwoDeck/TypeCards.json"));
		RuleTwoDeckTypeCards = Json.ParseString(typeCardsStr).AsGodotDictionary();
	}
	
	public static void LoadRuleMahjong() {
		var dataStr = File.ReadAllText(ProjectSettings.GlobalizePath("res://rule/RuleMahjong/Data.json"));
		RuleMahjongData = Json.ParseString(dataStr).AsGodotDictionary();
		
		var typeCardsStr = File.ReadAllText(ProjectSettings.GlobalizePath("res://rule/RuleMahjong/TypeCards.json"));
		RuleMahjongTypeCards = Json.ParseString(typeCardsStr).AsGodotDictionary();
	}
	
	public static void LoadRuleMath() {
		var dataStr = File.ReadAllText(ProjectSettings.GlobalizePath("res://rule/RuleMath/Data.json"));
		RuleMathData = Json.ParseString(dataStr).AsGodotDictionary();
		
		var typeCardsStr = File.ReadAllText(ProjectSettings.GlobalizePath("res://rule/RuleMath/TypeCards.json"));
		RuleMathTypeCards = Json.ParseString(typeCardsStr).AsGodotDictionary();
	}

	public static void LoadRuleBlackJack() {
		var dataStr = File.ReadAllText(ProjectSettings.GlobalizePath("res://rule/RuleBlackJack/Data.json"));
		RuleBlackJackData = Json.ParseString(dataStr).AsGodotDictionary();
		
		var typeCardsStr = File.ReadAllText(ProjectSettings.GlobalizePath("res://rule/RuleBlackJack/TypeCards.json"));
		RuleBlackJackTypeCards = Json.ParseString(typeCardsStr).AsGodotDictionary();
	}

	public static void LoadRuleTexas() {
		var dataStr = File.ReadAllText(ProjectSettings.GlobalizePath("res://rule/RuleTexas/Data.json"));
		RuleTexasData = Json.ParseString(dataStr).AsGodotDictionary();
		
		var typeCardsStr = File.ReadAllText(ProjectSettings.GlobalizePath("res://rule/RuleTexas/TypeCards.json"));
		RuleTexasTypeCards = Json.ParseString(typeCardsStr).AsGodotDictionary();
		
		var dataStrF = File.ReadAllText(ProjectSettings.GlobalizePath("res://rule/RuleTexas/FlushData.json"));
		RuleTexasFlushData = Json.ParseString(dataStrF).AsGodotDictionary();
		
		var typeCardsStrF = File.ReadAllText(ProjectSettings.GlobalizePath("res://rule/RuleTexas/FlushTypeCards.json"));
		RuleTexasFlushTypeCards = Json.ParseString(typeCardsStrF).AsGodotDictionary();
	}
	
}
