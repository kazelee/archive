'''bilibili 收藏夹导出成 html 文件（附封面）'''

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
    "cookie": "", # F12->网络->搜索list->Ctrl+R刷新->找到"list?media_id=..."->见headers
    "user-agent": "" # 获取方式同上
}

fav_id = int(input("请输入收藏夹ID: "))
page_max = int(input("请输入收藏夹的页数: "))

info_title = None
output_doc = '<table align="center">\n'
col_count = 0

# https://api.bilibili.com/x/v3/fav/resource/list?media_id=100000000&pn=1&ps=40&keyword=&order=mtime&tid=0&platform=web&web_location=0.0

for page_index in range(page_max):
    url = "https://api.bilibili.com/x/v3/fav/resource/list?media_id=" + str(fav_id) + "&pn=" + str(page_index + 1) + "&ps=40&keyword=&order=mtime&type=0&tid=0&platform=web&jsonp=jsonp"
    data_text = requests.get(url=url, headers=headers).text
    data = json.loads(data_text)
    # 列表一行开始
    if col_count == 0:
        output_doc += '\t<tr>\n'
  
    # 录入一页信息
    info_title = data["data"]["info"]["title"]

    # 将不能作为标题的字符替换成下划线
    info_title = validateTitle(info_title)

    medias = data["data"]["medias"]

    for media in medias:

        # 一行放置5个视频
        if col_count < 5:
            output_doc += '\t\t<td>\n'
            col_count += 1
        else:
            output_doc += '\t</tr>\n\t<tr>\n\t\t<td>\n'
            col_count = 1

        # 输入图片和标题链接
        output_doc = output_doc + '\t\t\t<img src="' + media["cover"] + '" height=108 width=192/><br/>\n'
        output_doc = output_doc + '\t\t\t<a href="https://www.bilibili.com/video/' + media["bv_id"] + '/">' + media["title"] + '</a>\n'
    
        output_doc += '\t\t</td>\n'

    time.sleep(1)

output_doc += '\t</tr>\n</table>'
info_title += '.html'

# 在运行的根目录下生成一个名称与收藏夹名称相同的html文件
with open(info_title, 'w', encoding='utf-8') as f:
    f.write(output_doc)
