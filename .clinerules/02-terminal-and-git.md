вумудщз\# Terminal and Git Workflow



\## 1. General Principles



\- Follow all environment and shell compatibility requirements defined in `01-project-rules.md`.

\- The project root is `F:\\GitRepos\\Obstacle-Rush-Prototype`.

\- The Git repository is located in the project root.

\- Never assume the current Git branch, repository state, or remote configuration.

\- Verify the repository state before performing Git operations.

\- Preserve all user changes, including uncommitted and untracked files.



\## 2. Repository Verification



Before starting a task involving project files:



1\. Verify the current working directory.

2\. Verify the Git repository root using `git rev-parse --show-toplevel`.

3\. Check the current branch using `git branch --show-current`.

4\. Inspect the working tree using `git status`.

5\. Inspect relevant changes before modifying affected files.



Use `git diff` to inspect modifications to tracked files and `git diff --cached` to inspect staged changes.



Use `git ls-files` to inspect tracked files when necessary.



Use `git check-ignore -v` to determine why a file is ignored.



\## 3. Git Modification Rules



\- Never discard or overwrite uncommitted user changes.

\- Never automatically stage all files using `git add .` or `git add -A`.

\- Stage only explicitly relevant files after reviewing their changes.

\- Never create commits without explicit user approval.

\- Never push to a remote repository without explicit user approval.

\- Never merge, rebase, cherry-pick, amend commits, or change branches without explicit user approval.

\- Never execute destructive commands such as:

&#x20; - `git reset --hard`

&#x20; - `git clean -fd`

&#x20; - `git checkout -- .`

&#x20; - `git restore .`

&#x20; - `git branch -D`

&#x20; - `git push --force`

\- Do not use alternative commands to achieve the same destructive result.

\- Never delete untracked files without explicit approval.

\- Never rewrite Git history without explicit approval.



\## 4. Git Diff and Change Review



Before proposing a commit:



\- Inspect all relevant changes.

\- Review both tracked and untracked files.

\- Identify unrelated modifications.

\- Check for accidental generated files.

\- Check for secrets, credentials, API keys, tokens, and local configuration.

\- Confirm that Unity metadata and asset references have not been unintentionally damaged.

\- Explain the purpose of each proposed change.



Do not claim that a change is safe to commit without reviewing its contents.



\## 5. Branches and Remotes



\- The existing primary branch is `main`.

\- The configured remote is `origin`.

\- Never assume that `main` is the current branch.

\- Do not create, rename, delete, or switch branches without explicit approval.

\- Do not change remote URLs or repository configuration without explicit approval.

\- Do not assume that the local branch is synchronized with GitHub.

\- Verify remote tracking status before discussing synchronization.



\## 6. Unity-Specific Git Rules



\- Preserve `.meta` files.

\- Never manually regenerate or delete GUID metadata as a cleanup operation.

\- Keep `Assets/`, `Packages/`, and `ProjectSettings/` under version control.

\- Do not commit generated folders such as `Library/`, `Temp/`, `Obj/`, `Logs/`, and `Build/` unless explicitly requested.

\- Do not change `.gitignore` without explaining the reason.

\- Treat `repomix-output.xml` and other generated reports as potentially useful files until their purpose and tracking requirements have been established.

\- Do not add large binary assets to Git without checking repository size and existing Git LFS configuration.



\## 7. Task Completion Report



After Git-related work, report:



1\. Current branch.

2\. Repository status.

3\. Files modified.

4\. Files added or removed.

5\. Whether changes are staged.

6\. Whether any commits were created.

7\. Whether anything was pushed to a remote.

8\. Any remaining concerns.



Never imply that a commit or push occurred when it did not.

\## 8. File Operation Safety Rules
- **NO PowerShell File Generation via Terminal:** Never use PowerShell cmdlets (`Set-Content`, `Out-File`, `Add-Content`, `echo`, redirection `>`) in the terminal to create, overwrite, or modify source code (`.cs`), configuration files, or Unity `.meta` files.
- **Use Native File Tools:** Always use Cline's built-in file writing and editing tools (such as `write_to_file` or `apply_diff`) for all file modifications. 
- **Meta File Integrity:** Never generate or manipulate Unity `.meta` files manually via terminal scripts. If a `.meta` file is missing or corrupted, delete it and let the Unity Editor recreate it automatically, or restore it via `git checkout`.


\## 9. File Operation Safety Rules
- **NO PowerShell File Generation via Terminal:** Never use PowerShell cmdlets (`Set-Content`, `Out-File`, `[System.IO.File]`, redirection `>`) in the terminal to create or modify source code files.
- **Use Native File Tools:** Always use Cline's built-in file writing tools (`write_to_file` / `apply_diff`) instead of executing text-writing scripts through the shell.

\## 10. Priority



Protecting the user's work and preserving repository integrity always takes priority over convenience or speed.

