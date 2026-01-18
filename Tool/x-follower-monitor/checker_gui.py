import json
import os
import glob
import tkinter as tk
from tkinter import messagebox, scrolledtext
import google.generativeai as genai

# フォルダ構成の設定
NG_FILE = 'config/ng_words.txt'
ALLOW_FILE = 'config/allowlist.txt'
API_KEY_FILE = 'config/api_key.txt'
INPUT_DIR = 'input'

class FollowerCheckerGUI:
    def __init__(self, root):
        self.root = root
        self.root.title("X Follower Monitor + Gemini AI")
        self.root.geometry("800x650")

        self.label = tk.Label(root, text="AI連携モード：キーワード検知後にGeminiが詳細分析します", pady=10)
        self.label.pack()

        self.btn = tk.Button(root, text="分析開始 (AIレポート付)", command=self.run_analysis, 
                             bg="#1d9bf0", fg="white", font=("Arial", 10, "bold"), width=30)
        self.btn.pack(pady=5)

        self.output_area = scrolledtext.ScrolledText(root, width=95, height=35, font=("Consolas", 9))
        self.output_area.pack(padx=10, pady=10)

        # Geminiの設定
        self.init_ai()

    def init_ai(self):
        if os.path.exists(API_KEY_FILE):
            with open(API_KEY_FILE, 'r', encoding='utf-8') as f:
                api_key = f.read().strip()
                genai.configure(api_key=api_key)
                self.model = genai.GenerativeModel('gemini-1.5-flash')
                self.ai_enabled = True
        else:
            self.ai_enabled = False

    def get_ai_report(self, username, bio):
        if not self.ai_enabled: return "APIキー未設定のためAI分析スキップ"
        
        prompt = f"""
        以下のX(Twitter)ユーザーの自己紹介文(Bio)を分析し、
        「投資勧誘、副業スパム、エロ垢、数稼ぎ用ボット」のいずれかに該当するか判定してください。
        
        ユーザー名: {username}
        Bio: {bio}
        
        【出力形式】
        判定：[安全 / 疑わしい / スパム確定]
        理由：(100文字以内で簡潔に)
        """
        try:
            response = self.model.generate_content(prompt)
            return response.text
        except Exception as e:
            return f"AI分析エラー: {str(e)}"

    def run_analysis(self):
        # 1. リスト読み込み
        if not os.path.exists(NG_FILE):
            messagebox.showerror("エラー", "ng_words.txtが見つかりません")
            return
        
        with open(NG_FILE, 'r', encoding='utf-8') as f:
            ng_list = [line.strip() for line in f if line.strip() and not line.startswith('#')]
        
        allow_list = []
        if os.path.exists(ALLOW_FILE):
            with open(ALLOW_FILE, 'r', encoding='utf-8') as f:
                allow_list = [line.strip() for line in f if line.strip() and not line.startswith('#')]

        # 2. 最新JSON取得
        json_files = glob.glob(os.path.join(INPUT_DIR, '*.json'))
        if not json_files:
            messagebox.showwarning("警告", "JSONがありません")
            return
        latest_json = max(json_files, key=os.path.getctime)
        
        with open(latest_json, 'r', encoding='utf-8') as f:
            followers = json.load(f)

        self.output_area.delete('1.0', tk.END)
        self.output_area.insert(tk.END, f"分析対象: {os.path.basename(latest_json)}\n" + "="*70 + "\n")

        suspects = 0
        for user in followers:
            if user['accountId'] in allow_list: continue

            # クリーニング処理
            raw_bio = user.get('bio', '')
            noise = ["フォローされています", "フォローバック", "フォロー中"]
            clean_bio = raw_bio
            for p in noise: clean_bio = clean_bio.replace(p, "")
            clean_bio = clean_bio.strip()

            found = [word for word in ng_list if word in clean_bio]
            
            if found:
                suspects += 1
                self.output_area.insert(tk.END, f"【検知】 {user['accountId']} ({user['username']})\n")
                self.output_area.insert(tk.END, f" 理由(KW): {', '.join(found)}\n")
                
                # AIレポートの取得
                self.output_area.insert(tk.END, " AI分析中...\n")
                self.root.update() # 画面を更新して「分析中」を出す
                
                report = self.get_ai_report(user['username'], clean_bio)
                self.output_area.insert(tk.END, f"{report}\n")
                self.output_area.insert(tk.END, "-"*70 + "\n")

        self.output_area.insert(tk.END, f"\n完了: {len(followers)}件中、{suspects}件を検知。")

if __name__ == "__main__":
    root = tk.Tk()
    app = FollowerCheckerGUI(root)
    root.mainloop()