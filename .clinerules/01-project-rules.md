## Terminal, Shell and Environment Rules

### 1. Default Environment

- The development environment is Windows 11.
- The default project terminal is Windows PowerShell 5.1.
- Assume Windows PowerShell 5.1 compatibility unless another shell has been explicitly verified.
- Never assume that the integrated terminal uses CMD, PowerShell 7, Git Bash, WSL, or another shell.
- Before executing shell-specific commands, identify the active shell and its version.
- Use `$PSVersionTable.PSVersion` to identify the PowerShell version.
- Use `Get-Location` to identify the current working directory.
- Do not change the user's default terminal, shell profile, execution policy, or system environment settings.

### 2. PowerShell Compatibility

- All shell commands must be compatible with Windows PowerShell 5.1.
- Do not use PowerShell 7+ syntax unless the installed version has been explicitly verified.
- Never use CMD-specific syntax in PowerShell.
- Never use Bash or Unix shell syntax in PowerShell.
- Do not use `&&` or `||` command chaining operators.
- Do not use `dir /s /b`, `dir /s`, `ls -R`, `rm -rf`, or similar shell-specific commands.
- Do not assume that commands with identical names behave identically across shells.
- Prefer explicit PowerShell cmdlets over aliases.

Use these PowerShell commands:

| Operation | Required command |
|---|---|
| Show current directory | `Get-Location` |
| Change directory | `Set-Location` |
| List files | `Get-ChildItem` |
| Read file | `Get-Content` |
| Write file | `Set-Content` or `Out-File` when appropriate |
| Check path | `Test-Path` |
| Copy file | `Copy-Item` |
| Move file | `Move-Item` |
| Remove file | `Remove-Item` |
| Create directory | `New-Item -ItemType Directory` |
| Search text | `Select-String` |
| Find command | `Get-Command` |
| Get file information | `Get-Item` |

### 3. Command Execution

- Execute sequential commands as separate tool calls whenever possible.
- Never combine commands using `&&` or `||`.
- Avoid long compound command strings.
- Do not assume that a command succeeded just because it was submitted.
- Inspect command output and error output before proceeding.
- If a command depends on a previous command, verify that the previous command succeeded first.
- Do not execute subsequent operations if a required previous operation failed.
- Prefer simple, independently verifiable commands.
- Avoid unnecessarily complicated PowerShell one-liners.
- Use a PowerShell script block only when multiple operations genuinely need to be executed together.
- Do not use CMD command chaining such as `&`, `&&` or `||` as a substitute for PowerShell syntax.

### 4. Working Directory and Paths

- The Unity project root is:
  `F:\GitRepos\Obstacle-Rush-Prototype`
- Always verify the current working directory before performing project-wide operations.
- Do not assume that the terminal starts in the project root.
- Use `Set-Location` only when the current directory is incorrect.
- Avoid unnecessary directory changes.
- Prefer absolute paths when the working directory is ambiguous.
- Use quotation marks around paths, especially when they contain spaces.
- Prefer `-LiteralPath` when working with paths that may contain special PowerShell wildcard characters.
- Before accessing a file or directory, verify its existence using `Test-Path`.
- Never infer that a file or directory does not exist solely because one command failed.
- Do not invent directory names or assume a conventional Unity folder structure.
- Respect the actual project structure discovered through filesystem inspection.

### 5. File System Operations

- Inspect existing files before modifying them.
- Never overwrite an existing file without understanding its current contents.
- Preserve existing file structure, formatting, line endings, and encoding whenever possible.
- Do not use commands that may silently truncate or overwrite files.
- Avoid recursive modifications unless explicitly required.
- Before creating files, verify that the destination does not already contain an existing file with the same name.
- Before moving or renaming files, inspect references and dependencies.
- Before deleting files, verify their purpose and obtain explicit user approval.
- Do not delete files simply because they appear unused.
- Do not modify Unity-generated folders such as Library, Temp, Obj and Logs manually.

### 6. Error Handling and Recovery

- If a command fails, stop and inspect the exact error message.
- Identify whether the failure is caused by shell syntax, permissions, an invalid path, a missing dependency, or an actual application error.
- Do not repeat the same failed command without changing the underlying cause.
- Do not interpret a shell parsing error as evidence that a file or directory is missing.
- Verify paths independently using `Test-Path`.
- If a command returns unexpected output, perform a simpler diagnostic command.
- Never claim that an operation succeeded unless its result has been verified.
- Include the exact error message in the final report when an operation cannot be completed.
- If the shell environment is incompatible with the required operation, explain the limitation instead of improvising potentially unsafe commands.

### 7. External Executables

- When executing external programs such as Git, Unity, dotnet or other CLI tools, use syntax compatible with PowerShell 5.1.
- Quote executable paths and arguments containing spaces.
- Do not assume that external executable arguments follow PowerShell cmdlet conventions.
- Check the exit code using `$LASTEXITCODE` after executing native programs when appropriate.
- Distinguish PowerShell errors from errors reported by external programs.
- Do not suppress error output merely to make a command appear successful.
- Do not use `Invoke-Expression` or dynamically constructed executable commands when a direct invocation is possible.

### 8. Git Safety

- Before Git operations, verify the repository root and run `git status`.
- Use the existing Git repository and its configured remotes.
- Never execute `git reset --hard`, `git clean -fd`, force push, branch deletion, or other destructive Git operations without explicit user approval.
- Never commit, push, merge, rebase, amend or change branches without explicit user approval.
- Never overwrite uncommitted user changes.
- Inspect diffs before suggesting that changes are ready to commit.
- Do not assume that untracked files are disposable.
- Preserve the existing `.gitignore` unless a specific change is justified.
- Never add generated Unity folders such as Library, Temp, Obj or Logs to Git.

### 9. Unity Project Safety

- Unity project root:
  `F:\GitRepos\Obstacle-Rush-Prototype`
- Unity version: 2022.3.62f2.
- Respect the existing architecture and folder layout.
- Scripts are located in `Assets/CodeBase`, not an assumed `Assets/Scripts` directory.
- Do not create duplicate managers, systems, folders or scripts without inspecting the existing implementation.
- Do not modify `.meta` files manually unless there is a specific technical reason.
- Preserve GUID references and Unity asset relationships.
- Do not manually modify scenes, prefabs or project settings without understanding their serialized structure.
- Do not modify ProjectSettings or Packages configuration unless required by the task.
- Never delete or regenerate Unity project metadata as a troubleshooting shortcut.
- After code changes, use appropriate validation and report whether Unity compilation was actually verified.
- Never claim that the project compiles successfully without evidence.

### 10. Final Verification

After every operation that changes files or project configuration:

- Verify that the expected files exist.
- Inspect the resulting contents or diff.
- Check for unintended changes.
- Check Git status.
- Report exactly what changed.
- Report what could not be verified.

For diagnostic-only tasks:

- Do not modify files.
- Do not create temporary files.
- Do not change Git state.
- Clearly distinguish inspected information from assumptions.

### 11. General Rule

Correctness and preservation of the existing project take priority over speed.

When uncertain about shell compatibility, file paths, project structure, or command behavior, perform a small diagnostic check before proceeding.

Never improvise a potentially destructive operation to work around a failed command.