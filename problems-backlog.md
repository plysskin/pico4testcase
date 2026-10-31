# Problems backlog — Test Case PICO 4 (U3D)

Список известных проблем, дефектов и технического долга проекта.

> **Важно.** Это **журнал задач, а не план работ**. Ни один пункт отсюда **не исправляется
> без отдельного явного запроса пользователя**. Файл только фиксирует состояние: что сломано, где,
> почему и как проверять. При взятии задачи в работу — менять её `Статус` и добавлять дату.

Обозначения: **P0** — блокирует запуск/основной сценарий, **P1** — ломает часть функциональности,
**P2** — качество/удобство/техдолг. Статусы: `открыта`, `в работе`, `решена`, `отложена`.

Сводка:

| # | Проблема | Приоритет | Статус |
|---|---|---|---|
| 1 | Видео `.wmv` не воспроизводится на PICO 4 | P0 | открыта |
| 2 | Подпись APK: Custom Keystore без пути и пароля | P0 | открыта |
| 3 | В сборку попадала не та сцена | P0 | решена 30.10 |
| 4 | Кириллица в тексте финала не отображается (TMP) | P1 | открыта |
| 5 | Двойной вызов `PlayVideo` и пустой `videoId` для финалов | P1 | открыта |
| 6 | Расхождение package id / версии между Player Settings и Build Profile | P1 | открыта |
| 7 | `NullReferenceException` в инспекторе `PXR_Manager` | P2 | открыта |
| 8 | Android Graphics API — только OpenGLES3 | P2 | открыта |
| 9 | PICO SDK подключён абсолютным путём | P2 | открыта |
| 10 | Размер сборки ~950 МБ, EXR-панорамы 20–97 МБ | P2 | открыта |
| 11 | Нет обработки `VideoPlayer.errorReceived` | P2 | открыта |
| 12 | Мусор в проекте (пустые/неиспользуемые ассеты и скрипты) | P2 | открыта |
| 13 | Две камеры `Main Camera` в сцене | P2 | открыта |
| 14 | Нет сохранения прогресса | P2 | отложена |
| 15 | `versionCode` всегда 1 | P2 | открыта |

---

## 1. Видео `.wmv` не воспроизводится на PICO 4

- **Приоритет:** P0 · **Статус:** открыта · **Обнаружено:** 31.10.2026
- **Симптом:** на шлеме сцена и панорама работают, но экран `VideoScreen` пустой — видео не идёт.
  В редакторе Unity всё воспроизводится.
- **Где:** `Assets/Resources/video_1.wmv`, `video_2.wmv`, `video_3.wmv`;
  `Assets/Scripts/tcVideoController.cs` (`Resources.Load<VideoClip>` → `videoPlayer.Play()`).
- **Причина:** файлы в контейнере ASF/WMV (заголовок `30 26 B2 75 8E 66 CF 11 …`), кодек VC-1/WMV3 + WMA.
  Android-бэкенд `VideoPlayer` (ExoPlayer/MediaCodec) WMV не поддерживает — в Windows-редакторе
  клип играет через Media Foundation. В импорт-настройках клипов нет транскодирования
  (`Assets/Resources/video_1.wmv.meta` — только базовые поля, «Keep Original»), поэтому в APK
  уходит исходный `.wmv`.
- **Как проверить:** `adb logcat` при запуске видео (искать ошибки `VideoPlayer`/`MediaCodec`);
  либо добавить лог `videoPlayer.errorReceived`.
- **Что сделать (один из вариантов):**
  1. перекодировать в H.264/AAC `.mp4`, имена ассетов оставить `video_1..3`
     (`ffmpeg -i video_1.wmv -c:v libx264 -profile:v high -pix_fmt yuv420p -crf 20 -c:a aac -b:a 192k video_1.mp4`);
  2. включить в импорте клипа `Transcode` → Codec `H.264` (Unity перекодирует при сборке).
- **Ограничение контента:** видео должно остаться **side-by-side** — шейдер `SBSVideo.shader`
  делит кадр по глазам.

## 2. Подпись APK: Custom Keystore без пути и пароля

- **Приоритет:** P0 · **Статус:** открыта · **Обнаружено:** 24.10.2026 (по логам редактора)
- **Симптом:** сборка падает за ~2 секунды:
  `UnityException: Can not sign the application` / `Unable to sign the application; please provide passwords!`
