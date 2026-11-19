# Code Quality Standards

## 1. General Principles

- Write clear, readable, predictable C# code that matches the existing style of the project.
- Prefer simple, direct solutions over speculative abstractions, extra interfaces, or unnecessary design patterns.
- Keep changes small and focused—never rewrite an entire file when a targeted edit is sufficient.
- Do not remove existing functionality unless explicitly requested.

## 2. C# & Unity Coding Conventions

- Use explicit access modifiers (`private`, `public`, `protected`, `internal`).
- Match the naming style of the surrounding file (`PascalCase` for types/public members, `camelCase` or `_camelCase` consistent with the existing class for private fields).
- Use `[SerializeField] private` instead of `public` fields when exposing variables to the Unity Inspector unless external code requires direct access.
- Cache component and dependency references rather than calling `GetComponent` or scene searches repeatedly.
- Guard against null references on optional Unity objects and events (`?.Invoke(...)`), while avoiding redundant checks that obscure logic.
- Write comments only when explaining non-obvious constraints, Unity lifecycle edge cases, or mathematical/physics reasoning.

## 3. Code Modification Workflow

- **Before editing:** Read the complete target file(s) and inspect immediate dependencies or callers.
- **During editing:** Apply minimal, surgical edits via native file tools.
- **After editing:** Check the modified code for syntax issues, broken references, missing unsubscriptions, or unintended changes, and provide a concise summary of what changed and any remaining manual checks.