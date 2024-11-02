import json
# name = "Symbols"
# name = "Items"
name = "Essences"

# Symbol: 去除d3_1-3 d5_1-5 dub(红叉) empty等（直接稀有度筛选就行）
with open(f"CheatingMod\{name}.json", 'r') as f:
    json_obj = json.load(f)
    new_obj = [i for i in json_obj.keys() if json_obj[i]["rarity"] in ["common", "uncommon", "rare", "very_rare", "essence"]]

s = ""
for i in new_obj:
    s = s + "{" + f"\"type\": \"{i}\"" + "}, "

print(s)
