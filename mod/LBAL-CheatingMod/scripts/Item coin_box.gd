extends "res://Mod Data.gd"

func _init():
    author_id = "76561199138219743"
    mod_type = "item"
    type = "coin_box"
    inherit_effects = false
    inherit_art = false
    inherit_groups = false
    display_name = "Coin Box"
    localized_names = {"en": "Coin Box", "zh": "硬币盒子"}
    description = "<color_E14A68>Gain<end> <color_FBF236>1000<end> <icon_coin> each spin."
    localized_descriptions = {"en": "<color_E14A68>Gain<end> <color_FBF236>1000<end> <icon_coin> each spin.", "zh": "每次旋转<color_E14A68>获得<end><color_FBF236>1000<end>个<icon_coin>"}
    values = [1000]
    rarity = "common"
    groups = []
    sfx = []
    effects = [
        {
            "value_to_change": "value",
            "diff": values[0]
        }
    ]