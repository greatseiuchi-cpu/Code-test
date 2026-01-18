(function() {
  const userCells = document.querySelectorAll('[data-testid="UserCell"]');
  const results = [];

  userCells.forEach(cell => {
    const text = cell.innerText.split('\n');
    if (text.length >= 2) {
      const username = text[0];
      const accountId = text.find(t => t.startsWith('@'));
      if (accountId) {
        // IDより後のテキストをBioとして結合
        const idIndex = text.indexOf(accountId);
        const bio = text.slice(idIndex + 1).join(" ");
        results.push({ username, accountId, bio });
      }
    }
  });

  if (results.length > 0) {
    const blob = new Blob([JSON.stringify(results, null, 2)], {type: "application/json"});
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `x_followers_data.json`;
    a.click();
    alert(results.length + "名分を保存しました");
  } else {
    alert("読み込みが終わってから押してください");
  }
})();