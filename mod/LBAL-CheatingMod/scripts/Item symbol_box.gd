extends "res://Mod Data.gd"

func _init():
    author_id = "76561199138219743"
    mod_type = "item"
    type = "symbol_box"
    inherit_effects = false
    inherit_art = false
    inherit_groups = false
    display_name = "Symbol Box"
    localized_names = {"en": "Symbol Box", "zh": "符号盒子"}
    description = "You can choose a symbol once more from all symbols each spin. "
    localized_descriptions = {"en": "You can choose a symbol once more from all symbols each spin. ", "zh": "每次旋转，你可以从所有符号中再选择一个添加"}
    values = [142]
    rarity = "common"
    groups = []
    sfx = []
    effects = [
        {
            "emails_to_add": [{"type": "add_any_tile"}]
        }
    ]