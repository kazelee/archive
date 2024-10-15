extends "res://Mod Data.gd"

func _init():
    author_id = "76561199138219743"
    mod_type = "item"
    type = "item_box"
    inherit_effects = false
    inherit_art = false
    inherit_groups = false
    display_name = "Item Box"
    localized_names = {"en": "Item Box", "zh": "物品盒子"}
    description = "You may <color_E14A68>destroy<end> this item and <color_E14A68>add<end> all items once."
    localized_descriptions = {"en": "You may <color_E14A68>destroy<end> this item and <color_E14A68>add<end> all essence once.", "zh": "你可以<color_E14A68>消除<end>此物品，然后<color_E14A68>添加<end>所有的物品 1 次"}
    values = [10]
    manually_destroyable = true
    rarity = "common"
    groups = []
    sfx = []
    effects = [
        {
            "comparisons": [
                {"a": "destroyed", "b": true}
            ],
            "items_to_add": [{"type": "adoption_papers"}, {"type": "ancient_lizard_blade"}, {"type": "anthropology_degree"}, {"type": "bag_of_holding"}, {"type": "barrel_o_dwarves"}, {"type": "birdhouse"}, {"type": "black_cat"}, {"type": "black_pepper"}, {"type": "blue_pepper"}, {"type": "blue_suits"}, {"type": "booster_pack"}, {"type": "bowling_ball"}, {"type": "brown_pepper"}, {"type": "capsule_machine"}, {"type": "cardboard_box"}, {"type": "checkered_flag"}, {"type": "chicken_coop"}, {"type": "chili_powder"}, {"type": "cleaning_rag"}, {"type": "clear_sky"}, {"type": "coffee"}, {"type": "coin_on_a_string"}, {"type": "comfy_pillow"}, {"type": "compost_heap"}, {"type": "conveyor_belt"}, {"type": "copycat"}, {"type": "credit_card"}, {"type": "cursed_katana"}, {"type": "cyan_pepper"}, {"type": "dark_humor"}, {"type": "devils_deal"}, {"type": "dishwasher"}, {"type": "dwarven_anvil"}, {"type": "egg_carton"}, {"type": "fertilizer"}, {"type": "fifth_ace"}, {"type": "fish_bowl"}, {"type": "flush"}, {"type": "four_leaf_clover"}, {"type": "frozen_pizza"}, {"type": "fruit_basket"}, {"type": "frying_pan"}, {"type": "golden_carrot"}, {"type": "goldilocks"}, {"type": "grave_robber"}, {"type": "gray_pepper"}, {"type": "green_pepper"}, {"type": "guillotine"}, {"type": "happy_hour"}, {"type": "holy_water"}, {"type": "horseshoe"}, {"type": "jackolantern"}, {"type": "kyle_the_kernite"}, {"type": "lefty_the_rabbit"}, {"type": "lemon"}, {"type": "lime_pepper"}, {"type": "lint_roller"}, {"type": "lockpick"}, {"type": "looting_glove"}, {"type": "lucky_carrot"}, {"type": "lucky_cat"}, {"type": "lucky_dice"}, {"type": "lucky_seven"}, {"type": "lunchbox"}, {"type": "maxwell_the_bear"}, {"type": "mining_pick"}, {"type": "mobius_strip"}, {"type": "ninja_and_mouse"}, {"type": "nori_the_rabbit"}, {"type": "oil_can"}, {"type": "oswald_the_monkey"}, {"type": "piggy_bank"}, {"type": "pink_pepper"}, {"type": "pizza_the_cat"}, {"type": "pool_ball"}, {"type": "popsicle"}, {"type": "protractor"}, {"type": "purple_pepper"}, {"type": "quigley_the_wolf"}, {"type": "quiver"}, {"type": "rain_cloud"}, {"type": "recycling"}, {"type": "red_pepper"}, {"type": "red_suits"}, {"type": "reroll"}, {"type": "ricky_the_banana"}, {"type": "ritual_candle"}, {"type": "rusty_gear"}, {"type": "shattered_mirror"}, {"type": "shedding_season"}, {"type": "shrine"}, {"type": "sunglasses"}, {"type": "swapping_device"}, {"type": "swear_jar"}, {"type": "symbol_bomb_big"}, {"type": "symbol_bomb_quantum"}, {"type": "symbol_bomb_small"}, {"type": "symbol_bomb_very_big"}, {"type": "tax_evasion"}, {"type": "telescope"}, {"type": "time_machine"}, {"type": "treasure_map"}, {"type": "triple_coins"}, {"type": "turtle_and_rabbit"}, {"type": "undertaker"}, {"type": "void_party"}, {"type": "void_portal"}, {"type": "wanted_poster"}, {"type": "watering_can"}, {"type": "white_pepper"}, {"type": "x_ray_machine"}, {"type": "yellow_pepper"}, {"type": "zaroffs_contract"},]
        },
    ]