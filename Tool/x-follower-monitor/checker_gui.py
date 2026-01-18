import json
import os
import glob
import tkinter as tk
from tkinter import messagebox, scrolledtext

# フォルダ構成の設定
NG_FILE = 'config/ng_words.txt'
ALLOW_FILE = 'config/allowlist.txt'
INPUT_DIR = 'input'

class FollowerCheckerGUI:
    def __init__(self, root):
        self.root = root
        self.root.title("X Follower Monitor GUI")
        self.root.geometry("700x550")

        # 説明ラベル
        self.label = tk.Label(root, text="inputフォルダ内の最新JSONファイルを分析します", pady=10)
        self.label.pack()

        # 分析ボタン
        self.btn = tk.Button(root, text="分析開始", command=self.run_analysis, 
                             bg="#1d9bf0", fg="white", font=("Arial", 10, "bold"), width=25)
        self.btn.pack(pady=5)

        # 結果表示エリア
        self.output_area = scrolledtext.ScrolledText(root, width=85, height=30, font=("Consolas", 9))
        self.output_area.pack(padx=10, pady=10)

    def run_analysis(self):
        # 1. NGワード/ホワイトリスト読み込み
        if not os.path.exists(NG_FILE):
            messagebox.showerror("エラー", f"設定ファイルが見つかりません:\n{NG_FILE}")
            return
        
        with open(NG_FILE, 'r', encoding='utf-8') as f:
            ng_list = [line.strip() for line in f if line.strip() and not line.startswith('#')]

        allow_list = []
        if os.path.exists(ALLOW_FILE):
            with open(ALLOW_FILE, 'r', encoding='utf-8') as f:
                allow_list = [line.strip() for line in f if line.strip() and not line.startswith('#')]

        # 2. 最新JSONの取得
        json_files = glob.glob(os.path.join(INPUT_DIR, '*.json'))
        if not json_files:
            messagebox.showwarning("警告", f"{INPUT_DIR} フォルダ内にJSONファイルがありません。")
            return
        
        latest_json = max(json_files, key=os.path.getctime)
        
        # 3. 分析処理
        with open(latest_json, 'r', encoding='utf-8') as f:
            followers = json.load(f)

        self.output_area.delete('1.0', tk.END)
        self.output_area.insert(tk.END, f"分析対象: {os.path.basename(latest_json)}\n")
        self.output_area.insert(tk.END, "="*60 + "\n")

        suspects = 0
        for user in followers:
            if user['accountId'] in allow_list:
                continue

            # --- ここからクリーニング処理 ---
            raw_bio = user.get('bio', '')
            
            # 不要な固定文言を削除
            noise_phrases = ["フォローされています", "フォローバック", "フォロー中"]
            clean_bio = raw_bio
            for phrase in noise_phrases:
                clean_bio = clean_bio.replace(phrase, "")
            
            # 前後の空白や改行を整える
            clean_bio = clean_bio.strip()
            # --- ここまでクリーニング処理 ---

            # 判定はクリーニング後のBioで行う
            found = [word for word in ng_list if word in clean_bio]
            
            if found:
                suspects += 1
                self.output_area.insert(tk.END, f"【NG】 {user['accountId']} ({user['username']})\n")
                self.output_area.insert(tk.END, f" 理由: {', '.join(found)}\n")
                # 表示もスッキリしたBioを表示
                self.output_area.insert(tk.END, f" Bio : {clean_bio[:100]}\n")
                self.output_area.insert(tk.END, "-"*60 + "\n")

        self.output_area.insert(tk.END, f"\n完了: {len(followers)}件中、{suspects}件を検知しました。")

if __name__ == "__main__":
    root = tk.Tk()
    app = FollowerCheckerGUI(root)
    root.mainloop()