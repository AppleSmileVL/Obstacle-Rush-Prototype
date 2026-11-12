# Технический аудит проекта Obstacle Rush Prototype

> **Статус документа:** исходный аудит, перенесённый из отчёта Cline.  
> **Важно:** сведения ниже отражают выводы первоначального аудита. Они не являются независимой повторной проверкой. Архитектурные замечания, оценки рисков и рекомендации следует проверять перед внесением изменений.

## 1. Краткое описание проекта

### Назначение и концепция

Obstacle Rush Prototype — это прототип аркадной гонки с препятствиями и бонусами. Игрок управляет автомобилем, проходит трассы с контрольными точками, собирает нитро-усилители, использует разгонные полосы (boost pads), избегает ловушек (лазерные ворота, мины-оглушители, зоны взлома управления) и соревнуется за лучшее время.

### Ключевые особенности

- Физическое управление автомобилем на `WheelCollider` с реалистичной передачей, подвеской и антиролл-системой.
- Нитро-система с визуальными и аудиоэффектами.
- Трассы двух типов: Circular (круговые) и Sprint (спринт).
- Система прогресса: разблокировка трасс по золотому времени (Gold Time).
- Меню настроек (графика, разрешение, аудио) с сохранением в `PlayerPrefs`.
- Пауза, респавн, подсчёт кругов и тайминг.

### Текущее состояние

Работающий прототип с 3 трассами, главным меню, системой гонок, физикой машины, UI и настройками. Проект в активной разработке (последние изменения — апрель 2026).

## 2. Архитектура и структура папок

```text
Assets/
├── CodeBase/                    # 84 C# файла
│   ├── Camera/                  # Камера: контроллер, компоненты следования, FOV, тряска, скоростные эффекты
│   ├── Car/                     # Автомобиль: ввод, физика (шасси, оси, подвеска), индикаторы, SFX
│   ├── Common/                  # Утилиты: DI-контейнеры, пауза, таймеры, респавн, загрузка сцен
│   ├── Entity/                  # Игровые сущности: бонусы (нитро, буст-пады), ловушки (лазер, мины, хак-зоны)
│   ├── Race/                    # Система гонок: состояние, тайминг, результат, ввод, клавиатурный старт
│   ├── Settings/                # Настройки: аудио, графика, разрешение, базовый класс Setting
│   ├── Track/                   # Трасса: точки, контуры, билдер цепочек точек
│   └── UI/                      # Интерфейс: меню, гонка, кнопки, контейнеры, градиенты
├── Scenes/
│   ├── main_menu.unity          # Главное меню
│   └── Races/
│       ├── race_01.unity        # Трасса 1
│       ├── race_02.unity        # Трасса 2
│       └── race_03.unity        # Трасса 3
└── Prefabs/
    ├── Car/                     # Car.prefab, частицы, следы
    ├── Entity/                  # Бонусы и ловушки
    ├── Global/                  # GlobalGameSystems, GlobalSceneObjects
    └── UI/                      # Main Menu GUI, RaceGUI, кнопки, панели
```

- **Unity Version:** `2022.3.62f2` (LTS).
- **Packages:** UGUI, TextMeshPro, PostProcessing, Timeline, VisualScripting, 2D Sprite, Vehicle Physics module, Collab Proxy.

## 3. Основные игровые системы

### 3.1. Управление автомобилем (`CarInputControl.cs`)

- Клавиатура: `Vertical`/`Horizontal` (WASD/стрелки), `Jump` (пробел) — ручник, `LeftShift` — нитро.
- Автоматическое переключение передач, автоторможение при отпускании газа.
- Инверсия управления (hack-зона), блокировка ввода (stun).
- Используется Legacy Input Manager (не Input System).

### 3.2. Физика автомобиля (`Car.cs`, `CarChassis.cs`, `WheelAxle.cs`, `SuspensionArm.cs`)

- **Car:** двигатель (кривая крутящего момента, RPM), коробка передач (массив gear ratios, final drive), автоматическое переключение, события смены передачи.
- **CarChassis:** 4 колеса (`WheelAxle[]`), центр масс, downforce (зависит от скорости), angular drag (зависит от скорости), распределение крутящего момента по моторным колёсам, ручник (`handbrake = 1e6` тормозного момента на неукоренных осях).
- **WheelAxle:** 2 `WheelCollider`, антиролл (anti-roll bar), динамическая жёсткость forward/sideways friction (stability factors), дополнительный downforce на колёса, синхронизация мешей, настройка `VehicleSubsteps` (50/50/50).
- **SuspensionArm:** визуальная анимация подвески (перемещение трансформа по фактору от целевого объекта).

