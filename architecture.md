# Архитектура проекта «Test Case PICO 4 empty (U3D)»

Документ описывает устройство VR-приложения: состав сцены, поток данных, рантайм-логику,
рендеринг, ввод и процесс сборки. Рассчитан на разработчика, который впервые открывает проект.

---

## 1. Что делает приложение

Интерактивный ветвящийся видео-сценарий в VR на PICO 4. Пользователь находится внутри
панорамного окружения (equirect-панорама как skybox), перед ним экран с SBS-видео
(stereo side-by-side), после окончания ролика появляются кнопки A/B, выбор переводит
сценарий на следующий узел. Ветка заканчивается «финалом» с текстом и кнопкой рестарта.

Сценарий целиком описан данными в JSON — код сценария не меняется при правке контента.

---

## 2. Стек

| Компонент | Версия / значение |
|---|---|
| Unity | 6000.0.62f1 (Unity 6) |
| Render pipeline | Universal RP 17.0.4 |
| XR Management | 4.7.0 |
| OpenXR | 1.16.1 (загрузчик только для Standalone) |
| PICO Integration SDK | 3.4.0, локальный пакет `file:G:/Unity Projects/PICO Unity SDK` |
| XR Interaction Toolkit | 3.0.8 (подтягивается транзитивно) |
| Input System | 1.14.2 |
| UI | uGUI 2.0.0 + TextMeshPro |
| Платформа | Android, ARM64, IL2CPP, minSdk 29, targetSdk auto (36) |
| Graphics API (Android) | OpenGLES3 (задан вручную, без Automatic) |
| Целевое устройство | PICO 4 (Android 10, PICO OS 5.13+, XR-рантайм PICO OpenXR) |

Пакеты перечислены в `Packages/manifest.json`, фактическое разрешение зависимостей — в `Packages/packages-lock.json`.

---

## 3. Структура репозитория

```
Assets/
├─ Scenes/
│  ├─ SampleScene 1.unity      ← рабочая сцена (входит в сборку)
│  └─ SampleScene.unity        ← шаблонная сцена URP/XRI, в сборку не входит
├─ Scripts/                    ← вся игровая логика (12 файлов)
├─ Resources/                  ← контент, загружаемый по имени в рантайме
│  ├─ scenario.json            ← граф сценария
│  ├─ video_1..3.wmv           ← SBS-видео (источники)
│  ├─ panorama_*.exr           ← панорамы окружения
│  └─ PXR_*.asset              ← настройки PICO SDK
├─ Materials/                  ← материалы кнопок, skybox, SBS-видео
├─ Shaders/                    ← PanoramaUnlit.shader, SBSVideo.shader
├─ RenderTexture/              ← VideoRenderTexture (3840×1080)
├─ Settings/                   ← URP-ассеты (Mobile/PC), volume profile, Build Profile «PICO 4»
├─ XR/                         ← загрузчики и настройки XR (PXR_Loader, OpenXR)
├─ XRI/, Samples/, TutorialInfo/ ← служебное из шаблонов и сэмплов
ProjectSettings/                ← Player/Build/Quality/XR-настройки проекта
Packages/                       ← манифест пакетов
Library/, Temp/, Logs/          ← генерируемое, в git не входит
```

---

## 4. Сцена `SampleScene 1`

Иерархия и роли ключевых объектов:

