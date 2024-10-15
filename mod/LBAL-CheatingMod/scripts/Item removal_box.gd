extends "res://Mod Data.gd"

func _init():
    author_id = "76561199138219743"
    mod_type = "item"
    type = "removal_box"
    inherit_effects = false
    inherit_art = false
    inherit_groups = false
    display_name = "Removal Box"
    localized_names = {"en": "Removal Box", "zh": "消除盒子"}
    description = "<color_E14A68>Gain<end> <color_FBF236>100<end> <icon_removal_token> each spin."
    localized_descriptions = {"en": "<color_E14A68>Gain<end> <color_FBF236>100<end> <icon_removal_token> each spin.", "zh": "每次旋转<color_E14A68>获得<end><color_FBF236>100<end>个<icon_removal_token>"}
    values = [100]
    rarity = "common"
    groups = []
    sfx = []
    effects = [
        {
            "value_to_change": "removal_value",
            "diff": values[0]
        }
    ]