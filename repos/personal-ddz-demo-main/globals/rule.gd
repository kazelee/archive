extends Node

signal score_doubled

const PATH := "res://assets/rule.json"

# A: 11, 2: 12, 3: 0
var _point := PackedInt32Array([11, 12, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 13, 14])

var _point_str := PackedStringArray(["3", "4", "5", "6", "7", "8", "9", "0", "J", "Q", "K", "A", "2", "w", "W"])

# 最大数量，带什么，重复数量
#var type_2_num := {
#	"seq_single7": [1, 0, 7],
#	"seq_pair6": [2, 0, 6],
#	"bomb_single": [4, 1, 1],
#	"seq_trio_pair2": [3, 2, 2],
#	"seq_trio_pair5": [3, 2, 5],
#	"seq_pair7": [3, 2, 7],
#	"trio_single": [3, 1, 1],
#	"seq_single11": [1, 0, 11],
#	"seq_trio6": [3, 0, 6],
#	"seq_pair8": [2, 0, 8],
#	"seq_pair3": [2, 0, 3],
#	"seq_trio_pair4": [3, 2, 4],
#	"seq_single10": [1, 0, 10],
#	"seq_single9": [1, 0, 9],
#	"seq_single5": [1, 0, 5],
#	"seq_trio2": [3, 0, 2],
#	"pair": [2, 0, 1],
#	"trio": [3, 0, 1],
#	"seq_pair5": [2, 0, 5],
#	"bomb": [4, 0, 1],
#	"seq_trio5": [3, 0, 5],
#	"seq_single12": [1, 0, 12],
#	"seq_trio_pair3": [3, 2, 3],
#	"bomb_pair": [4, 2, 1],
#	"seq_trio_single4": [3, 1, 4],
#	"seq_trio_single5": [3, 1, 5],
#	"seq_pair10": [2, 0, 10],
#	"seq_single8": [1, 0, 8],
#	"seq_trio_single3": [3, 1, 3],
#	"seq_trio4": [3, 0, 4],
#	"seq_pair4": [2, 0, 4],
#	"rocket": [0, 0, 1],
#	"seq_pair9": [2, 0, 9],
#	"seq_single6": [1, 0, 6],
#	"seq_trio_single2": [3, 1, 2],
#	"seq_trio3": [3, 0, 3],
#	"trio_pair": [3, 2, 1],
#	"single": [1, 0, 1],
#}

enum State {
	FIRST, NONE, FOLLOW
}

#var hint_cards := []
var card_types := {}

var last_type := []: set = set_type
#var last_cards := []


func set_type(v: Array) -> void:
	last_type = v
	if v == []:
		return
	if last_type[0] == "bomb" or last_type[0] == "rocket":
		score_doubled.emit()


func _ready() -> void:
	var file := FileAccess.open(PATH, FileAccess.READ)
	var json := file.get_as_text()
	var data := JSON.parse_string(json) as Dictionary
	for k in data.keys():
		var i := 0
		for type in data[k]:
			card_types[type] = [k, i]
			i += 1


#func can_follow_cards(cards: Array) -> bool:
##	var flag := false
#	if last_type[0] == "rocket":
#		return false
#
#	var type_array: Array = type_2_num[last_type[0]]
#	var count_array := PackedInt32Array()
#	count_array.resize(15)
#	count_array.fill(0)
#	var target := PackedInt32Array()
#	count_array.resize(15)
#	count_array.fill(0)
#	for card in cards:
#		count_array[_point[card]] += 1
#	for card in Game.biggest_lead:
#		target[_point[card]] += 1
#	var type_string := ""
#	var target_string := ""
#	for count in count_array:
#		type_string += str(count)
#	for count in target:
#		target_string += str(count)
#
#	if type_string.ends_with("11"):
##		hint_cards.append([52, 53])
##		flag = true
#		return true
#	if type_string.contains("4") and (last_type[0] != "bomb" or target_string.find("4") < type_string.find("4")):
##		hint_cards.append()
#		return true
#	if not type_string.contains("4") and last_type[0] == "bomb":
#		return false
#
#	var type_num :Array = type_2_num[last_type[0]]
#	var index = type_string.find(str(type_num[0]).repeat(type_num[2]))
#	if index == -1:
#		return false
#	if index > target_string.find(str(type_num[0]).repeat(type_num[2])):
#		if type_num[0] == 4 and type_num[1] == 1 and cards.size() >= 6 and type_string.contains("1"):
#			return true
#
#	# [abandoned]
#
#	return true


func get_lead_state(id: int) -> State:
	if Game.biggest_lead == []:
		return State.FIRST
	return State.FOLLOW
#	if can_follow_cards(Game.get_cards(id)):
#		return State.FOLLOW
#	else:
#		return State.NONE


func is_valid(cards: Array) -> bool:
	var ordered_cards := Game.sorted(cards)
	ordered_cards.reverse()
	var type_string := ""
	for card in cards:
		type_string += _point_str[_point[card]]
	var result = card_types.get(type_string)
	if result == null:
		return false
	if last_type == [] or \
	last_type[0] == result[0] and last_type[1] < result[1] or \
	last_type[0] != result[0] and last_type[0] != "rocket" and result[0] == "bomb" or \
	result[0] == "rocket":
		rpc("set_last_type", result)
#		print("new")
#		print(last_type)
		return true
#	elif last_type[0] == result[0] and last_type[1] < result[1] or result[0] == "bomb":
#		rpc("set_last_type", result)
#		print(last_type)
#		return true
	else:
		return false


@rpc("any_peer", "call_local")
func set_last_type(type: Array) -> void:
	last_type = type.duplicate(true)


#func get_type(cards: Array) -> String:
#	if cards == [52, 53]:
#		return "BR"
#	var count_array := PackedInt32Array()
#	count_array.resize(15)
#	count_array.fill(0)
#	for card in cards:
#		count_array[_weight[card]] += 1
#	var type_string := ""
#	for count in count_array:
#		var tmp := 0
#		if count != 0 or count != tmp:
#			tmp = count
#			type_string = type_string + str(count)
#	match type_string:
#		"1":
#			return "110" + str(count_array.find(1))
#		"11111":
#			return "150" + str(count_array.find(1))
#		_:
#			return "N"