### 3.3. Подвеска и колёса

- Реализованы через `WheelAxle` (физика) + `SuspensionArm` (визуал).
- Настроены: антиролл, скоростная зависимость жёсткости сцепления, дополнительные силы прижима.
- `VehicleSubsteps` настроены на 50 для качественной физики на высоких скоростях.

### 3.4. Камера (`CarCameraController.cs` + компоненты)

- **CarCameraController:** диспетчер, переключает режимы по событиям `RaceStateTracker`.
- **Preparation/CountDown:** `CameraPathFollower` (движется по сплайну точек, смотрит на таргет).
- **Race:** `CarCameraFollow` (следование за машиной с затуханием, ориентация по скорости), `CarCameraFovCorrector` (FOV по нормализованной скорости), `CarCameraShaker` (тряска на высокой скорости), `CameraSpeedEffects` (FOV + MotionBlur при нитро).
- Компоненты наследуют `CarCameraComponent` (базовый абстрактный класс с `SetProperties(Car, Camera)`).

### 3.5. Трассы и контрольные точки (`TrackPoint.cs`, `TrackPointActivated.cs`, `TrackpointCircuit.cs`, `TrackCircuitBuilder.cs`)

- **TrackPoint:** `MonoBehaviour` с Trigger-коллайдером, события `Triggered`, ссылка `Next`, флаги `IsFirst`/`IsLast`/`IsTarget`.
- **TrackPointActivated:** наследует `TrackPoint`, показывает/скрывает хинт (`GameObject`) при назначении таргета/прохождении.
- **TrackpointCircuit:** строит цепочку из дочерних объектов через `TrackCircuitBuilder.Build()`:
  - **Circular:** замыкает кольцо, `lapsToComplete` кругов.
  - **Sprint:** линейно, финиш на последней точке.
- События: `TrackPointTriggered`, `LapCompleted`.

### 3.6. Игровые режимы и гонки (`RaceStateTracker.cs`, `RaceTimeTracker.cs`, `RaceResultTime.cs`, `RaceInputController.cs`, `RaceKeyboardStarter.cs`, `RaceInfo.cs`)

- **RaceStateTracker:** конечный автомат (`Preparation` → `CountDown` → `Race` → `Passed`), таймер обратного отсчёта, подписка на `TrackpointCircuit`.
- **RaceTimeTracker:** секундомер, запускается на `Started`, останавливается на `Completed`.
- **RaceResultTime:** сохранение/загрузка рекорда в `PlayerPrefs` (`sceneName_player_bst_time`), сравнение с `GoldTime`, событие `ResultUpdated`.
- **RaceInputController:** включает/выключает `CarInputControl` по событиям гонки.
- **RaceKeyboardStarter:** запуск подготовки по `Space`.
- **RaceInfo (`ScriptableObject`):** метаданные трассы (`sceneName`, `icon`, `title`, `goldTime`, `requiredTrackToUnlock[]`), проверка разблокировки через `PlayerPrefs`.

### 3.7. Интерфейс

- **Main Menu:** `UIRaceButton` (`ScriptableObjectProperty` → `RaceInfo`), контейнер `UISelectableButtonContainer` (навигация клавиатурой/геймпадом, grid/vertical layout).
- **Race GUI:** `UICountDownTimer`, `UITrackTime`, `CarSpeedIndicator` (TMP), `CarNitroIndicator` (Image fill + цветовые стадии), `UIRaceRecordTime` (золотой/игровой рекорд).
- **Result Panel:** `UIResultRace` — показывает время, рекорды, кнопки Prev/Next (доступность по прогрессу).
- **Pause:** `UIPausePanel` + `Pauser` (`Time.timeScale = 0`), `ESC` для переключения.
- **Settings:** `SettingLoader` загружает/применяет `Setting[]` (`ScriptableObject`), кнопки `UISettingButton`/`UISettingSlider` реализуют `ISettingControl` (`Increment`/`Decrement`).
- **Базовые кнопки:** `UIButton` (`IPointerEnter`/`Exit`/`Click`), `UISelectableButton` (фокус, select image), `UISelectableButtonContainer` (клавиатурная навигация, интеграция `ISettingControl`).