- **Где:** `ProjectSettings/ProjectSettings.asset` — `AndroidKeystoreName` пустой (строка 271),
  `AndroidKeyaliasName: pico4dev`, при этом `androidUseCustomKeystore: 1` (строка 284).
- **Причина:** включён кастомный keystore, но не задан файл хранилища (и, соответственно, пароли).
- **Как проверить:** `Build` в Unity — ошибка в консоли сразу после старта сборки.
- **Что сделать:** либо указать `G:\Unity Projects\Keystore\user.keystore` + alias/пароли
  в Player Settings → Publishing Settings, либо снять галку Custom Keystore (debug-подпись).

## 3. В сборку попадала не та сцена

- **Приоритет:** P0 · **Статус:** решена 30.10.2026
- **Симптом:** на шлеме открывалась пустая сцена с кубом, скрипты не выполнялись.
- **Где:** `ProjectSettings/EditorBuildSettings.asset`.
- **Причина:** в списке сцен был шаблонный `Assets/Scenes/SampleScene.unity` (0 ссылок на игровые
  скрипты), а рабочая `Assets/Scenes/SampleScene 1.unity` (10 ссылок) в сборку не входила.
- **Что сделано:** рабочая сцена добавлена в список, шаблонная отключена.
- **Регрессионный чек:** перед сборкой убедиться, что `SampleScene 1` стоит индексом 0.

## 4. Кириллица в тексте финала не отображается (TMP)

- **Приоритет:** P1 · **Статус:** открыта
- **Симптом (ожидаемый):** вместо «Вы достигли финала 1» — пустые квадраты/ничего.
- **Где:** `Assets/Resources/scenario.json` (`endingText`), TMP-компонент `Ending Text` в сцене;
  шрифт — `Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset`.
- **Причина:** у шрифта `m_AtlasPopulationMode: 0` (Static), в атласе 250 глифов и **0 символов
  в диапазоне кириллицы (U+0400–U+04FF)**.
- **Как проверить:** запустить финал на устройстве/в Play Mode и посмотреть на `Ending Text`.
- **Что сделать:** добавить кириллицу в атлас (Font Asset → Update Atlas / Character Set →
  «Cyrillic»), либо подключить TTF с кириллицей (например, Roboto/Liberation с нужным диапазоном),
  либо задать fallback-шрифт с кириллицей.

## 5. Двойной вызов `PlayVideo` и пустой `videoId` для финалов

- **Приоритет:** P1 · **Статус:** открыта
- **Где:** `Assets/Scripts/ScenarioManager.cs`, метод `EnterNode` (строки 137–143).
- **Симптом:** видео стартует дважды (`Stop()` → `Play()` подряд), возможен рывок/перезапуск;
  для узлов `Ending` вызывается `PlayVideo("")` → warning «Не задан videoId» и лишний `Stop()`.
- **Причина:** вызов продублирован — внутри `if (NodeKind == Video)` и сразу после него.
- **Что сделать:** оставить один вызов внутри ветки `Video`; `SetPanorama` вызывать всегда
  (он и так вне условия — это корректно).

## 6. Расхождение package id / версии между Player Settings и Build Profile

- **Приоритет:** P1 · **Статус:** открыта
- **Где:** `ProjectSettings/ProjectSettings.asset` — `Android: com.ilyalukin.VRScenarios`,
  `bundleVersion: 0.9.0`; `Assets/Settings/Build Profiles/PICO 4.asset` —
  `com.ailab.pico4test`, `bundleVersion: 0.1.0`.
- **Симптом:** на шлеме оказывается приложение с другим именем/версией, чем ожидается;
  новая сборка ставится как отдельное приложение, старая иконка продолжает работать.
- **Что сделать:** определиться с одним package id и версией; синхронизировать Player Settings
  и Build Profile, удалить с устройства лишние пакеты.

## 7. `NullReferenceException` в инспекторе `PXR_Manager`

- **Приоритет:** P2 · **Статус:** открыта
- **Где:** `G:\Unity Projects\PICO Unity SDK\Editor\PXR_ManagerEditor.cs:280`, при выделении
  объекта с `PXR_Manager` в редакторе.
- **Причина:** несовместимость PICO Integration SDK 3.4.0 с Unity 6000.0.62f1 на уровне
  редакторского инспектора (сборку и рантайм не ломает).
- **Что сделать:** дождаться версии SDK с поддержкой Unity 6 либо обойти (не открывать инспектор,
  задавать параметры через `Assets/Resources/PICOProjectSetting.asset`).

