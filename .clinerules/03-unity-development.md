\# Unity Development Rules



\## 1. Project Environment



\- Unity version: `2022.3.62f2`.

\- This is an existing Unity project, not a new project.

\- Preserve the current project architecture and established coding conventions.

\- Do not upgrade Unity or project packages unless explicitly requested.

\- Do not introduce new dependencies without explaining their purpose and obtaining approval.

\- Prefer built-in Unity functionality and existing project dependencies when appropriate.



\## 2. Existing Project Structure



The main project directories include:



\- `Assets/CodeBase/` — C# source code.

\- `Assets/Global/` — global project assets and configuration.

\- `Assets/Materials/` — materials.

\- `Assets/Mesh/` — meshes and models.

\- `Assets/PostProcessing/` — post-processing assets.

\- `Assets/Prefabs/` — prefabs.

\- `Assets/Scenes/` — scenes.

\- `Assets/Sounds/` — audio assets.

\- `Assets/Standard Assets/` — existing standard assets.

\- `Assets/TextMesh Pro/` — TextMesh Pro assets.

\- `Assets/UI/` — interface assets.

\- `Packages/` — Unity package configuration.

\- `ProjectSettings/` — Unity project settings.



Important:

\- Scripts are located in `Assets/CodeBase/`, not in an assumed `Assets/Scripts/` directory.

\- Inspect the existing folder structure before creating new directories.

\- Follow the existing organization of scripts and assets.

\- Do not move or rename existing files merely to conform to a preferred folder structure.



\## 3. Existing Architecture



Before implementing or modifying a gameplay system:



1\. Find the relevant existing classes.

2\. Read their complete implementations.

3\. Inspect related interfaces, base classes, and ScriptableObjects.

4\. Find where components are instantiated and configured.

5\. Inspect relevant prefabs and scenes when necessary.

6\. Identify dependencies and references.

7\. Reuse existing systems where appropriate.



Do not create duplicate implementations of existing functionality.



Do not introduce a new manager, service, controller, event system, or singleton without first checking whether the project already has an equivalent.



\## 4. Unity Components and Lifecycle



\- Use Unity lifecycle methods appropriately.

\- Understand the execution order and responsibilities of `Awake`, `OnEnable`, `Start`, `Update`, `FixedUpdate`, and `LateUpdate`.

\- Do not place physics calculations in `Update` when they belong in `FixedUpdate`.

\- Use `LateUpdate` where appropriate for camera behavior that depends on updated object positions.

\- Avoid unnecessary work in frequently executed lifecycle methods.

\- Subscribe to events when an object becomes active and unsubscribe when appropriate.

\- Avoid duplicate event subscriptions.

\- Ensure that Unity object references are validated before use when they may be missing.

\- Respect Unity's object lifecycle and destroyed-object behavior.



\## 5. Physics and Vehicle Systems



The project includes an existing vehicle gameplay implementation.



Before changing vehicle physics:



\- Inspect the existing vehicle controller and related physics classes.

\- Identify how input, chassis, suspension, wheel axles, and vehicle behavior interact.

\- Understand which calculations are performed in `Update` and `FixedUpdate`.

\- Inspect Rigidbody configuration and relevant physics settings.

\- Preserve existing handling behavior unless the task explicitly requires changing it.

\- Avoid introducing competing physics controllers.

\- Do not arbitrarily change mass, drag, suspension, friction, forces, or other physical parameters.

\- Validate physics changes in the Unity Editor whenever possible.



\## 6. Cameras



\- Inspect the existing camera architecture before implementing camera behavior.

\- Reuse existing camera controllers and camera components where appropriate.

\- Avoid introducing a second camera follow system without justification.

\- Preserve existing camera transitions, follow behavior, FOV effects, and camera shake.

\- Consider execution order when the camera follows physics-driven objects.

\- Avoid unnecessary allocations and expensive calculations in camera update loops.



\## 7. Scenes and Prefabs



\- Inspect scene and prefab dependencies before modifying them.

\- Preserve serialized component references.

\- Preserve prefab links and overrides unless the task requires changing them.

\- Do not manually edit Unity scene or prefab YAML without a clear technical reason.

\- Do not remove components or serialized fields simply because they appear unused.

\- Avoid breaking references between scenes, prefabs, scripts, and assets.

\- Do not create duplicate prefabs when an existing prefab can be extended.

\- Preserve `.meta` files and their GUIDs.



\## 8. ScriptableObjects and Configuration



\- Inspect existing ScriptableObject definitions and assets before creating new ones.

\- Prefer existing configuration mechanisms where appropriate.

\- Avoid hardcoding values that are already configured through project assets.

\- Do not move configuration data into code without a clear reason.

\- Preserve serialized data when changing ScriptableObject fields.

\- Consider compatibility with existing serialized assets when renaming or removing fields.



\## 9. Packages and APIs



\- Check `Packages/manifest.json` before using a package.

\- Verify that the required package is installed and compatible with the project's Unity version.

\- Do not assume that an API exists in Unity 2022.3.

\- Do not use APIs from newer Unity versions without checking compatibility.

\- Avoid unnecessary package additions.

\- Do not modify package versions as a workaround for unrelated problems.



\## 10. Asset and Metadata Safety



\- Never manually delete `.meta` files.

\- Never change GUID values without an explicit technical requirement.

\- Do not modify generated folders such as `Library/`, `Temp/`, `Obj/`, or `Logs/`.

\- Do not replace existing assets with generated placeholders unless requested.

\- Preserve asset names and references where possible.

\- Inspect dependencies before moving or renaming assets.



\## 11. Unity Validation



After Unity-related code changes:



\- Check for syntax and compilation errors.

\- Inspect relevant references and serialized fields.

\- Check for missing scripts and missing references where possible.

\- Verify package compatibility.

\- Test the affected gameplay behavior in the Unity Editor when available.

\- Distinguish between static code inspection and actual runtime testing.

\- Never claim that a scene, prefab, or gameplay feature works without appropriate verification.



\## 12. Scope Control



\- Implement only what the user requested.

\- Avoid unrelated refactoring.

\- Do not redesign existing architecture during a small feature task.

\- Explain architectural changes before implementing them.

\- When a task requires changes across multiple systems, map the dependencies before editing.

\- Ask for approval before making major structural changes.



\## 13. Priority



Preserve the existing Unity project, its assets, serialized data, and gameplay behavior.



A minimal, compatible change is preferable to an unnecessary architectural rewrite.