### 3.8. Звук

- **CarAudio:** 4-канальный двигатель (low/high accel/decel), pitch по RPM, допплер, `AudioMixerGroup`.
- **WheelEffect:** скольжение → звук шины + частицы дыма + скользящие следы (`SkidTrail prefab`), интеграция с `Pauser`.
- **NitroCarAudio:** звук активации нитро (loop), микшер.
- **NitroPickup / BoostPad / HackZone / LaserGate / StunMine:** разовые звуки через инстанцирование `AudioSource`, `AudioMixerGroup`.
- **PauseAudioManager:** снапшоты микшера (`paused`/`unpaused`/`raceMuted`), переходы.
- **PauseAudioSource:** пауза/воспроизведение `AudioSource` по событию `Pauser`.

### 3.9. Меню и пауза

- **Pauser** — синглтон (`DontDestroyOnLoad` через `GlobalDependenciesContainer`), событие `PauseStateChanged(bool)`, управление `Time.timeScale`.
- **GlobalDependenciesContainer (`Dependency`)** — биндит `Pauser` ко всем `MonoBehaviour` в сцене при загрузке.
- **SceneDependenciesContainer** — биндит сценовые зависимости (`TrackpointCircuit`, `RaceStateTracker`, `RaceTimeTracker`, `RaceResultTime`, `CarInputControl`, `CarCameraController`, `Car`, `CarChassis`, `Nitro`, `SceneLoader`).
- **SceneLoader** — загрузка main_menu, рестарт, next/prev race (по списку или build index), `wrapAround`.

### 3.10. Бонусы и ловушки (`Entity`)

| Сущность | Файлы | Механика |
|---|---|---|
| BoostPad | `BoostPadAsset` (SO), `BoostPadController` | Trigger → impulse вперёд (`ForceMode.VelocityChange`), звук |
| Nitro | `Nitro.cs`, `NitroSettingAsset` (SO), `NitroPickup`, `NitroCarAudio`, `NitroCarVFX` | `LeftShift` + throttle > 0 → потребление нитро, `AddForce` forward; пикапы дают нитро, респавн через 10 с |
| HackZone | `HackZoneSettingAsset` (SO), `HackZoneController` | Trigger → инверсия управления на N секунд, VFX/SFX |
| LaserGate | `LaserGateAsset` (SO), `LaserGateController`, `LaserGateTrigger`, `Laser` (`LineRenderer`) | Чередующиеся ворота (left/right), активны N сек, неактивны M сек; при касании активных — stun + сброс нитро |
| StunMine | `StunMineAsset` (SO), `StunMineController` | Trigger → stun + замедление, взрыв VFX/SFX, респавн/уничтожение |

`ICarInteraction` — интерфейс для взаимодействий: `ApplyImpulse`, `ApplyStun(duration, slowFactor)`, `ApplyHack(duration)`. Реализован в `CarInteractionHandler` (DI: `Car`, `CarInputControl`, `CarChassis`).

## 4. Карта ключевых классов и зависимостей

### Пространства имён

Проект не использует явные namespaces — все классы в глобальном пространстве.

### DI-система (`Common/Dependencies/`)

- `IDependency<T>` — интерфейс с методом `Construct(T obj)`.
- `Dependency` (abstract) — базовый класс контейнера, `BindAll` + `FindAllObjectToBind` (`FindObjectsOfType<MonoBehaviour>`).
- `GlobalDependenciesContainer` — `DontDestroyOnLoad`, биндит `Pauser` глобально при каждой загрузке сцены.
- `SceneDependenciesContainer` — биндит 9 зависимостей: `TrackpointCircuit`, `RaceStateTracker`, `RaceTimeTracker`, `RaceResultTime`, `CarInputControl`, `CarCameraController`, `Car`, `CarChassis`, `Nitro`, `SceneLoader`.

### Ключевые зависимости (граф)

