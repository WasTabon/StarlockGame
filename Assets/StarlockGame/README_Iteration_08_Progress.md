# Итерация 8: Прогрессия уровней

## Что добавлено

### Новые скрипты

**Core:**
- `ProgressManager.cs` — сохранение/загрузка прогресса (PlayerPrefs)
- `LevelConfig.cs` — конфигурация уровней (пары, скорость, направление)

**UI:**
- `LevelButton.cs` — кнопка уровня с визуалом прогресса (замок, галочка, звёзды)

**Editor:**
- `StarlockSetup_08_Progress.cs` — настройка прогрессии, debug-команды

### Обновлённые скрипты

- `GameManager.cs` — интеграция с LevelConfig и ProgressManager
- `MainMenuUI.cs` — поддержка LevelButton, кнопка сброса прогресса
- `GameplayController.cs` — использование LevelConfig для настройки уровня

---

## Установка

### 1. Скопируйте файлы

| Файл | Куда |
|------|------|
| `ProgressManager.cs` | `Assets/StarlockGame/Scripts/Core/` |
| `LevelConfig.cs` | `Assets/StarlockGame/Scripts/Core/` |
| `LevelButton.cs` | `Assets/StarlockGame/Scripts/UI/` |
| `GameManager.cs` | `Assets/StarlockGame/Scripts/` (заменить!) |
| `MainMenuUI.cs` | `Assets/StarlockGame/Scripts/` (заменить!) |
| `GameplayController.cs` | `Assets/StarlockGame/Scripts/` (заменить!) |
| `StarlockSetup_08_Progress.cs` | `Assets/StarlockGame/Editor/` |

### 2. Для MainMenu сцены

1. Откройте MainMenu сцену
2. **Starlock → Setup Progress System (Iteration 8)**
3. **Starlock → Setup Level Buttons (MainMenu)**
4. Сохраните сцену

### 3. Для Gameplay сцены

1. Откройте Gameplay сцену
2. **Starlock → Setup Progress System (Iteration 8)**
3. Сохраните сцену

---

## Конфигурация уровней

| Уровень | Пар | Скорость | Направление | Max внутри |
|---------|-----|----------|-------------|------------|
| 1 | 3 | 20° | → | 10 |
| 2 | 4 | 25° | → | 10 |
| 3 | 5 | 30° | → | 10 |
| 4 | 5 | 35° | ← | 9 |
| 5 | 6 | 40° | → | 9 |
| 6 | 6 | 45° | ← | 8 |
| 7 | 7 | 50° | → | 8 |
| 8 | 7 | 55° | ← | 7 |
| 9 | 8 | 60° | → | 7 |
| 10 | 10 | 70° | ← | 6 |

---

## Система звёзд

| Очки | Звёзды |
|------|--------|
| < базовый счёт | ⭐ |
| ≥ базовый счёт | ⭐⭐ |
| ≥ 1.5x базового | ⭐⭐⭐ |

Базовый счёт = пары × 100

---

## LevelButton визуал

Для полной настройки каждой кнопки уровня можно добавить:

```
LevelButton (Button + LevelButton компонент)
├── LevelNumberText (TextMeshProUGUI) — "1", "2", "3"...
├── LockedOverlay (Image) — затемнение + замок
├── CompletedCheckmark (Image) — галочка
└── StarsContainer (GameObject)
    ├── Star1 (Image)
    ├── Star2 (Image)
    └── Star3 (Image)
```

Минимально достаточно только номера уровня — остальное опционально.

---

## Debug команды (Editor)

| Команда | Описание |
|---------|----------|
| **Starlock → Debug: Reset All Progress** | Сбросить весь прогресс |
| **Starlock → Debug: Unlock All Levels** | Разблокировать все уровни |

---

## PlayerPrefs ключи

| Ключ | Описание |
|------|----------|
| `MaxUnlockedLevel` | Максимальный разблокированный уровень |
| `LevelCompleted_N` | Уровень N пройден (0/1) |
| `LevelStars_N` | Звёзды за уровень N (0-3) |
| `LevelHighscore_N` | Лучший счёт за уровень N |

---

## Как работает

### При запуске:
1. `ProgressManager` загружает прогресс из PlayerPrefs
2. `MainMenuUI` получает `LevelButton` компоненты
3. Каждый `LevelButton.UpdateVisuals()` показывает состояние

### При выборе уровня:
1. Проверка `IsLevelUnlocked()`
2. `GameManager.StartLevelMode()` загружает `LevelConfig`
3. `GameplayController` применяет настройки уровня
4. `ShapeSpawner` спавнит нужное количество пар

### При победе:
1. `GameManager.OnLevelCompleted()` считает звёзды
2. `ProgressManager.CompleteLevel()` сохраняет прогресс
3. Следующий уровень разблокируется

---

## Ожидаемый результат

- ✅ Уровень 1 разблокирован по умолчанию
- ✅ Остальные уровни заблокированы
- ✅ При победе — следующий уровень разблокируется
- ✅ Пройденные уровни отмечены (галочка / цвет)
- ✅ Звёзды сохраняются за лучший результат
- ✅ Разное количество фигур на каждом уровне
- ✅ Скорость вращения увеличивается
- ✅ На некоторых уровнях вращение в обратную сторону
- ✅ Прогресс сохраняется между сессиями

---

## Следующая итерация

**Итерация 9:** Endless режим
- Бесконечный спавн фигур
- Увеличение сложности со временем
- Таблица рекордов
