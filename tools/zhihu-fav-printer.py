import requests
import json
import time

# url = "https://www.zhihu.com/api/v4/collections/00000000/items?offset=0&limit=20"

headers = {
    "cookie": "", # F12->网络->搜索items->Ctrl+R刷新->找到"items?offset=..."->见headers
    "user-agent": "" # 获取方式同上
}

fav_id = int(input("请输入收藏夹ID: "))
page_max = int(input("请输入收藏夹的页数: "))
fav_name = input("请输入收藏夹名称：")
fav_info = input("请输入收藏夹简介：")

# info_title = str(fav_id)
info_title = fav_name

output_doc = f'<p align="center"><b>{fav_name}</b></p>\n<p align="center">{fav_info}</p>\n<table align="center">\n'
col_count = 0

for page_index in range(page_max):
    url = "https://www.zhihu.com/api/v4/collections/" + str(fav_id) + "/items?offset=" + str(page_index * 20) + "&limit=20"
    data_text = requests.get(url=url, headers=headers).text
    data = json.loads(data_text)
    
    # 录入一页信息
    pages = data["data"]

    for page in pages:

        content = page["content"]
        page_type = content["type"]
        page_url = ""
        page_title = ""

        author_name = ""
        author_url = ""
        author_face = ""

        if page_type == "answer":
            page_title = content["question"]["title"]
            page_url = content["url"]
        elif page_type == "article":
            page_title = content["title"]
            page_url = content["url"]
        else:
            page_title = content["excerpt_title"]
            page_url = content["url"]

        author_name = content["author"]["name"]
        author_url = content["url"]
        author_face = content["author"]["avatar_url"]

        output_doc = output_doc + '\t<tr>\n\t\t<td>\n\t\t\t<img src="' + author_face + '" height=25 width=25 />\n'
        output_doc = output_doc + '\t\t\t<a href="' + author_url + '">' + author_name + '</a>\n\t\t</td>\n\t\t<td>\n'
        output_doc = output_doc + '\t\t\t<a href="' + page_url + '">' + page_title + '</a><br/>\n\t\t</td>\n\t</tr>\n'

    time.sleep(1)

output_doc += '</table>'
info_title += '.html'

with open(info_title, 'w', encoding='utf-8') as f:
    f.write(output_doc)