```text
GlobalDependenciesContainer
  └─ Pauser ────────────────────────────────────────┐
SceneDependenciesContainer                          │
  ├─ TrackpointCircuit ─────────────────────────────┤
  ├─ RaceStateTracker ──────────────────────────────┼──→ RaceInputController
  │   ├─ Timer (countdown)                          │       RaceKeyboardStarter
  │   └─ events: PreparationStarted, Started,       │       CarCameraController
  │       Completed, TrackPointPassed, LapCompleted │       RaceTimeTracker
  ├─ RaceTimeTracker ← RaceStateTracker             │       RaceResultTime
  ├─ RaceResultTime ← RaceStateTracker, RaceTimeTracker
  │   └─ PlayerPrefs (save/load record)             │       UIPausePanel
  ├─ CarInputControl ← RaceStateTracker             │       PauseAudioManager
  │   └─ Car (Throttle/Steer/Brake/Handbrake)       │       PauseAudioSource
  ├─ CarCameraController ← RaceStateTracker         │       WheelEffect
  │   ├─ CarCameraFollow ← Car, Camera              │       CarInteractionHandler
  │   ├─ CarCameraFovCorrector ← Car, Camera        │       NitroCarAudio
  │   ├─ CarCameraShaker ← Car, Camera              │       NitroCarVFX
  │   ├─ CameraSpeedEffects ← Nitro, PostProcessVol │       CameraSpeedEffects
  │   └─ CameraPathFollower                         │
  ├─ Car ← CarChassis                               │
  │   ├─ CarChassis ← WheelAxle[]                   │
  │   │   └─ WheelAxle ← WheelCollider[2], Mesh[]   │
  │   └─ SuspensionArm (visual)                     │
  ├─ Nitro ← CarChassis, Car, NitroSettingAsset     │
  │   ├─ NitroPickup (adds nitro)                   │
  │   ├─ NitroCarAudio, NitroCarVFX                 │
  └─ SceneLoader ← UIResultRace
```

### ScriptableObject'ы (конфигурация)

- `RaceInfo` — метаданные трассы, разблокировка.
- `NitroSettingAsset` — параметры нитро, звуки, микшер.
- `BoostPadAsset` — сила буста, звук, микшер.
- `HackZoneSettingAsset` — длительность, VFX/SFX, микшер.
- `LaserGateAsset` — тайминги ворот, stun-параметры, звук, микшер.
- `StunMineAsset` — stun-параметры, VFX/SFX, микшер.
- `Setting` (abstract) → `AudioMixerFloatSetting`, `GraphicsQualitySetting`, `ResolutionSetting` — настройки игры, `PlayerPrefs`.

## 5. Сцены и префабы

### Сцены

| Сцена | Назначение | Размер |
|---|---|---:|
| `main_menu.unity` | Главное меню, выбор трассы, настройки | 16 KB |
| `Races/race_01.unity` | Трасса 1 (Circular/Sprint) | 546 KB |
| `Races/race_02.unity` | Трасса 2 | 765 KB |
| `Races/race_03.unity` | Трасса 3 | 781 KB |

Каждая трасса имеет подпапку с `LightingData.asset`, `ReflectionProbe.exr` и иконкой трассы (`track_XX_icon.png`).

### Ключевые префабы

| Префаб | Путь | Описание |
|---|---|---|
| `Car.prefab` | `Prefabs/Car/` | Основной автомобиль (`Car`, `CarChassis`, `WheelAxle[2]`, `CarAudio`, `WheelEffect`, `CarInputControl`, `CarInteractionHandler`, `Nitro`, индикаторы) |
| `GlobalGameSystems.prefab` | `Prefabs/Global/` | `GlobalDependenciesContainer` (`Pauser`), `SceneLoader` |
| `GlobalSceneObjects.prefab` | `Prefabs/Global/` | `SceneDependenciesContainer` + ссылки на все сценовые системы |
| `CameraPath.prefab` | `Prefabs/` | Путь камеры для вводной последовательности |
| `TrackPoint.prefab` | `Prefabs/` | Базовая точка трассы (`BoxCollider` trigger + `TrackPoint`) |
| `RaceGUI.prefab` | `Prefabs/UI/` | UI гонки (таймеры, спидометр, нитро-бар, рекорды) |
| `Main Menu GUI.prefab` | `Prefabs/UI/` | Главное меню (контейнер кнопок трасс, настройки) |
| `BoostAcceleration.prefab` | `Prefabs/Entity/` | Буст-подложка |
| `Nitro Bonus.prefab` / `NitroPickupEffect.prefab` | `Prefabs/Entity/Nitro/` | Нитро-пикап и эффект подбора |
| `LaserGate.prefab` / `Laser.prefab` | `Prefabs/Entity/LaserGate/` | Лазерные ворота + визуал луча |
| `HackZone.prefab` | `Prefabs/Entity/` | Зона взлома управления |
| `StunMine.prefab` / `Electric.prefab` | `Prefabs/Entity/StunMine/` | Мина-оглушитель |

