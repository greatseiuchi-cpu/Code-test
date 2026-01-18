document.getElementById('scanBtn').onclick = async () => {
  const [tab] = await chrome.tabs.query({active: true, currentWindow: true});
  chrome.scripting.executeScript({
    target: { tabId: tab.id },
    files: ['content.js']
  });
};