| Объект | Роль |
|---|---|
| `[Building Block] PICO Hand Tracking XR Origin (XR Rig)` | XR Origin + настройка трекинга рук; база для камеры |
| `Camera Offset` → `Main Camera` | Камера VR (`Target Eye = Both`, `TrackedPoseDriver`), единственная активная |
| `XR Interaction Manager` | Менеджер XRI (рейкасты/интеракторы) |
| `EventSystem` | Обработка UI-событий uGUI |
| `Canvas` / `Visual Elements` | World-space UI: тексты TMP, кнопки, экран финала |
| `VirtualButtonPanel` | Дочерний объект `Main Camera`: контейнер кнопок A/B/Restart, привязанный к полю зрения (`tcFovPanelAnchor`: 0.5 м, 32° вниз) |
| `VideoScreen` | Quad с `SBSVideoMaterial`, отображает `VideoRenderTexture` |
| `tcScenarioManager` | `ScenarioManager` + `tcScenarioUI` — оркестратор сценария и UI |
| `tcScenarioHolder` | `tcScenarioHolder` — загрузка и валидация `scenario.json` |
| `tcVideoController` | `tcVideoController` + `VideoPlayer` (output → `VideoRenderTexture`) |
| `tcPanoramaController` | `tcPanoramaController` — подмена skybox-панорамы |
| `tcHandInputController` | `tcHandControler` — определение касания кнопок пальцем |
| `VirtualButtonA` / `VirtualButtonB` / `VirtualButtonRestart` | Коллайдеры + `tcVirtualButton` (A / B / Restart) |
| `Cube`, `Directional Light`, `Global Volume` | Статичное демо-окружение и пост-обработка |

Сцена активна в Build Settings как единственная запись — см. `ProjectSettings/EditorBuildSettings.asset`.
`SampleScene.unity` — остаток шаблона, оставлен для сравнения/отладки.

---

## 5. Рантайм-архитектура

### 5.1 Компоненты и ответственность

| Скрипт | Ответственность |
|---|---|
| `ScenarioManager` | Единая точка входа игрового цикла: старт, переходы между узлами, реакция на выбор игрока, реакция на конец видео |
| `tcScenarioHolder` | `Resources.Load<TextAsset>("scenario")` → `JsonUtility.FromJson<ScenarioConfig>` → словарь узлов + валидация |
| `tcVideoController` | `Resources.Load<VideoClip>(id)` → `VideoPlayer.clip` → `Play()`; событие `PlaybackFinished` |
| `tcPanoramaController` | `Resources.Load<Texture>(id)` → `_MainTex` runtime-копии skybox-материала → `RenderSettings.skybox` |
| `tcScenarioUI` | Показ/скрытие кнопок и текста финала, подписка на кнопки, отладочные клавиши 1–4 |
| `tcVirtualButton` | «Нажатие» кнопки: коллайдер + `Press()` → `tcScenarioUI.ButtonPressed(choice)` |
| `tcFovPanelAnchor` | Ставит панель кнопок в нижнюю часть поля зрения: 0.5 м от глаз, 32° ниже оси взгляда, лицом к пользователю |
| `tcHandControler` | Каждый кадр: кончик пальца из `PXR_Hand` → `Physics.OverlapSphere` → ближайшая `tcVirtualButton` → `Press()` |
| `ScenarioNode` (ScriptableObject) | Альтернативная, **не используемая в рантайме** модель узла (осталась от ранней версии) |
| `MouseButtonTester`, `VideoPlayerTest` | Отладочные утилиты (мышь в редакторе, принудительный Play) |
| `HandButtonTouch` | Пустой файл-заглушка |

### 5.2 Поток выполнения

```
ScenarioManager.Start()
   ├─ подписка на tcVideoController.PlaybackFinished
   ├─ tcScenarioHolder.LoadScenario()            // JSON → Dictionary<id, node>
   ├─ подписка tcScenarioUI.ChoiceA / ChoiceB
   └─ RestartScenario()
         └─ EnterNode(startNodeId)
               ├─ currentNode = node
               ├─ isVideoFinished = false
               ├─ VideoController.PlayVideo(node.videoId)     // если тип Video
               └─ PanoramaController.SetPanorama(node.panoramaId)

VideoPlayer.loopPointReached
   └─ tcVideoController.PlaybackFinished → ScenarioManager.HandleVideoFinished()
         └─ isVideoFinished = true  → tcScenarioUI показывает кнопки A/B

tcVirtualButton.Press() → tcScenarioUI.ButtonPressed(choice)
   └─ ScenarioManager.ChooseA()/ChooseB()
         ├─ блокировка, пока видео не закончилось (!isVideoFinished)
         ├─ проверка существования узла nextA/nextB
         └─ EnterNode(nextNodeId)  → следующее видео или финал

Для узла Ending: показывается endingText, кнопки A/B скрыты, доступен Restart.
```