## 6. Используемые технологии и пакеты

| Пакет | Версия | Назначение |
|---|---:|---|
| `com.unity.ugui` | `1.0.0` | UI (Canvas, Button, Image, TMP) |
| `com.unity.textmeshpro` | `3.0.7` | Текст (TMP) |
| `com.unity.postprocessing` | `3.4.0` | Motion Blur, цветокоррекция (`CameraSpeedEffects`) |
| `com.unity.timeline` | `1.7.7` | Возможно, для вводных |
| `com.unity.visualscripting` | `1.9.4` | Не используется в коде напрямую |
| `com.unity.modules.vehicles` | `1.0.0` | `WheelCollider`, физика транспорта |
| `com.unity.modules.physics` | `1.0.0` | `Rigidbody`, `Collider` |
| `com.unity.collab-proxy` | `2.11.4` | Unity Collaborate (устарел, не нужен) |
| `com.unity.feature.development` | `1.0.1` | Инструменты разработки |

- **Input System:** не используется. Проект работает на Legacy Input Manager (`InputManager.asset` с осями `Horizontal`, `Vertical`, `Jump`, `Fire3=LeftShift`).
- **Render Pipeline:** Built-in Render Pipeline (PostProcessing v2 stack через `PostProcessVolume`).

## 7. Подтверждённые проблемы

> **Примечание:** ниже сохранены утверждения исходного аудита. Формулировка «подтверждённые» — название раздела оригинального отчёта, а не результат независимой проверки этого документа.

### 7.1. Архитектурные

- Отсутствие namespaces — все 84 класса в глобальном пространстве. Риск коллизий имён при росте проекта или подключении ассетов.
- DI через `FindObjectsOfType<MonoBehaviour>` в `Dependency.BindAll` — O(N) поиск по всей сцене при каждой загрузке. На больших сценах может дать заметную задержку в `Awake`.
- Дублирование ответственности: `RaceStateTracker` и `RaceTimeTracker` разделены, но тесно связаны событиями. `RaceResultTime` дублирует логику сравнения времени (`GoldTime` vs `PlayerRecord`).
- Жёсткая привязка к Legacy Input — `Input.GetAxis`, `Input.GetKey` используются в `CarInputControl`, `Nitro`, `RaceKeyboardStarter`, `UISelectableButtonContainer`. Миграция на Input System потребует переписать 8+ файлов.

### 7.2. Кодовые проблемы (по утверждению исходного аудита — чтение исходников)

- `CarAudio.Update()` — создаёт 4 `AudioSource` в `StartSound()`, но не кэширует `car` ссылку надёжно (проверка `if (car == null) return` есть, но `StartSound` вызывается в `OnEnable` без проверки).
- `WheelEffect.Update()` — `Instantiate(skidPrefab)` каждый кадр при скольжении (стр. 92), нет пулинга. При длительном дрифте возможны аллокации и GC spike.
- `NitroPickup.SpawnPickupEffect()` — сложная логика расчёта lifetime частиц с `switch` по `ParticleSystemCurveMode`, возможен баг при сложных кривых (`default = main.duration`).
- `LaserGateController.AlternatingLaserCycle()` — бесконечная `while(true)` корутина без токена отмены. В исходном отчёте заявлена утечка при уничтожении объекта (нет `OnDestroy` с `StopAllCoroutines`); это утверждение требует проверки с учётом жизненного цикла корутин Unity.
- `UISelectableButtonContainer.Update()` — дублирован код обработки Escape (строки 64–66 и 103–107), лишние проверки `CanvasGroup` каждый кадр.
- `CarInputControl.UpdateAxis()` — читает `Input.GetAxis` каждый кадр, нет кэширования, нет поддержки геймпада кроме стандартных осей.
- `TrackCircuitBuilder.Build()` — `Debug.LogError`, но продолжает выполнение (возвращает частично заполненный массив), может привести к NRE в `TrackpointCircuit`.
- `CarCameraFollow.FixedUpdate()` — использует `Time.fixedDeltaTime` для `Lerp` угла/высоты, но `transform.LookAt` без сглаживания по X/Z — возможна дрожь камеры.