## 8. Android Graphics API — только OpenGLES3

- **Приоритет:** P2 · **Статус:** открыта
- **Где:** `ProjectSettings/ProjectSettings.asset` — `m_BuildTargetGraphicsAPIs` для `AndroidPlayer`:
  `m_APIs: 0b000000` (только GLES3), `m_Automatic: 0`.
- **Комментарий:** PICO для Unity 6 рекомендует Vulkan; GLES3 работает, но при артефактах
  рендера/производительности первым делом проверить Vulkan.
- **Что сделать:** при необходимости включить Vulkan (и учесть требование PICO SDK
  «Optimize Buffer Discards» для Vulkan).

## 9. PICO SDK подключён абсолютным путём

- **Приоритет:** P2 · **Статус:** открыта
- **Где:** `Packages/manifest.json` — `"com.unity.xr.picoxr": "file:G:/Unity Projects/PICO Unity SDK"`.
- **Симптом:** при переносе проекта на другую машину/диск пакет не резолвится, проект не собирается.
- **Что сделать:** вендорить SDK внутрь проекта (`Packages/`) или использовать git/tarball-ссылку.

## 10. Размер сборки ~950 МБ, EXR-панорамы 20–97 МБ

- **Приоритет:** P2 · **Статус:** открыта
- **Где:** `Assets/Resources/panorama_*.exr` (5 файлов, ~313 МБ исходников), итоговый билд 950 МБ.
- **Риски:** долгий старт (на устройстве первый кадр через ~40 с), давление на память, большой APK.
- **Что сделать:** сжать панорамы (JPEG/BC7-кубмап), рассмотреть StreamingAssets/Addressables,
  включить `Compress Assets on Build` осознанно, проверить `GraphicsSettings` по текстурам.

## 11. Нет обработки `VideoPlayer.errorReceived`

- **Приоритет:** P2 · **Статус:** открыта
- **Где:** `Assets/Scripts/tcVideoController.cs`.
- **Симптом:** ошибки воспроизведения (кодек, отсутствующий клип, неподдерживаемый формат)
  не попадают в консоль — диагностика «молчит».
- **Что сделать:** подписаться на `errorReceived` и логировать текст ошибки.

## 12. Мусор в проекте

- **Приоритет:** P2 · **Статус:** открыта
- **Состав:**
  - `Assets/Scripts/HandButtonTouch.cs` — пустой файл (0 байт);
  - `Assets/Scripts/ScenarioNode.cs` — ScriptableObject-модель узла, в рантайме не используется
    (рабочая модель — `ScenarioNodeData`);
  - `Assets/RenderTexture/VideoScreenMaterial.mat`, `Assets/Materials/PanoramaMaterial.mat`,
    `Assets/Shaders/PanoramaMaterial.mat` — на сцену не ссылаются;
  - `Assets/Scenes/SampleScene.unity` — шаблонная сцена, в сборке не участвует;
  - объект `Cube` в сцене — демо-заглушка.
- **Что сделать:** удалить лишнее либо пометить как архив (`Assets/_Legacy/`).

## 13. Две камеры `Main Camera` в сцене

- **Приоритет:** P2 · **Статус:** открыта
- **Где:** `Assets/Scenes/SampleScene 1.unity` — вторая `Main Camera` (tag `Camera0`) выключена
  (`m_IsActive: 0`, `Camera.m_Enabled: 0`).
- **Риск:** валидация PICO SDK требует ровно одну `MainCamera` и один `AudioListener`;
  случайное включение второй камеры ломает стерео-вывод.
- **Что сделать:** удалить выключенный объект.

## 14. Нет сохранения прогресса

- **Приоритет:** P2 · **Статус:** отложена (по дизайну)
- **Где:** `Assets/Scripts/ScenarioManager.cs`.
- **Комментарий:** при перезапуске приложения сценарий начинается с `startNodeId`;
  состояние не персистится. Если потребуется — отдельная задача (PlayerPrefs/JSON в persistentDataPath).

## 15. `versionCode` всегда 1

- **Приоритет:** P2 · **Статус:** открыта
- **Где:** `ProjectSettings/ProjectSettings.asset` — `AndroidBundleVersionCode: 1`.
- **Риск:** обновление поверх установленной сборки в сторе/через менеджер требует роста versionCode.
- **Что сделать:** поднимать `versionCode` на каждый релизный билд (или автоматизировать в CI).
