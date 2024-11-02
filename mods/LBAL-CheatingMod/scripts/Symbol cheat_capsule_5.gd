extends "res://Mod Data.gd"

func _init():
    author_id = "76561199138219743"
    mod_type = "symbol"
    type = "cheat_capsule_5"
    inherit_effects = false
    inherit_art = false
    inherit_groups = false
    inherit_description = false
    display_name = "Cheat Capsule"
    localized_names = {"en": "Cheat Capsule", "zh": "作弊扭蛋"}
    value = 0
    description = "<color_E14A68>Destroys<end> itself. Adds <icon_coin_box> , <icon_reroll_box> , <icon_removal_box> , <icon_symbol_box> , <color_FBF236>10<end> <icon_item_box> and <color_FBF236>10<end> <icon_essence_box> ."
    localized_descriptions = {"en": "<color_E14A68>Destroys<end> itself. Adds <icon_coin_box> , <icon_reroll_box> , <icon_removal_box> , <icon_symbol_box> , <color_FBF236>10<end> <icon_item_box> and <color_FBF236>10<end> <icon_essence_box> .", "zh": "<color_E14A68>消除自身<end>，添加<icon_coin_box> , <icon_reroll_box> , <icon_removal_box> , <icon_symbol_box> , <color_FBF236>10<end> <icon_item_box> 和 <color_FBF236>10<end> <icon_essence_box> "}
    values = []
    values = []
    rarity = "common"
    groups = []
    sfx = []
    effects = [
        {
            "value_to_change": "destroyed",
            "diff": true,
            "anim": "shake",
            "sfx_override": "growhit",
        },
        {
            "comparisons": [
                {"a": "destroyed", "b": true}
            ],
            "items_to_add": [
                {"type": "coin_box"},
                {"type": "reroll_box"},
                {"type": "removal_box"},
                {"type": "symbol_box"},

                {"type": "item_box"}, 
                {"type": "item_box"}, 
                {"type": "item_box"}, 
                {"type": "item_box"}, 
                {"type": "item_box"}, 
                {"type": "item_box"}, 
                {"type": "item_box"}, 
                {"type": "item_box"}, 
                {"type": "item_box"}, 
                {"type": "item_box"}, 

                {"type": "essence_box"}, 
                {"type": "essence_box"}, 
                {"type": "essence_box"}, 
                {"type": "essence_box"}, 
                {"type": "essence_box"}, 
                {"type": "essence_box"}, 
                {"type": "essence_box"}, 
                {"type": "essence_box"}, 
                {"type": "essence_box"}, 
                {"type": "essence_box"}, 
            ],
        },
    ]