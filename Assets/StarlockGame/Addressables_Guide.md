# Addressables + Cloudflare R2 — Полное руководство для Starlock

## Что такое что (простыми словами)

**Addressables** — это система Unity, которая позволяет хранить ассеты (спрайты, звуки, префабы) НЕ внутри билда игры, а скачивать их с сервера при первом запуске. Это уменьшает размер APK/IPA и позволяет обновлять контент без перезагрузки приложения в стор.

**Cloudflare R2** — это облачное хранилище файлов (как Google Drive, но для серверов). Туда загружаются бандлы (пакеты ассетов), и игра скачивает их оттуда.

**Бакет (Bucket)** — это просто "папка" в облаке. Один бакет = одно хранилище для одного проекта.

---

## ЧАСТЬ 1: Настройка Cloudflare R2

### Шаг 1.1 — Создать бакет

1. Зайди в [dash.cloudflare.com](https://dash.cloudflare.com)
2. В левом меню найди **R2 Object Storage** → нажми
3. Нажми **Create bucket**
4. Имя бакета: `starlock-assets` (или любое другое, латиницей, без пробелов)
5. Регион: выбери ближайший к твоим пользователям (или **Automatic**)
6. Нажми **Create bucket**

### Шаг 1.2 — Включить публичный доступ

Addressables должны скачивать файлы по HTTP, поэтому бакет нужно сделать публичным:

1. Зайди в созданный бакет `starlock-assets`
2. Перейди на вкладку **Settings**
3. Найди секцию **Public access** (или **Custom Domains**)
4. **Вариант А — R2.dev subdomain (проще):**
   - Включи "R2.dev subdomain" → получишь URL вида:
   - `https://pub-XXXXXXXXX.r2.dev`
   - Это и будет твой базовый URL для Addressables
5. **Вариант Б — Custom domain (если есть свой домен):**
   - Добавь свой домен, например `assets.starlock.com`
   - Следуй инструкциям по DNS

> **Запомни свой публичный URL** — он понадобится в Unity. Пример: `https://pub-abc123def456.r2.dev`

### Шаг 1.3 — Структура папок в бакете

После того как Unity соберёт бандлы, ты загрузишь их в бакет. Структура будет такая:

```
starlock-assets/          (бакет)
  └── StandaloneWindows64/    (или Android/ или iOS/ — платформа)
        ├── catalog_xxxx.hash
        ├── catalog_xxxx.json
        ├── defaultlocalgroup_xxxx.bundle
        └── ...другие бандлы
```

Загрузка файлов в бакет делается через:
- Веб-интерфейс dash.cloudflare.com (перетащить файлы)
- Или через CLI утилиту `wrangler` (подробнее ниже)

---

## ЧАСТЬ 2: Настройка Unity Addressables

### Шаг 2.1 — Установить пакет Addressables

1. В Unity: **Window → Package Manager**
2. Нажми **+** → **Add package by name**
3. Введи: `com.unity.addressables`
4. Нажми **Add** → дождись установки

### Шаг 2.2 — Инициализировать Addressables

1. **Window → Asset Management → Addressables → Groups**
2. Появится окно, нажми **Create Addressables Settings**
3. Появится папка `Assets/AddressableAssetsData/`

### Шаг 2.3 — Создать папки для ассетов

Создай в проекте такую структуру:

```
Assets/
  RemoteAssets/
    Shapes/
      Circle.png
      Square.png
      Triangle.png
      Diamond.png
      Hexagon.png
    Audio/
      SFX/
        tap.wav (или .ogg/.mp3)
        match.wav
        victory.wav
        gameover.wav
        shapeenter.wav
      UI/
        button_click.wav
        button_hover.wav
        panel_open.wav
        panel_close.wav
```

> **Важно:** Сейчас спрайты генерируются кодом. Тебе нужно нарисовать/найти реальные PNG файлы для каждой фигуры и положить их в `Assets/RemoteAssets/Shapes/`. То же с аудио — сейчас звуки генерируются процедурно, нужны реальные аудиофайлы.

### Шаг 2.4 — Пометить ассеты как Addressable

1. Выдели все файлы в `Assets/RemoteAssets/Shapes/` в Project окне
2. В Inspector поставь галку **Addressable**
3. В поле Address задай понятные имена:
   - `Circle.png` → адрес `Shapes/Circle`
   - `Square.png` → адрес `Shapes/Square`
   - `Triangle.png` → адрес `Shapes/Triangle`
   - `Diamond.png` → адрес `Shapes/Diamond`
   - `Hexagon.png` → адрес `Shapes/Hexagon`
4. То же для аудио:
   - `tap.wav` → адрес `Audio/SFX/tap`
   - `match.wav` → адрес `Audio/SFX/match`
   - `victory.wav` → адрес `Audio/SFX/victory`
   - `gameover.wav` → адрес `Audio/SFX/gameover`
   - `shapeenter.wav` → адрес `Audio/SFX/shapeenter`
   - `button_click.wav` → адрес `Audio/UI/button_click`
   - `button_hover.wav` → адрес `Audio/UI/button_hover`
   - `panel_open.wav` → адрес `Audio/UI/panel_open`
   - `panel_close.wav` → адрес `Audio/UI/panel_close`

### Шаг 2.5 — Настроить группу и лейблы

1. **Window → Asset Management → Addressables → Groups**
2. Создай новую группу: правый клик → **Create New Group → Packed Assets**
3. Назови её `RemoteAssets`
4. Перетащи все ассеты из Default Local Group в `RemoteAssets`
5. Выдели группу `RemoteAssets` → в Inspector настрой:
   - **Build Path:** `RemoteBuildPath`
   - **Load Path:** `RemoteLoadPath`
6. Выдели все ассеты в группе → в Inspector добавь лейбл: `GameContent`

### Шаг 2.6 — Настроить профиль (Remote URL)

1. **Window → Asset Management → Addressables → Profiles**
2. В профиле Default:
   - `RemoteLoadPath` = `https://pub-XXXXXXXXX.r2.dev/[BuildTarget]`
     - Замени `pub-XXXXXXXXX.r2.dev` на свой URL из Шага 1.2
     - `[BuildTarget]` — Unity автоматически подставит платформу (Android, iOS, etc.)
   - `RemoteBuildPath` = `ServerData/[BuildTarget]`
     - Это локальная папка куда Unity соберёт бандлы перед загрузкой на сервер

### Шаг 2.7 — Настроить Addressable Asset Settings

1. Выдели `Assets/AddressableAssetsData/AddressableAssetSettings.asset`
2. В Inspector:
   - **Build Remote Catalog:** ✅ включить
   - **Build & Load Paths:** для каталога тоже `RemoteBuildPath` / `RemoteLoadPath`

---

## ЧАСТЬ 3: Скрипты Unity

В архиве `AddressablesScripts.zip` ты найдёшь все нужные скрипты. Вот что они делают:

### Новые скрипты (положить в `Assets/Scripts/`):

| Скрипт | Описание |
|--------|----------|
| `ContentLoader.cs` | Проверяет интернет, скачивает бандлы, показывает прогресс |
| `AddressableAssetService.cs` | Загружает конкретные ассеты (спрайты, аудио) по адресу |
| `BootstrapManager.cs` | Управляет сценой загрузки — запускает ContentLoader, потом переходит в меню |

### Изменённые скрипты (заменить существующие):

| Скрипт | Что изменилось |
|--------|---------------|
| `ShapeFactory.cs` | Вместо `Resources.Load` загружает спрайты через `AddressableAssetService` |
| `GameAudio.cs` | Загружает звуковые клипы через `AddressableAssetService` |
| `AudioManager.cs` | Загружает UI-звуки через `AddressableAssetService` |
| `GameManager.cs` | Добавлен флаг `ContentReady` — игра не стартует пока контент не скачан |

---

## ЧАСТЬ 4: Настройка сцены Bootstrap

### Шаг 4.1 — Создать сцену

1. **File → New Scene** → сохрани как `Bootstrap`
2. В Build Settings (**File → Build Settings**):
   - Добавь сцену `Bootstrap` → поставь её **первой** (индекс 0)
   - `MainMenu` — индекс 1
   - `Gameplay` — индекс 2

### Шаг 4.2 — Создать UI загрузки

В сцене `Bootstrap` создай:

1. **Canvas** (Screen Space - Overlay, Scale With Screen Size, 1080x1920)
2. Внутри Canvas:
   - **Panel** (фон) — назови `BackgroundPanel`, залей цветом фона игры
   - **Text (TMP)** — назови `StatusText`, по центру, текст "Загрузка..."
   - **Slider** — назови `ProgressSlider`, по центру ниже текста
     - Убери Interactable
     - Min Value = 0, Max Value = 1
   - **Text (TMP)** — назови `PercentText`, рядом со слайдером, текст "0%"
   - **GameObject** (пустой) — назови `NoInternetPanel`, добавь Image компонент
     - Внутри: Text (TMP) "Нет подключения к интернету"
     - Внутри: Button "Повторить" — назови `RetryButton`
     - Скрой этот объект (выключи GameObject)

### Шаг 4.3 — Повесить скрипты

1. Создай пустой GameObject → назови `BootstrapManager`
2. Добавь компонент `BootstrapManager`
3. Присвой ссылки в Inspector:
   - `statusText` → StatusText
   - `progressSlider` → ProgressSlider
   - `percentText` → PercentText
   - `noInternetPanel` → NoInternetPanel
   - `retryButton` → RetryButton

---

## ЧАСТЬ 5: Сборка и загрузка на Cloudflare

### Шаг 5.1 — Собрать бандлы

1. **Window → Asset Management → Addressables → Groups**
2. **Build → New Build → Default Build Script**
3. Дождись окончания сборки
4. Файлы появятся в папке `ServerData/[BuildTarget]/` в корне проекта (рядом с Assets)

### Шаг 5.2 — Загрузить в Cloudflare R2

**Вариант А — Через веб-интерфейс:**
1. Зайди в dash.cloudflare.com → R2 → бакет `starlock-assets`
2. Создай папку с именем платформы (например `Android` или `StandaloneWindows64`)
3. Загрузи ВСЕ файлы из `ServerData/[BuildTarget]/` в эту папку

**Вариант Б — Через wrangler CLI:**
```bash
# Установить wrangler (нужен Node.js)
npm install -g wrangler

# Залогиниться
wrangler login

# Загрузить файлы
# Замени starlock-assets на имя твоего бакета
cd ServerData/Android/
for file in *; do
  wrangler r2 object put "starlock-assets/Android/$file" --file "$file"
done
```

### Шаг 5.3 — Проверить

Открой в браузере:
```
https://pub-XXXXXXXXX.r2.dev/Android/catalog_xxxx.json
```
Если видишь JSON — всё работает.

---

## ЧАСТЬ 6: При обновлении контента

Когда нужно обновить спрайты/звуки:

1. Замени файлы в `Assets/RemoteAssets/`
2. В Addressables Groups: **Build → Update a Previous Build**
3. Загрузи новые файлы из `ServerData/` в Cloudflare R2 (замени старые)
4. Игроки при следующем запуске автоматически скачают обновления

---

## Чек-лист

- [ ] Создан бакет в Cloudflare R2
- [ ] Включён публичный доступ, записан URL
- [ ] Установлен пакет Addressables в Unity
- [ ] Созданы реальные файлы спрайтов (PNG) для всех фигур
- [ ] Созданы/найдены аудиофайлы для всех звуков
- [ ] Все ассеты помечены как Addressable с правильными адресами
- [ ] Группа RemoteAssets настроена на RemoteBuildPath/RemoteLoadPath
- [ ] В профиле прописан URL Cloudflare
- [ ] Включён Build Remote Catalog
- [ ] Создана сцена Bootstrap с UI загрузки
- [ ] Скрипты из архива добавлены/заменены в проекте
- [ ] Bootstrap — первая сцена в Build Settings
- [ ] Бандлы собраны и загружены в R2
- [ ] Проверено в браузере что файлы доступны по URL
