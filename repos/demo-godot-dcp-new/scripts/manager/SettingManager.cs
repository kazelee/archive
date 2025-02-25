using Godot;
using FileAccess = Godot.FileAccess;

namespace DouCardPuzzoom.scripts.manager; 

public static class SettingManager {
    public static string[] SupportLanguages = new[] {
        "en", "zh_CN", "zh_TW", "ja"
    };
    
    /// <summary>
    /// 语言，暂定只有英语、日语，简中和繁中，默认英语
    /// </summary>
    public static string Language; // = "en";

    public static void SetToNextLanguage() {
        GD.Print(Language);
        for (int i = 0; i < SupportLanguages.Length; i++) {
            if (SupportLanguages[i] == Language) {
                Language = SupportLanguages[(i + 1) % 4];
                TranslationServer.SetLocale(Language);
                DataLoader.UserSettings["locate"] = Language;
                DataLoader.StoreUserSettings();
                break; // 注释不能注释掉关键语句啊！！！
                
                // [test] 看看文件最终的结果是什么
                // var readFile = FileAccess.Open("user://settings.json", FileAccess.ModeFlags.Read);
                // GD.Print(readFile.GetAsText()); // 打印文件的内容
                // break;
            }
        }
    }
}