`tcScenarioUI.Update()` каждый кадр вызывает `RefreshUI()` — видимость кнопок и текста
выводится из состояния `ScenarioManager` (`CurrentNode`, `isVideoFinished`), а не хранится в UI.

### 5.3 Модель данных сценария

`Assets/Resources/scenario.json` → классы `ScenarioConfig` / `ScenarioNodeData` (`ScenarioJsonData.cs`).
`JsonUtility` требует плоской структуры — переходы заданы строковыми id, а не ссылками.

```json
{
  "startNodeId": "start",
  "nodes": [
    {
      "id": "start",
      "type": "Video",
      "panoramaId": "panorama_1",
      "videoId": "video_1",
      "choiceAText": "A",
      "choiceBText": "B",
      "nextA": "episode_2",
      "nextB": "episode_3",
      "endingText": ""
    }
  ]
}
```

Правила, которые проверяет `tcScenarioHolder.LoadScenario()`:

- `startNodeId` не пустой и существует среди `nodes`;
- `id` уникальны и не пустые;
- `type` приводится к `ScenarioNodeKind` (`Video` / `Ending`);
- для каждого `Video`-узла существуют узлы `nextA` и `nextB`.

`panoramaId` и `videoId` — **имена ассетов** в `Resources` без расширения
(`Resources.Load` не принимает путь с папками/расширением).

---

## 6. Рендеринг

- **Пайплайн**: URP. В `QualitySettings` единственный уровень `Mobile`, он указывает на
  `Assets/Settings/Mobile_RPAsset.asset`; `GraphicsSettings` по умолчанию ссылается на `PC_RPAsset`,
  но на Android действует уровень качества.
- **Окружение**: панорама `panorama_*.exr` (equirect, EXR) подставляется в `_MainTex`
  материала `Assets/Materials/Panorama skyBox.mat` (Skybox/Panoramic) и назначается в
  `RenderSettings.skybox` в рантайме. Материал клонируется (`new Material(template)`),
  чтобы не менять ассет.
- **Видео**: `VideoPlayer` (source = `VideoClip`) рендерит в `Assets/RenderTexture/VideoRenderTexture.renderTexture`
  (3840×1080 — SBS-кадр, по 1920×1080 на глаз). RenderTexture натянута на quad `VideoScreen`
  через `Assets/Materials/SBSVideoMaterial.mat`.
- **Стерео**: `Assets/Shaders/SBSVideo.shader` в вершинном/фрагментном проходе использует
  `unity_StereoEyeIndex`: левый глаз берёт `uv.x * 0.5`, правый — `uv.x * 0.5 + 0.5`.
  Поэтому источник видео должен быть side-by-side, а не моно.
- **Пост-обработка**: `Global Volume` + `DefaultVolumeProfile`.

---

## 7. Ввод и взаимодействие

- **Руки**: PICO Building Blocks (`[Building Block] PICO Hand Tracking ...`) создают `PXR_Hand`
  для левой и правой руки. `tcHandControler` читает `handJoints[(int)HandJoint.JointIndexTip]`,
  делает `Physics.OverlapSphere(fingertip, touchRadius = 0.04f)` и вызывает `Press()` у найденной
  `tcVirtualButton` (поиск через `GetComponentInParent`).
- **Мышь (редактор/отладка)**: `tcVirtualButton.OnMouseDown()` и `MouseButtonTester` (луч из камеры).
- **Клавиатура (редактор)**: `tcScenarioUI.KeyboardDebugUpdate()` — клавиши `1`–`4` двигают
  `rightHand` к кнопкам A / B / Restart и обратно в исходную точку (быстрая проверка без шлема).
  Позиция берётся от текущей позы кнопки (`+0.2 м` в сторону пользователя), потому что панель
  постоянно смещается вместе с головой.
