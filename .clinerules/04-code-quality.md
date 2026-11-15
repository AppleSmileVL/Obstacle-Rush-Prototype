\# Code Quality and Engineering Standards



\## 1. General Principles



\- Write clear, maintainable, predictable, and readable code.

\- Prefer simple solutions over unnecessary abstractions.

\- Follow established conventions already present in the project.

\- Do not introduce complexity without a concrete benefit.

\- Avoid speculative architecture and premature optimization.

\- Do not refactor unrelated code while implementing a requested feature.

\- Keep responsibilities clearly separated.

\- Make changes as small and focused as reasonably possible.



\## 2. C# Coding Standards



\- Use meaningful names for classes, methods, properties, fields, and variables.

\- Follow the naming conventions already used in the project.

\- Use PascalCase for types and public members.

\- Use camelCase for local variables and method parameters.

\- Use `\_camelCase` for private fields only if this matches the existing project convention.

\- Prefer explicit access modifiers.

\- Use appropriate access levels instead of exposing everything publicly.

\- Prefer properties over public fields when encapsulation is needed.

\- Avoid excessively long methods and classes with unrelated responsibilities.

\- Avoid deeply nested conditional structures when a clearer alternative exists.

\- Use `const` and `readonly` where appropriate.

\- Avoid unnecessary comments that simply repeat what the code does.

\- Add comments when they explain important reasoning, constraints, or non-obvious behavior.



\## 3. Architecture and Design



\- Follow existing project architecture before introducing new patterns.

\- Apply SOLID principles where they improve maintainability.

\- Prefer composition over inheritance when appropriate.

\- Avoid unnecessary inheritance hierarchies.

\- Keep classes focused on a clear responsibility.

\- Avoid tightly coupling unrelated systems.

\- Do not introduce interfaces for every class without a practical need.

\- Avoid unnecessary singleton patterns and global state.

\- Avoid creating managers that become containers for unrelated functionality.

\- Keep business logic separate from presentation and input handling when appropriate.

\- Avoid circular dependencies between systems.

\- Do not introduce dependency injection frameworks unless explicitly justified and approved.



\## 4. Error Handling



\- Handle expected failure cases explicitly.

\- Do not silently swallow exceptions.

\- Do not use empty `catch` blocks.

\- Catch specific exceptions whenever practical.

\- Do not use exceptions for ordinary control flow.

\- Provide useful diagnostic information when errors occur.

\- Avoid excessive logging in frequently executed methods.

\- Do not use logging as a substitute for proper error handling.

\- Validate external input and data at appropriate boundaries.

\- Avoid defensive checks that duplicate guaranteed invariants without adding value.



\## 5. Performance



\- Avoid unnecessary allocations in frequently executed methods.

\- Be mindful of garbage collection in Unity gameplay loops.

\- Avoid unnecessary LINQ operations in performance-critical code.

\- Avoid repeated component lookups when caching is appropriate.

\- Avoid unnecessary calls to `FindObjectOfType`, `FindAnyObjectByType`, or broad scene searches.

\- Avoid expensive operations in `Update` and `FixedUpdate`.

\- Use object pooling only when object creation and destruction are demonstrably expensive or frequent.

\- Do not optimize code based solely on assumptions.

\- Prefer profiling and measurements when investigating performance problems.

\- Avoid introducing complex caching systems without a demonstrated need.



\## 6. Async and Concurrency



\- Use asynchronous operations only when they provide a clear benefit.

\- Understand Unity main-thread restrictions.

\- Do not access Unity objects from background threads unless the API explicitly supports it.

\- Avoid `async void` except for appropriate event handlers.

\- Handle cancellation and object lifecycle when asynchronous work may outlive its owner.

\- Avoid unnecessary tasks, threads, and synchronization mechanisms.

\- Do not introduce concurrency to solve problems that can be handled safely with simpler sequential code.



\## 7. Data and State Management



\- Keep ownership of mutable state clear.

\- Avoid exposing mutable collections unnecessarily.

\- Avoid hidden side effects in property getters and methods.

\- Validate state transitions where appropriate.

\- Avoid duplicating the same source of truth across multiple systems.

\- Keep configuration separate from runtime state when appropriate.

\- Avoid hardcoded values when they belong in existing configuration assets.

\- Preserve compatibility with existing serialized data.



\## 8. Testing and Verification



\- Inspect existing tests and testing conventions before adding new ones.

\- Write tests for important business logic and non-trivial behavior where practical.

\- Prefer deterministic tests.

\- Avoid tests that depend unnecessarily on execution order or external state.

\- Test boundary conditions and failure cases.

\- Do not claim that tests passed unless they were actually executed.

\- Distinguish between code review, static analysis, compilation, automated testing, and runtime verification.

\- When Unity Editor or test infrastructure is unavailable, explicitly state what could not be verified.



\## 9. Code Changes and Refactoring



Before modifying code:



1\. Read the complete relevant implementation.

2\. Inspect related classes and call sites.

3\. Understand the current behavior.

4\. Identify possible side effects.

5\. Define the smallest appropriate change.



After modifying code:



1\. Review the complete diff.

2\. Check for compilation and syntax issues.

3\. Check references and dependent systems.

4\. Check for unintended changes.

5\. Run relevant tests or explain why they could not be run.

6\. Summarize the changes and remaining risks.



Never rewrite an entire file when a focused modification is sufficient.



Do not remove existing functionality unless explicitly requested or necessary for the approved change.



\## 10. Documentation



\- Document public APIs when their purpose or usage is not obvious.

\- Keep comments accurate when code changes.

\- Update relevant documentation when changing externally visible behavior.

\- Do not generate excessive documentation for trivial code.

\- Do not claim that undocumented behavior is guaranteed.

\- Use concrete examples where they improve understanding.



\## 11. Security and Configuration



\- Never hardcode API keys, passwords, access tokens, or other secrets.

\- Do not expose secrets in logs or generated documentation.

\- Do not commit local credentials or private configuration.

\- Validate data received from external sources.

\- Avoid executing untrusted scripts or commands.

\- Do not weaken security settings to bypass development problems.



\## 12. Final Response Requirements



For every completed coding task, provide:



1\. Summary of implemented changes.

2\. List of created, modified, or deleted files.

3\. Explanation of important implementation decisions.

4\. Validation performed.

5\. Tests executed and their results.

6\. Known limitations or unresolved issues.



Be precise about what was actually implemented and verified.



\## 13. Priority



Correctness, maintainability, compatibility, and preservation of existing functionality take priority over writing less code or completing a task as quickly as possible.