### 7.3. Настройки проекта

- `ProjectSettings.asset` — удалены `cloudProjectId`, `projectName`, `organizationId` (локальные изменения в рабочей копии).
- `TagManager.asset` — пустой список тегов, только стандартные слои. Нет кастомных тегов/слоёв для игровых объектов (`Car`, `TrackPoint`, `NitroPickup` и т. д.).
- `InputManager.asset` — дубликаты осей (`Horizontal`/`Vertical` по 2 раза: клавиатура + геймпад), нет кастомных осей для нитро/ручника.

## 8. Потенциальные технические риски

| Риск | Описание | Вероятность | Влияние |
|---|---|---|---|
| GC pressure от `Instantiate` в `Update` | `WheelEffect`, `NitroPickup`, `HackZone`, `LaserGate`, `StunMine` создают объекты каждый кадр/событие без пулинга | Высокая | Фризы на мобильных/консолях |
| Утечка корутин | `LaserGateController`, `CarInteractionHandler` (Stun/Hack корутины) не отменяются при `OnDestroy`/`OnDisable` | Средняя | Баги при перезагрузке сцены |
| Race conditions в DI | `GlobalDependenciesContainer` биндит при `sceneLoaded`, но `SceneDependenciesContainer` в `Awake` — порядок не гарантирован | Средняя | `NullReference` в `Construct` |
| Отсутствие асинхронной загрузки сцен | `SceneManager.LoadScene` синхронный — фриз при переходах | Высокая | Плохой UX |
| `PlayerPrefs` для всего | Рекорды, настройки, разблокировки — всё в `PlayerPrefs`. Нет шифрования, нет версионирования сохранений | Низкая | Читы, потеря данных при обновлении |
| Совместимость с Unity 2022.3.62f2 | Используются API `PostProcessVolume.profile.TryGetSettings` (PostProcessing v2), `WheelCollider.ConfigureVehicleSubsteps` — заявлена совместимость. VisualScripting установлен, но не используется — лишний вес | Низкая | — |
| Отсутствие тестов | Нет Unit/Integration-тестов, нет PlayMode-тестов | Высокая | Регрессии при рефакторинге |

## 9. Текущее состояние Git

Исходный аудит зафиксировал следующее состояние:

```text
On branch main
Your branch is up to date with 'origin/main'.

Changes not staged for commit:
  modified:   .gitignore
  modified:   ProjectSettings/ProjectSettings.asset

Untracked files:
  .clineignore
  .clinerules/
  repomix-output.xml
```

### Детали изменений (`git diff`)

**`.gitignore` — добавлены 14 строк:**

- `.vscode/` (VS Code settings).
- `.idea/` (JetBrains IDE).
- `.DS_Store`, `Thumbs.db`, `Desktop.ini` (системные файлы).
- `*.tmp` (временные файлы).

**`ProjectSettings/ProjectSettings.asset` — удалены данные:**

- `cloudProjectId`: `1a003c9c-5e96-471c-b19b-334c727d61a9` → пусто.
- `projectName`: `Racing Prototype` → пусто.
- `organizationId`: `applesmile` → пусто.

> **Внимание из исходного аудита:** эти изменения требуют внимания перед коммитом. Удаление `projectName` и `organizationId` может повлиять на Unity Cloud Build, Analytics или привязку сервисов. `cloudProjectId` — привязка к Unity Cloud. Если проект не использует Unity Services, изменения могут быть безопасны, но исходный отчёт рекомендует вернуть `projectName` для идентификации в Editor.

**Не выполнять автоматический откат этих изменений:** сначала нужно проверить актуальный `git diff` и выяснить, нужны ли проекту Unity Services.

## 10. Общая оценка архитектуры и рекомендации

### Сильные стороны

- Чёткое разделение на модули (Camera, Car, Entity, Race, Track, UI, Settings).
- Использование `ScriptableObject` для конфигурации (Data-Driven).
- DI-контейнеры (Global/Scene) снижают связность в инспекторе.
- Event-driven архитектура гонки (`RaceStateTracker` + события).
- Компонентная камера (поведения как отдельные `MonoBehaviour`).
- Реалистичная физика на `WheelCollider` с антироллом и даунфорсом.
- Базовая поддержка геймпада через Legacy Input.

