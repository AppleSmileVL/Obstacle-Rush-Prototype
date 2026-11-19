# Unity Development Rules

## 1. Compatibility & Architecture

- **Target Version:** Unity `2022.3.62f2`. Check `Packages/manifest.json` before assuming any package or newer Unity API exists.
- **Existing Architecture:**
  - All scripts are in `Assets/CodeBase/` without explicit `namespace` declarations—match the style and conventions of the file you are editing.
  - Dependency injection uses custom `IDependency<T>` containers (`GlobalDependenciesContainer` for `Pauser`, `SceneDependenciesContainer` for scene systems). Respect existing `Construct(...)` patterns.
  - Configuration uses `ScriptableObject` assets (`RaceInfo`, `NitroSettingAsset`, `LaserGateAsset`, etc.). Keep tunable values in existing assets rather than hardcoding them.
- **Reuse First:** Inspect existing classes and interfaces (`ICarInteraction`, `Pauser`, `CarCameraComponent`, etc.) before adding new managers, singletons, or duplicate logic.

## 2. Lifecycle, Physics & Performance

- **Lifecycle & Events:** Subscribe to events (`Pauser`, `RaceStateTracker`, `TrackpointCircuit`) in `OnEnable`/`Start` and always unsubscribe in `OnDisable`/`OnDestroy`. Cancel active coroutines or clean up pooled/spawned objects on disable/destroy.
- **Update Loops:** Keep physics logic in `FixedUpdate` and camera follow updates in `LateUpdate`. Avoid heavy work, scene-wide searches (`FindObjectsOfType`), or unnecessary allocations inside `Update`/`FixedUpdate`.
- **Pooling:** Prefer `UnityEngine.Pool.ObjectPool<T>` (or lightweight reuse) for frequently spawned effects/objects instead of repeated `Instantiate`/`Destroy` calls during gameplay.

## 3. Scenes, Prefabs & Assets

- Do not manually edit `.unity` scene YAML or `.prefab` YAML unless explicitly requested and verified.
- Preserve existing `[SerializeField]` field names and types so serialized references on prefabs and scenes do not break.
- Never delete `.meta` files or alter asset GUIDs.

## 4. Scope & Validation

- Only modify systems directly required by the user's request. Before changing vehicle physics (`Car`, `CarChassis`, `WheelAxle`) or shared camera behavior, inspect all dependent components first.
- After editing C# scripts, review the changed code for syntax, null-reference safety, and lifecycle cleanup, and state clearly what the user should verify in the Unity Editor.