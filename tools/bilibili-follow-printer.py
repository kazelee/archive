'''bilibili 关注导出成 md 文件（附头像）'''

import requests
import json
import time
import re

def validateTitle(title):
    """将字符串中不能作为标题的字符转为下划线"""
    rstr = r"[\/\\\:\*\?\"\<\>\|]"  # '/ \ : * ? " < > |'
    new_title = re.sub(rstr, "_", title)  # 替换为下划线
    return new_title

headers = {
    "cookie": "", # F12->网络->搜索follow->Ctrl+R刷新->找到"followings?order=..."->见headers
    "user-agent": "" # 获取方式同上
}

p_id = int(input("请输入用户ID: "))
page_max = int(input("请输入关注的页数: "))

info_title = str(p_id)
output_doc = ''
col_count = 0

# https://api.bilibili.com/x/relation/followings?order=desc&order_type=&vmid={{pid}}&pn=1&ps=24&gaia_source=main_web&web_location=333.1387

for page_index in range(page_max):
    url = "https://api.bilibili.com/x/relation/followings?order=desc&order_type=&vmid=" + str(p_id) + "&pn=" + str(page_index + 1) + "&ps=24&gaia_source=main_web&web_location=333.1387"
    data_text = requests.get(url=url, headers=headers).text
    data = json.loads(data_text)

    uploaders = data["data"]["list"]

    for uploader in uploaders:

        local_up_data = f'''
![|L|75]({uploader["face"]})
[{uploader["uname"]}](https://space.bilibili.com/{uploader["mid"]}/)
{uploader["sign"]}
'''

        output_doc += local_up_data

    time.sleep(1)

info_title += '.md'

with open(info_title, 'w', encoding='utf-8') as f:
    f.write(output_doc)