- **Панель кнопок**: `VirtualButtonPanel` — дочерний объект `Main Camera`, поэтому повторяет
  движение головы без задержки. `tcFovPanelAnchor` пересчитывает локальную позу панели по
  `distance` (0.5 м) и `pitchDown` (32°): кнопки всегда в нижней части поля зрения, лицом к
  пользователю. `distance` измеряется от глаз до центра панели по оси взгляда.
- **Контроллеры**: XRI `XR Interaction Manager` присутствует, но текущая логика кнопок завязана
  на физические коллайдеры и трекинг рук, а не на XRI-интеракторы.

---

## 8. Сборка и запуск

- **Build Profile**: `Assets/Settings/Build Profiles/PICO 4.asset` (Android) — задаёт
  `applicationIdentifier: com.ailab.pico4test`, `bundleVersion: 0.1.0`, использует глобальный список сцен.
- **Список сцен**: `ProjectSettings/EditorBuildSettings.asset` — рабочая сцена
  `Assets/Scenes/SampleScene 1.unity` должна стоять индексом 0 (исторически там был шаблонный
  `SampleScene.unity`).
- **Подпись**: в Player Settings включён Custom Keystore (`androidUseCustomKeystore: 1`),
  путь к хранилищу и alias задаются там же (файл `G:\Unity Projects\Keystore\user.keystore`).
  Без заполненных пути/пароля сборка падает на этапе подписи.
- **Деплой**: `Build And Run` устанавливает APK через Gradle и запускает
  `com.ailab.pico4test/com.unity3d.player.UnityPlayerActivity` на подключённом устройстве.
- **Отладка на устройстве**: `adb logcat` (тег `Unity`) — сообщения `Debug.Log`/`LogError`
  видны напрямую; PICO SDK пишет собственные логи с префиксом `PXRLog`.
- **Editor**: Play Mode с XR Device Simulator (сэмпл XRI) либо отладочные клавиши/мышь.

---

## 9. Как расширять

| Задача | Что менять |
|---|---|
| Добавить эпизод | Новый узел в `scenario.json` + видео/панорама в `Resources` с теми же именами, что в `videoId`/`panoramaId` |
| Поменять ветвление | Поля `nextA`/`nextB` в JSON (валидация проверит существование узлов) |
| Поменять текст финала | `endingText` соответствующего `Ending`-узла |
| Добавить тип узла | Расширить `ScenarioNodeKind`, обработку в `ScenarioManager.EnterNode`, валидацию в `tcScenarioHolder` |
| Поменять форму экрана видео | Меш `VideoScreen` + материал; сам кадр — `VideoRenderTexture` (учитывать SBS-раскладку в шейдере) |
| Поменять место/наклон панели кнопок | `tcFovPanelAnchor` на `VirtualButtonPanel`: `distance` (м от глаз), `pitchDown` (градусы ниже оси взгляда) |
| Настроить параметры PICO (частота, foveation, MRC) | `Assets/XR/Settings/PXR_Settings.asset`, `Assets/Resources/PICOProjectSetting.asset`, компонент `PXR_Manager` в сцене |
| Добавить сцену | Build Settings: новая сцена + переход через `SceneManager` (сейчас сцена одна) |

---

## 10. Соглашения и архитектурные ограничения

- **Контент только из `Resources`**: и видео, и панорамы, и JSON грузятся по строковому id.
  Это удобно для тестового проекта, но исключает адресную загрузку и увеличивает стартовый билд;
  при росте контента логичный шаг — Addressables/StreamingAssets.
- **Один сценарий на приложение**: `scenario.json` фиксированного имени, один `startNodeId`.
- **Состояние не сохраняется**: рестарт перезапускает сценарий с начала, прогресс не персистится.
- **UI полностью управляется состоянием**: `tcScenarioUI.RefreshUI()` вызывается каждый кадр,
  поэтому добавление новых элементов должно следовать той же схеме (вывод из `CurrentNode`).
- **Связи между компонентами — через инспектор** (`[SerializeField]`), без сервис-локатора и DI;
  при переименовании объектов сцены ссылки нужно переназначать.
- **PICO SDK подключён абсолютным путём** (`file:G:/Unity Projects/PICO Unity SDK`) — проект
  привязан к этой машине/раскладке диска.
