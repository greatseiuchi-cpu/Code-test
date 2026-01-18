import json
import os
import glob
import tkinter as tk
from tkinter import messagebox, scrolledtext
import threading  # ブロック解消用
from google import genai  # 最新SDK

# フォルダ構成
NG_FILE = 'config/ng_words.txt'
ALLOW_FILE = 'config/allowlist.txt'
API_KEY_FILE = 'config/api_key.txt'
INPUT_DIR = 'input'

class FollowerCheckerGUI:
    def __init__(self, root):
        self.root = root
        self.root.title("X Follower Monitor + Gemini AI (Async)")
        self.root.geometry("800x650")

        self.label = tk.Label(root, text="最新SDK対応版：AI分析中も画面は固まりません", pady=10)
        self.label.pack()

        self.btn = tk.Button(root, text="分析開始", command=self.start_analysis_thread, 
                             bg="#1d9bf0", fg="white", font=("Arial", 10, "bold"), width=30)
        self.btn.pack(pady=5)

        self.output_area = scrolledtext.ScrolledText(root, width=95, height=35, font=("Consolas", 9))
        self.output_area.pack(padx=10, pady=10)

        self.client = None
        self.init_ai()

    def init_ai(self):
        """APIキーの読み込みとクライアント初期化"""
        try:
            if os.path.exists(API_KEY_FILE):
                with open(API_KEY_FILE, 'r', encoding='utf-8') as f:
                    api_key = f.read().strip()
                if api_key:
                    self.client = genai.Client(api_key=api_key)
            else:
                self.log("警告: config/api_key.txt が見つかりません。AI分析はスキップされます。")
        except Exception as e:
            self.log(f"初期化エラー: {e}")

    def log(self, message):
        """スレッドセーフなログ出力"""
        def _append():
            self.output_area.insert(tk.END, message + "\n")
            self.output_area.see(tk.END)
        self.root.after(0, _append)

    def start_analysis_thread(self):
        """分析を別スレッドで開始（GUIをフリーズさせない）"""
        self.btn.config(state=tk.DISABLED) # 二重押し防止
        self.output_area.delete('1.0', tk.END)
        thread = threading.Thread(target=self.run_analysis)
        thread.daemon = True
        thread.start()

    def run_analysis(self):
        try:
            # 1. 各種リストの読み込み
            ng_list = []
            if os.path.exists(NG_FILE):
                with open(NG_FILE, 'r', encoding='utf-8') as f:
                    ng_list = [l.strip() for l in f if l.strip() and not l.startswith('#')]
            
            allow_list = []
            if os.path.exists(ALLOW_FILE):
                with open(ALLOW_FILE, 'r', encoding='utf-8') as f:
                    allow_list = [l.strip() for l in f if l.strip() and not l.startswith('#')]

            # 2. 最新JSON取得
            json_files = glob.glob(os.path.join(INPUT_DIR, '*.json'))
            if not json_files:
                self.log("エラー: inputフォルダにJSONがありません。")
                self.root.after(0, lambda: self.btn.config(state=tk.NORMAL))
                return
            
            latest_json = max(json_files, key=os.path.getctime)
            with open(latest_json, 'r', encoding='utf-8') as f:
                followers = json.load(f)

            self.log(f"分析対象: {os.path.basename(latest_json)}")
            self.log("="*70)

            # 3. ループ処理
            suspects = 0
            for user in followers:
                if user['accountId'] in allow_list: continue

                # ノイズ除去
                bio = user.get('bio', '')
                for p in ["フォローされています", "フォローバック", "フォロー中"]:
                    bio = bio.replace(p, "")
                bio = bio.strip()

                found = [word for word in ng_list if word in bio]
                if found:
                    suspects += 1
                    self.log(f"【検知】 {user['accountId']} ({user['username']})")
                    self.log(f" 理由(KW): {', '.join(found)}")
                    
                    if self.client:
                        self.log(" AI分析中...")
                        try:
                            # 最新SDK形式での呼び出し
                            response = self.client.models.generate_content(
                                model="gemini-1.5-flash",
                                contents=f"以下のXユーザーを分析し、スパム(投資・エロ・勧誘)か判定して。理由も100文字以内で。\n名前:{user['username']}\nBio:{bio}"
                            )
                            self.log(f" {response.text}")
                        except Exception as ai_e:
                            self.log(f" AI分析エラー: {ai_e}")
                    self.log("-"*70)

            self.log(f"\n完了: {len(followers)}件中、{suspects}件を検知。")

        except Exception as e:
            self.log(f"予期せぬエラー: {e}")
        finally:
            self.root.after(0, lambda: self.btn.config(state=tk.NORMAL)) # ボタンを元に戻す

if __name__ == "__main__":
    root = tk.Tk()
    app = FollowerCheckerGUI(root)
    root.mainloop()