# diff = 99 / 198 / 495 useless for items_to_add
# changed to 50 / 100 / 250

# def repeat(s, n):
#     res = "{" + f"\"type\": \"{s}\"" + "}, "
#     fin_res = res * n
#     print(f"\"items_to_add\": [{fin_res}],")

def repeat_list(lst:list, n:int):
    fin_res = ""
    for item in lst:
        res = "{" + f"\"type\": \"{item}\"" + "}, "
        fin_res += res * n
    print(f"\"items_to_add\": [{fin_res}],")

def repeat_list_with_smallest(lst:list, smallest:int):
    print(f"------{smallest}------")
    repeat_list(lst, smallest)
    print(f"------{smallest * 2}------")
    repeat_list(lst, smallest * 2)
    print(f"------{smallest * 5}------")
    repeat_list(lst, smallest * 5)

# print("------99------")
# repeat("white_pepper", 99)
# print("------198------")
# repeat("white_pepper", 198)
# print("------495------")
# repeat("white_pepper", 495)

# repeat_list_with_smallest(["coin_box", "item_box", "removal_box", "reroll_box", "symbol_box"], 20)

# repeat_list(["coin_box", "item_box", "removal_box", "reroll_box", "symbol_box"], 10)
# repeat_list_with_smallest(["coin_box", "removal_box", "reroll_box"], 10)
# repeat_list_with_smallest(["coin_box", "item_box", "removal_box", "reroll_box", "symbol_box", "essence_box"], 10)

repeat_list(["essence_box"], 10)
