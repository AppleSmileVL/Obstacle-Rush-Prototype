# Project and Environment Rules

## 1. Environment & Tooling

- **Project Root:** `F:\GitRepos\Obstacle-Rush-Prototype`
- **Engine:** Unity `2022.3.62f2` (Built-in Render Pipeline, Legacy Input Manager).
- **Shell Environment:** Windows 11 with **MINGW64 (Git Bash)** as the default integrated terminal. Never run raw PowerShell cmdlets (`Get-Content`, `Set-Content`, `Out-File`, `Copy-Item`, etc.) directly in Bash.
- **Native File Tools Only:** Always read, create, and edit project files using Cline's built-in file tools (`read_file`, `write_to_file`, `replace_in_file` / `apply_diff`). Never use terminal commands or temporary `/tmp` files to write, copy, or modify project files.
- **Terminal Usage:** Only execute terminal commands when explicitly requested by the user or for lightweight read-only Git checks (`git status -s`, `git diff -- <path>`).

## 2. Project Layout & Safety

- **Scripts Location:** All C# gameplay and system scripts live under `Assets/CodeBase/` (not `Assets/Scripts/`).
- **Preservation:** Never delete or overwrite existing user changes, untracked reports (`PROJECT-AUDIT.md`, `repomix-output.xml`), or scene/prefab references unless explicitly instructed.
- **Metadata & Generated Folders:** Never manually edit, generate, or delete `.meta` files or touch Unity-generated folders (`Library/`, `Temp/`, `Obj/`, `Logs/`, `Build/`).
- **Scope:** Make minimal, focused edits only to files relevant to the current task. Do not refactor unrelated systems.

## 3. Honesty & Verification

- Never claim that Unity compilation or Play Mode tests succeeded unless explicitly verified.
- Clearly distinguish between static code review and behavior that still needs manual testing in the Unity Editor.