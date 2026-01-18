import os
import re

# --- Settings ---
OLD_FILE = 'input/followers_0117.txt'
NEW_FILE = 'input/followers_0117-2.txt'
BLOCKLIST_FILE = 'config/blocklist.txt'
NG_WORDS_FILE = 'config/ng_words.txt'
REPORT_FILE = 'output/follower_report.txt'
FOLLOWED_BY_MARKER = 'フォローされています' # This marker is in the source data, so it stays in Japanese
# --- Settings End ---

def parse_follower_file(filepath):
    """
    Parses a follower list file and returns a dictionary keyed by account ID.
    File format is expected to be:
    UserName
    @AccountID
    フォローされています
    Bio...
    """
    followers = {}
    if not os.path.exists(filepath):
        return followers

    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()

    # Split user blocks by one or more empty lines
    user_blocks = re.split(r'\n\s*\n', content)

    for block in user_blocks:
        lines = [line.strip() for line in block.split('\n') if line.strip()]
        if not lines or len(lines) < 2 or not lines[1].startswith('@'):
            continue

        username = lines[0]
        account_id = lines[1]
        
        bio = ""
        try:
            marker_index = lines.index(FOLLOWED_BY_MARKER)
            bio_lines = lines[marker_index + 1:]
            bio = "\n".join(bio_lines).strip()
        except ValueError:
            if len(lines) > 2:
                bio = "\n".join(lines[2:]).strip()

        followers[account_id] = {'username': username, 'bio': bio}
        
    return followers

def main():
    """Main comparison logic"""
    old_followers = parse_follower_file(OLD_FILE)
    new_followers = parse_follower_file(NEW_FILE)

    # --- Load auxiliary lists ---
    blocklist = set()
    if os.path.exists(BLOCKLIST_FILE):
        with open(BLOCKLIST_FILE, 'r', encoding='utf-8') as f:
            blocklist = {line.strip() for line in f if line.strip()}
            
    ng_words = set()
    if os.path.exists(NG_WORDS_FILE):
        with open(NG_WORDS_FILE, 'r', encoding='utf-8') as f:
            ng_words = {line.strip() for line in f if line.strip() and not line.startswith('#')}

    # --- Calculate differences ---
    old_ids = set(old_followers.keys())
    new_ids = set(new_followers.keys())

    added_ids = new_ids - old_ids
    removed_ids = old_ids - new_ids
    kept_ids = new_ids & old_ids

    # --- Analyze changes ---
    profile_changers = []
    for user_id in kept_ids:
        if old_followers[user_id]['bio'] != new_followers[user_id]['bio']:
            profile_changers.append(user_id)

    # --- Build Report ---
    report_lines = ["--- Follower Comparison Results ---"]

    report_lines.append(f"\n[1. New Followers] ({len(added_ids)})")
    if added_ids:
        for user_id in sorted(list(added_ids)):
            info = new_followers[user_id]
            markers = []
            if not info['bio']:
                markers.append("[BLANK PROFILE]")
            else:
                for word in ng_words:
                    if word in info['bio'] or word in info['username']:
                        markers.append(f"[NG WORD: {word}]")
                        break
            
            marker_str = " ".join(markers)
            report_lines.append(f"- {info['username']} ({user_id}) {marker_str}")
            if info['bio']:
                report_lines.append(f"  Bio: {info['bio']}\n")
            else:
                report_lines.append("  Bio: (Not set)\n")
    else:
        report_lines.append("None")

    report_lines.append(f"\n[2. Unfollowers] ({len(removed_ids)})")
    if removed_ids:
        for user_id in sorted(list(removed_ids)):
            info = old_followers[user_id]
            block_marker = "[ON BLOCKLIST]" if user_id in blocklist else ""
            report_lines.append(f"- {info['username']} ({user_id}) {block_marker}")
    else:
        report_lines.append("None")

    report_lines.append(f"\n[3. Profile Changers] ({len(profile_changers)})")
    if profile_changers:
        for user_id in sorted(list(profile_changers)):
            old_bio = old_followers[user_id]['bio']
            new_bio = new_followers[user_id]['bio']
            block_marker = "[ON BLOCKLIST]" if user_id in blocklist else ""
            report_lines.append(f"- {new_followers[user_id]['username']} ({user_id}) {block_marker}")
            report_lines.append(f"  Old Bio: {old_bio}")
            report_lines.append(f"  New Bio: {new_bio}\n")
    else:
        report_lines.append("None")
        
    report_lines.append("\n--- End of Report ---")

    # --- Write report to file ---
    with open(REPORT_FILE, 'w', encoding='utf-8') as f:
        f.write("\n".join(report_lines))
        
    print(f"Comparison complete. Report saved to '{REPORT_FILE}'")


if __name__ == "__main__":
    main()
