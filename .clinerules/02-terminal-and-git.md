# Terminal and Git Workflow

## 1. General Principles

- Follow all environment and shell compatibility requirements defined in `01-project-rules.md`.
- The project root is `F:\GitRepos\Obstacle-Rush-Prototype`.
- The Git repository is located in the project root.
- The default terminal environment is **MINGW64 (Git Bash)** on Windows. Never run raw PowerShell cmdlets directly in Bash.
- Preserve all user changes, including uncommitted and untracked files.
- Never assume the current Git branch, repository state, or remote configuration.

## 2. File Operation Safety Rules (CRITICAL)

- **Native File Tools ONLY:** Always use Cline's built-in file tools (`read_file`, `write_to_file`, `replace_in_file` / `apply_diff`) to read, create, or modify source code (`.cs`), documentation (`.md`), configuration files, and Unity assets.
- **NO Shell-Based File Manipulation:** Never use terminal commands (`execute_command`), Bash, or `powershell -Command` to create, copy, move, overwrite, or modify project files.
- **Forbidden Commands & Patterns:** Never execute `Get-Content`, `Set-Content`, `Out-File`, `Add-Content`, `Copy-Item`, `Move-Item`, `Remove-Item`, `[System.IO.File]`, `cat`, `tee`, `echo`, heredocs (`<< EOF`), or output redirection (`>`, `>>`) to read or write project files.
- **NO Temporary Staging Files:** Never write intermediate or temporary files to `/tmp`, `F:\tmp`, `%TEMP%`, or anywhere else as a workaround to copy or patch files into the repository.
- **Meta File Integrity:** Never generate, edit, or delete Unity `.meta` files manually via terminal scripts. If a `.meta` file is missing or corrupted, let the Unity Editor recreate it or restore it via explicit Git recovery approved by the user.

## 3. Repository Verification (Lightweight)

- Do **not** run a full suite of Git commands at the start of every task unless the task specifically involves Git inspection, diff review, or repository cleanup.
- When Git verification is actually needed:
  1. Check working tree status with `git status -s`.
  2. Inspect specific modifications with `git diff -- <path>` (avoid dumping the entire repository diff unless requested).
  3. Check current branch with `git branch --show-current` before any branch or commit discussion.

## 4. Git Modification Rules

- Never discard or overwrite uncommitted user changes.
- Never automatically stage all files using `git add .` or `git add -A`.
- Stage only explicitly relevant files after reviewing their changes and receiving user approval.
- Never create commits or push to a remote repository without explicit user approval.
- Never merge, rebase, cherry-pick, amend commits, or switch/create/delete branches without explicit user approval.
- Never execute destructive commands or their equivalents:
  - `git reset --hard`
  - `git clean -fd`
  - `git checkout -- .`
  - `git restore .`
  - `git branch -D`
  - `git push --force`
- Never delete untracked files or rewrite Git history without explicit user approval.

## 5. Unity-Specific Git Rules

- Preserve `.meta` files alongside their corresponding assets and folders.
- Keep `Assets/`, `Packages/`, and `ProjectSettings/` under version control.
- Do not stage or commit generated folders (`Library/`, `Temp/`, `Obj/`, `Logs/`, `Build/`) unless explicitly requested.
- Do not modify `.gitignore` without explaining the reason and getting approval.
- Treat `repomix-output.xml` and audit/design reports (such as `PROJECT-AUDIT.md`) as user-managed files; never delete or overwrite them without explicit permission.

## 6. Pre-Commit Review & Reporting

Before proposing a commit or concluding a Git-related task:

- Review modified tracked files and relevant untracked files.
- Check for accidental temporary files, secrets, tokens, or unintended `.meta` / `ProjectSettings` changes.
- Report clearly:
  1. Current branch and working tree status.
  2. Modified, added, or removed files.
  3. Whether changes are staged, committed, or pushed (never imply an action occurred if it did not).

## 7. Priority

Protecting the user's work, avoiding shell-based file corruption, and preserving repository integrity always take priority over convenience or speed.