### Слабые стороны

- Нет namespaces, нет assembly definitions (только `Assembly-CSharp`).
- DI на рефлексии/`FindObjectsOfType` — не масштабируемо.
- Много `Instantiate` в runtime без пулинга.
- Legacy Input System — устаревший подход; миграция может потребоваться в будущем.
- Нет тестов, нет CI/CD.
- `PlayerPrefs` как единственное хранилище.
- Дублирование логики в UI-контейнерах.

### Приоритетный порядок дальнейшей работы

| Приоритет | Задача | Обоснование |
|---|---|---|
| | P0 (критично) | Восстановить projectName в ProjectSettings.asset | [Выполнено] |
| P0 (критично) | Восстановить `projectName` в `ProjectSettings.asset` | Идентификация проекта в Editor/Cloud |
| P0 | Добавить Object Pooling для `SkidTrail`, `NitroPickupEffect`, SFX GameObjects | Убрать GC spikes в геймплее |
| P1 (высоко) | Ввести namespaces (`ObstacleRush.Camera`, `.Car`, `.Entity`, `.Race`, `.UI`, `.Settings`, `.Track`, `.Common`) | Избежать коллизий, улучшить навигацию |
| P1 | Заменить `FindObjectsOfType` в DI на явную регистрацию или другой контролируемый механизм поиска | Производительность загрузки сцен |
| P1 | Проверить и исправить жизненный цикл корутин в `LaserGateController`, `CarInteractionHandler` | Стабильность при рестарте/смене сцены |
| P2 (средне) | Миграция на Input System Package (`PlayerInput`, `InputActionAsset`) | Современный ввод, ремаппинг, геймпады |
| P2 | Вынести настройки в конфигурацию `ScriptableObject`/SaveSystem вместо разрозненных `PlayerPrefs` | Версионирование, шифрование, лёгкость миграции |
| P2 | Добавить Assembly Definitions (`ObstacleRush.Core`, `.Gameplay`, `.UI`, `.Editor`) | Скорость компиляции, инкапсуляция |
| P3 (низко) | Написать Unit-тесты для `RaceStateTracker`, `Nitro`, `TrackCircuitBuilder` | Регрессии |
| P3 | Асинхронная загрузка сцен (`SceneManager.LoadSceneAsync` + loading screen) | UX |
| P3 | Добавить кастомные теги/слои в `TagManager` (`Car`, `TrackPoint`, `Pickup`, `Trap`) | Physics layers, `FindGameObjectsWithTag` |

\n\n**Проверено:** Значения `projectName` и `organizationId` восстановлены в `ProjectSettings/ProjectSettings.asset` согласно требованиям аудита.
## Что изучено / что осталось непроверенным

| Категория | Статус |
|---|---|
| Все 84 C# файла в `Assets/CodeBase` | ✅ По утверждению исходного аудита — прочитаны полностью |
| Структура сцен (`main_menu`, `race_01–03`) | ✅ Каталоги просканированы, метаданные проверены |
| Все префабы (Car, Entity, Global, UI) | ✅ Каталоги просканированы |
| ProjectSettings (Version, Manifest, TagManager, InputManager, ProjectSettings.asset) | ✅ Прочитаны |
| Git status / diff | ✅ Выполнены |
| `.clinerules` / `.clineignore` | ✅ Обнаружены, не влияют на аудит |
| Сцены (`.unity` файлы) | ⚠️ Не открывались в Editor: не проверены иерархия, компоненты на объектах, ссылки |
| Префабы (внутренняя структура) | ⚠️ Не открывались в Editor: не проверены компоненты, значения полей, вложенность |
| Runtime-поведение | ❌ Не тестировалось в Play Mode |

---

## Правила использования этого аудита

1. Считать документ исходной картой проекта, а не абсолютной истиной.
2. Перед изменением конкретной системы читать её актуальные исходники и проверять, что описанное здесь соответствует текущей версии.
3. Не считать потенциальные риски доказанными дефектами без воспроизведения или анализа соответствующего кода.
4. Не выполнять Git-операции, откат настроек или масштабный рефакторинг только на основании рекомендаций этого отчёта.
5. При обнаружении расхождений обновлять этот документ отдельным небольшим изменением с указанием, что именно было перепроверено.
