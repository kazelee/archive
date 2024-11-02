# https://github.com/onestraw/doudizhu

extends Node

const DATA_PATH := "res://assets/data.json"
const TYPE_CARDS_PATH := "res://assets/type_cards.json"

var _point := PackedInt32Array([
	11, 12, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10,
	11, 12, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10,
	11, 12, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10,
	11, 12, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 13, 14
])

var _point_str := PackedStringArray([
	"3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A", "2", "BJ", "CJ"
])

const CARDS := ["3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A", "2", "BJ", "CJ"]
var CARD_IDX: Dictionary

var DATA: Dictionary
var TYPE_CARDS: Dictionary

func _ready() -> void:
	# assign CARDS_IDX
	for i in range(len(CARDS)):
		CARD_IDX[CARDS[i]] = i

	var file1 := FileAccess.open(DATA_PATH, FileAccess.READ)
	var json1 := file1.get_as_text()
	DATA = JSON.parse_string(json1) as Dictionary

	var file2 := FileAccess.open(TYPE_CARDS_PATH, FileAccess.READ)
	var json2 := file2.get_as_text()
	TYPE_CARDS = JSON.parse_string(json2) as Dictionary

	print(list_greater_cards("6-6-6-3-3", "CJ-A-A-A-K-Q-J-10-10-10-10-9-7-7-5-5"))


func point2str(id: int) -> String:
	return _point_str[_point[id]]


func idarr2str(cards: Array) -> String:
	var ordered_cards := Game.sorted(cards)
	ordered_cards.reverse()
	var new_cards := []
	for card in cards:
		new_cards.append(_point_str[_point[card]])
	var result := "-".join(new_cards)
	return result

"""
斗地主规则检查及比较器
~~~~~~~~~~~~~~~~~~~~~
枚举所有37种牌型，制作一个花色无关、顺序无关的字典，
能够在O(1)时间内判断出牌是否有效，在O(1)时间内比较大小
"""
func cards2str(cards: Array) -> String:
	return '-'.join(cards)


func str2cards(string: String) -> Array:
	return string.split('-') if string != "" else []


func str2cardmap(string: String) -> Dictionary:
	var cards := str2cards(string)
	var cardmap := {}
	for c in cards:
		if c in cardmap.keys():
			cardmap[c] += 1
		else:
			cardmap[c] = 1
	return cardmap


func sort_cards(cards: Array) -> Array:
	var new_cards := cards
	new_cards.sort_custom(func(a, b): return CARD_IDX[a] < CARD_IDX[b])
	return new_cards


func check_card_type(cards: Array) -> Array:
	var sorted_cards := sort_cards(cards)
	var value = DATA.get(cards2str(sorted_cards))
	return value if value else []


func type_greater(type_x: Array, type_y: Array) -> bool:
	print("compare %s with %s" % [type_x, type_y])
	"""check if x is greater than y
	type_x/y: (type, weight)
	>0: x > y
	=0: x = y
	<0: x < y
	"""
	if type_x[0] == type_y[0]:
		return type_x[1] > type_y[1]
	else:
		if type_x[0] == "rocket" and type_x[1] != -1:
			return true
		elif type_y[0] == "rocket" and type_y[1] != -1:
			return false
		elif type_x[0] == "bomb" and type_y[1] != -1:
			return true
	return false


func cards_greater(cards_x: Array, cards_y: Array) -> bool:
	"""check if x is greater than y
	x, y可能分别组成不同牌型
	只要有x一种牌型大于y，就返回True和牌型
	"""
	# x is me while y is other
	var type_x := check_card_type(cards_x)
	var type_y := check_card_type(cards_y)
	if type_x == [] and type_y != []:
		return false
	if type_x != [] and type_y == []:
		return true
	if type_x == [] and type_y == []:
		return false
	for tx in type_x:
		for ty in type_y:
			if type_greater(tx, ty):
				return true
	return false



func cards_contain(candidate_cardmap: Dictionary, cardmap: Dictionary):
	for k in cardmap.keys():
		if k not in candidate_cardmap:
			return false
		if candidate_cardmap[k] < cardmap[k]:
			return false
	return true


func list_greater_cards(cards_target: String, cards_candidate: String):
	""" 对于目标牌组合cards_target
	从候选牌cards_candidate中找出所有可以压制它的牌型

	1. 对于cards_taget同牌型的不同权重组合来说，按其最大权重计算
	如target='3-3-3-3-2-2-2-2', candidate='5-5-5-5-6-6-6-6'),
	这里target当作<四个2带2对3>，所以返回是：
	{'bomb': ['5-5-5-5', '6-6-6-6']}

	2. 对于candidate中一组牌可作不同组合压制cards_taget的场景，只返回一种组合
	如target='3-3-3-3-4-4-4-4', candidate='5-5-5-5-6-6-6-6'),
	<四个5带2对6>，<四个6带2对5> 均大于 <四个4带2对3>
	只返回一次'5-5-5-5-6-6-6-6',
	{'bomb': ['5-5-5-5', '6-6-6-6'], 'four_two_pair': ['5-5-5-5-6-6-6-6']}
	"""
	var target_type := check_card_type(str2cards(cards_target))
	if target_type == []:
		print("target is null")

	# 对target_type去重，保留同type中weight最大的
	var tmp_dict := {}
	for type_and_weight in target_type:
		var card_type: String = type_and_weight[0]
		var weight: int = type_and_weight[1]
		if card_type not in tmp_dict or weight > tmp_dict[card_type]:
			tmp_dict[card_type] = weight

	for key in tmp_dict.keys():
		target_type.append([key, tmp_dict[key]])

	# 如果目标牌型为rocket，则一定打不过，直接返回空
	if target_type[0][0] == 'rocket':
		return {}

	# 按牌型大小依次判断是否可用bomb, rocket
	if target_type[0][0] != 'rocket':
		if target_type[0][0] != 'bomb':
			target_type.append(['bomb', -1])
		target_type.append(['rocket', -1])
	elif target_type[0][0] != 'bomb':
		target_type.append(['bomb', -1])

#	print('target_type: {}'.format([target_type]))
	var candidate_cardmap := str2cardmap(cards_candidate)
	var cards_gt := {}


	for card_type_and_weight in target_type:
		var card_type: String = card_type_and_weight[0]
		var weight: int = card_type_and_weight[1]
		var weight_gt = []
		for w in TYPE_CARDS[card_type].keys():
			if int(w) > weight:
				weight_gt.append(w)
		if card_type not in cards_gt:
			cards_gt[card_type] = []

		weight_gt.sort_custom(func(a, b): return int(a) < int(b))

		for w in weight_gt:
			for w_cards in TYPE_CARDS[card_type][w]:
				var w_cardmap := str2cardmap(w_cards)
				if cards_contain(candidate_cardmap, w_cardmap) \
						and w_cards not in cards_gt[card_type]:
					cards_gt[card_type].append(w_cards)
		if not cards_gt[card_type]:
			cards_gt.erase(card_type)
	return cards_gt
