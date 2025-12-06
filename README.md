# 🚀 Git Workflow Guide for SalesLedger Team

---

## 📋 Table of Contents
- [⚡ Daily Workflow Quick Reference](#-daily-workflow-quick-reference)
- [🎬 Starting a New Feature](#-starting-a-new-feature)
- [💻 Working on Your Feature](#-working-on-your-feature)
- [📤 Creating a Pull Request](#-creating-a-pull-request)
- [✅ After Your PR is Merged](#-after-your-pr-is-merged)
- [🔄 Switching Between Branches](#-switching-between-branches)
- [🎯 Common Scenarios](#-common-scenarios)
- [🔧 Troubleshooting](#-troubleshooting)
- [📚 Best Practices Summary](#-best-practices-summary)
- [⚡ Quick Command Reference](#-quick-command-reference)

---

## ⚡ Daily Workflow Quick Reference
<details> <summary> Click to Expand </summary>
   
### 🟢 Start Your Day
```bash
git checkout main                           # Switch to main branch
git pull origin main                        # Grab the current main branch
git checkout -b feature/my-new-feature      # Create a new branch for your feature
```

### 🔨 During Work
```bash
git add .                                   # Add your updates
git commit -m "Clear description"           # Commit with a message
git push -u origin feature/my-new-feature   # Push to GitHub repo
```

### 🎉 End of Feature
```bash
git checkout developer                      # Switch to developer
git pull origin developer                   # Grab current developer branch
git merge feature/my-new-feature            # Merge your feature in
git push origin developer                   # Update the branch
```

### 🧹 After Testing Passes
```bash
git branch -d feature/my-new-feature                # Delete locally
git push origin --delete feature/my-new-feature     # Delete from repo
```
</details>

## 🎬 Starting a New Feature
<details> <summary> Click to Expand </summary>
   
### Step 1️⃣ Check Your Current Branch
```bash
git branch
```
> 💡 **Tip:** Look for the `*` to see where you are

### Step 2️⃣ Make Sure You're on Main
```bash
git checkout main
```

### Step 3️⃣ Get the Latest Code
```bash
git pull origin main
```
> ⚠️ **Important:** Always pull before creating a new branch!

### Step 4️⃣ Create Your Feature Branch
```bash
git checkout -b feature/your-feature-name
```
> 📝 **Example:** `git checkout -b feature/email-validation`

### Step 5️⃣ Verify You're on the New Branch
```bash
git branch
```
> ✅ You should see `*` next to your new feature branch
</details
   
---

## 💻 Working on Your Feature
<details> <summary> Click to Expand </summary>
   
### Step 1️⃣ Make Your Changes
🖊️ Open Visual Studio Code or your editor  
📝 Edit the files you need to change  
💾 Save your work

### Step 2️⃣ Check What Changed
```bash
git status
```
> 👀 This shows all modified files

### Step 3️⃣ Stage Your Changes
```bash
git add .                    # Add all changes
```
**Verify they're staged:**
```bash
git status                   # Check again
```
> ✅ Files should now be listed as "Changes to be committed"

### Step 4️⃣ Commit Your Changes
```bash
git commit -m "Your simple commit message describing the change"
```

#### ✅ Good Commit Messages:
- ✨ "Add email validation to customer service"
- 🐛 "Fix null reference bug in Order.cs"
- 📝 "Update customer tests for new validation"
- 🔧 "Refactor product service for better performance"

#### ❌ Bad Commit Messages:
- ❌ "updates"
- ❌ "fixed stuff"
- ❌ "asdfasdf"
- ❌ "work"

### Step 5️⃣ Push to Remote Repository
```bash
# First time pushing this branch
git push -u origin feature/email-validation

# After the first push, just use
git push
```
> 🎯 The `-u` flag sets up tracking so future pushes are easier
</details
   
---

## 📤 Creating a Pull Request
<details> <summary> Click to Expand </summary>
   
### Step 1️⃣ Go to GitHub
🌐 Navigate to your repository  
👀 Look for: **"feature/your-feature had recent pushes"**  
🖱️ Click → **"Compare & pull request"**

### Step 2️⃣ Fill Out PR Information
```
📌 Title: Add email validation to customer service

📝 Description:
   What Changed: [Describe your changes]
   Testing: [How you tested it]
   Related Issues: [Link any issues]
```

### Step 3️⃣ Set Base and Compare Branches
- **Base branch:** `developer` ⚠️ **(NOT main!)**
- **Compare branch:** `feature/your-feature`

### Step 4️⃣ Request Reviewers
👥 Select 1-2 team members  
🖱️ Click → **"Create Pull Request"**

### Step 5️⃣ Wait for Review
- ✅ **Approved** - Ready to merge!
- 💬 **Changes Requested** - Make updates
- ❌ **Rejected** - Discuss with team

### Step 6️⃣ If Changes Are Requested
```bash
# Make the requested changes in your editor
git add .
git commit -m "Address review feedback"
git push
```
> 🔄 The PR automatically updates with your new commits

### Step 7️⃣ Merge the Pull Request
✅ Once approved, click → **"Merge pull request"**  
✅ Click → **"Confirm merge"**  
🎉 **Success!** Your code is now in the developer branch

</details>

---

## ✅ After Your PR is Merged
<details> <summary> Click to Expand </summary>
   
### Step 1️⃣ Switch Back to Main
```bash
git checkout main
```

### Step 2️⃣ Pull Latest Changes
```bash
git pull origin main
```

### Step 3️⃣ Delete Your Local Feature Branch
```bash
git branch -d feature/email-validation
```
> 🗑️ Cleans up your local branches

### Step 4️⃣ Delete the Remote Branch
```bash
git push origin --delete feature/email-validation
```
> 💡 **Or:** Click "Delete branch" button on GitHub

### Step 5️⃣ Verify Deletion
```bash
git branch -a
```
> ✅ Your feature branch should NOT be listed anymore

### Step 6️⃣ Start Your Next Feature
```bash
git checkout -b feature/next-feature
```
> 🔄 Ready to start the cycle again!
</details>

---

## 🔄 Switching Between Branches
<details> <summary> Click to Expand </summary>
   
### 🔀 Switch to an Existing Branch
```bash
git checkout main                          # Switch to main
git checkout developer                     # Switch to developer
git checkout feature/email-validation      # Switch to a feature branch
git checkout -                             # Switch to previous branch
```

### 🆕 Create and Switch in One Command
```bash
git checkout -b feature/new-feature
```

### ⚠️ Before You Switch - Save Your Work!
**If you have uncommitted changes:**
```bash
git add .
git commit -m "Work in progress"
git checkout other-branch
```
> 💾 Always commit or stash before switching!
</details>

---

## 🎯 Common Scenarios
<details> <summary> Click to Expand </summary>
### 🔄 Scenario 1: Update Your Branch with Latest Code
**Problem:** Your feature branch is behind main/developer

```bash
git add .
git commit -m "Save current work"
git checkout main
git pull origin main
git checkout feature/your-feature
git merge main
# If conflicts appear, resolve them (see Troubleshooting)
git push origin feature/your-feature
```

### 🚨 Scenario 2: Need to Switch Tasks Urgently
**Problem:** Working on feature A, but need to fix urgent bug

```bash
git stash                               # 💾 Save work without committing
git checkout main
git pull origin main
git checkout -b hotfix/critical-bug
# Fix the bug
git add .
git commit -m "Fix critical bug"
git push origin hotfix/critical-bug
git checkout feature/your-feature
git stash pop                           # 📂 Restore your work
```

### 😅 Scenario 3: Made Changes on Wrong Branch
**Problem:** Edited files while on main instead of feature branch

```bash
git status                              # Shows: modified files on main
git stash                               # ⚠️ DON'T commit yet! Stash instead
git checkout -b feature/correct-branch
git stash pop                           # Restore your changes
git add .
git commit -m "Add feature (on correct branch now)"
```

### ✏️ Scenario 4: Need to Rename a Branch
**Problem:** Created `feature/thing` but should be `feature/somethingelse`

```bash
# While on the branch:
git branch -m feature/customer-validation

# OR from another branch:
git branch -m feature/thing feature/customer-validation
git push origin --delete feature/thing
git push -u origin feature/customer-validation
```

### 😱 Scenario 5: Accidentally Committed to Main
**Problem:** Made commits directly to main branch

```bash
git log --oneline                       # Check the commits you made
git checkout -b feature/should-be-here  # Create feature branch with commits
git checkout main
git reset --hard origin/main            # Reset main to before your commits
git checkout feature/should-be-here
git push -u origin feature/should-be-here
```
</details>

---

## 🔧 Troubleshooting
<details> <summary> Click to Expand </summary>
   
### ⚠️ Problem: Merge Conflicts
**When:** Git says "CONFLICT" when merging or pulling

#### Step 1️⃣ See which files have conflicts
```bash
git status
```

#### Step 2️⃣ Open the conflicted file
Look for these markers:
```
<<<<<<< HEAD
your changes
=======
their changes
>>>>>>> main
```

#### Step 3️⃣ Edit the file
Keep what you want, remove the markers

#### Step 4️⃣ Mark as resolved
```bash
git add Services/CustomerService.cs
```

#### Step 5️⃣ Complete the merge
```bash
git commit -m "Resolve merge conflict"
git push
```

---

### 📉 Problem: "Your branch is behind"
**When:** Someone else merged code to main

```bash
git checkout main
git pull origin main
git checkout feature/your-branch
git merge main
# Resolve any conflicts if they appear
git push origin feature/your-branch
```

---

### 🔙 Problem: Accidentally Deleted Important Code
**When:** You deleted something and need it back

```bash
# Find the commit that had your code
git log --oneline --all

# View the file from that commit (replace abc1234 with actual commit hash)
git show abc1234:Services/CustomerService.cs

# Restore the entire file
git checkout abc1234 -- Services/CustomerService.cs

# Or create a branch from that point
git checkout -b recover-work abc1234
```

---

### ↩️ Problem: Want to Undo Last Commit
**When:** Committed too early or made a mistake

```bash
# Undo commit but KEEP changes (still staged)
git reset --soft HEAD~1

# Undo commit and UNSTAGE changes (but keep in files)
git reset HEAD~1
```
> ⚠️ **Warning:** Don't use `--hard` unless you want to delete changes!

---

### 👀 Problem: Need to See What Changed

```bash
git diff                                # See unstaged changes
git diff --staged                       # See staged changes
git diff Services/CustomerService.cs    # See changes in specific file
git diff main..feature/your-branch      # Compare branches
```

---

### 📁 Problem: "fatal: not a git repository"
**When:** You're not in the project folder

```bash
# Navigate to your project folder
cd ~/path/to/SalesLedger

# Verify you're in the right place
git status
```

---

### 🔐 Problem: Pushed Sensitive Data
**When:** Accidentally committed API keys or passwords

> 🚨 **CRITICAL:** DO NOT just delete and recommit - it's still in history!

**Step 1:** Change the password/key immediately on the service  
**Step 2:** Ask team lead to remove from history (complex)  
**Prevention:** Add sensitive files to `.gitignore`

```bash
echo "appsettings.json" >> .gitignore
echo ".env" >> .gitignore
git add .gitignore
git commit -m "Add sensitive files to gitignore"
```
</details>

---

## 📚 Best Practices Summary
<details> <summary> Click to Expand </summary>
   
### ✅ DO:
- ✨ Always work on feature branches, never directly on main
- 📥 Pull before you start work each day
- 💬 Commit frequently with clear messages
- 📤 Push regularly (at least once per day)
- 🗑️ Delete branches after they're merged
- 🆘 Ask for help when stuck

### ❌ DO NOT:
- 🚫 Commit directly to main or developer
- ⏳ Leave branches unmerged for a long time
- 📝 Use bad commit messages like "updates" or "fixes"
- 💪 Force push (`git push -f`) unless you know what you're doing
- 🎭 Work on multiple features in one branch
- 🙈 Forget to pull before starting work

</details>

---

## ⚡ Quick Command Reference
<details> <summary> Click to Expand </summary>
   
### 🌿 Branch Management
```bash
git branch                          # 📋 List branches
git branch -a                       # 📋 List all branches (including remote)
git checkout -b feature/name        # ➕ Create and switch to new branch
git checkout branch-name            # 🔄 Switch to existing branch
git branch -d branch-name           # 🗑️ Delete local branch
git push origin --delete branch     # 🗑️ Delete remote branch
```

### 📅 Daily Workflow
```bash
git status                          # 👀 See what changed
git add .                           # ➕ Stage all changes
git add file.cs                     # ➕ Stage specific file
git commit -m "message"             # 💾 Commit changes
git push                            # 📤 Push to remote
git pull                            # 📥 Pull from remote
```

### 💼 Saving Work Temporarily
```bash
git stash                           # 💾 Save current work
git stash pop                       # 📂 Restore saved work
git stash list                      # 📋 See all stashes
```

### 📖 Viewing History
```bash
git log                             # 📚 See commit history
git log --oneline                   # 📄 Compact history
git diff                            # 👁️ See uncommitted changes
```

### ↩️ Undoing Things
```bash
git reset HEAD~1                    # ↩️ Undo last commit (keep changes)
git checkout -- file.cs             # 🔄 Discard changes in file
```

### 🌐 Remote Operations
```bash
git pull                            # 📥 Download and merge remote changes
git push -u origin branch-name      # 📤 Push new branch first time
git fetch                           # 📥 Download remote changes (don't merge)
```
</details>

---

**Last Updated:** December 6, 2025  
**Version:** 1.0  
**Maintained by:** Nick, Gaberiel, Alex, Tyler
