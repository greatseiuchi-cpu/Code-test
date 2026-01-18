import json
import os
import glob

# パスの設定
NG_FILE = 'config/ng_words.txt'
ALLOW_FILE = 'config/allowlist.txt'
INPUT_DIR = 'input'

def analyze():
    # 1. NGワードリストの読み込み
    if os.path.exists(NG_FILE):
        with open(NG_FILE, 'r', encoding='utf-8') as f:
            ng_list = [line.strip() for line in f if line.strip() and not line.startswith('#')]
    else:
        print(f"エラー: {NG_FILE} が見つかりません。")
        return

    # 2. ホワイトリスト（除外リスト）の読み込み
    allow_list = []
    if os.path.exists(ALLOW_FILE):
        with open(ALLOW_FILE, 'r', encoding='utf-8') as f:
            allow_list = [line.strip() for line in f if line.strip() and not line.startswith('#')]

    # 3. 最新のJSONを検索
    json_files = glob.glob(os.path.join(INPUT_DIR, '*.json'))
    if not json_files:
        print(f"エラー: {INPUT_DIR} 内にJSONファイルが見つかりません。")
        return
    latest_json = max(json_files, key=os.path.getctime)
    print(f"--- 分析実行中: {latest_json} ---")

    with open(latest_json, 'r', encoding='utf-8') as f:
        followers = json.load(f)

    suspects = 0
    for user in followers:
        # ホワイトリストに入っているIDは完全に無視する
        if user['accountId'] in allow_list:
            continue

        bio = user.get('bio', '')
        found = [word for word in ng_list if word in bio]
        
        if found:
            suspects += 1
            print(f"【NG検出】 {user['accountId']} ({user['username']})")
            print(f"  キーワード: {', '.join(found)}")
            print(f"  Bio: {bio[:60]}...")
            print("-" * 40)

    print(f"\n完了: {len(followers)}件中、{suspects}件を検知（ホワイトリスト適用済）")

if __name__ == "__main__":
    analyze()