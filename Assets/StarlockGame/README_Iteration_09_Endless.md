# Итерация 9: Endless режим

## Что добавлено

### Новые скрипты

**Gameplay:**
- `EndlessManager.cs` — управление бесконечным режимом (спавн, сложность)

**Core:**
- `HighscoreManager.cs` — сохранение рекордов (топ 5)

**Editor:**
- `StarlockSetup_09_Endless.cs` — настройка endless режима

### Обновлённые скрипты

- `GameplayController.cs` — поддержка endless режима (заменить!)
- `ShapeSpawner.cs` — добавлен SetSpawnOnStart (заменить!)
- `GameplayUI.cs` — время, рекорд для endless (заменить!)
- `GameOverPopup.cs` — показ рекорда и времени (заменить!)
- `VictoryPopup.cs` — фикс кнопок (заменить!)

---

## Установка

### 1. Скопируйте файлы

| Файл | Куда |
|------|------|
| `EndlessManager.cs` | `Assets/StarlockGame/Scripts/Gameplay/` |
| `HighscoreManager.cs` | `Assets/StarlockGame/Scripts/Core/` |
| `GameplayController.cs` | `Assets/StarlockGame/Scripts/` (заменить!) |
| `ShapeSpawner.cs` | `Assets/StarlockGame/Scripts/` (заменить!) |
| `GameplayUI.cs` | `Assets/StarlockGame/Scripts/` (заменить!) |
| `GameOverPopup.cs` | `Assets/StarlockGame/Scripts/` (заменить!) |
| `VictoryPopup.cs` | `Assets/StarlockGame/Scripts/` (заменить!) |
| `StarlockSetup_09_Endless.cs` | `Assets/StarlockGame/Editor/` |

### 2. Откройте Gameplay сцену

### 3. Запустите Editor скрипт

**Starlock → Setup Endless Mode (Iteration 9)**

### 4. Сохраните сцену

---

## Как работает Endless режим

### Начало:
1. 3 пары фигур спавнятся сразу
2. Таймер начинает отсчёт

### Игровой процесс:
1. Каждые N секунд спавнится новая пара
2. Каждые 10 секунд сложность увеличивается:
   - Интервал спавна уменьшается
   - Скорость вращения увеличивается
   - Иногда меняется направление

### Конец игры:
1. Круг заполнен = Game Over
2. Счёт сохраняется в таблицу рекордов
3. Если новый рекорд — показывается "NEW RECORD!"

---

## Настройки EndlessManager

| Параметр | Описание | По умолчанию |
|----------|----------|--------------|
| Initial Spawn Interval | Начальный интервал спавна | 3 сек |
| Min Spawn Interval | Минимальный интервал | 1 сек |
| Spawn Interval Decrease Rate | Уменьшение интервала | 0.05 сек |
| Max Shapes On Screen | Максимум фигур | 20 |
| Initial Rotation Speed | Начальная скорость | 30° |
| Max Rotation Speed | Максимальная скорость | 100° |
| Rotation Speed Increase Rate | Увеличение скорости | 1° |
| Difficulty Increase Interval | Интервал роста сложности | 10 сек |

---

## Таблица рекордов

- Хранит топ 5 результатов
- Каждый результат: очки + время
- Сохраняется в PlayerPrefs

### PlayerPrefs ключи:

| Ключ | Описание |
|------|----------|
| `EndlessHighscore` | Лучший счёт |
| `EndlessHighscores_N_score` | Счёт N-го места |
| `EndlessHighscores_N_time` | Время N-го места |

---

## UI для Endless режима

### HUD:
- Счёт (как обычно)
- Время (MM:SS)
- Лучший счёт

### Game Over попап:
- "GAME OVER" или "NEW RECORD!"
- Текущий счёт
- Лучший счёт
- Время игры

---

## Debug команды

| Команда | Описание |
|---------|----------|
| **Starlock → Debug: Reset Endless Highscores** | Сбросить все рекорды |

---

## Как тестировать

1. Запусти MainMenu сцену
2. Нажми "Endless"
3. Играй пока не проиграешь
4. Проверь что счёт сохранился
5. Сыграй снова — проверь что рекорд отображается

---

## Ожидаемый результат

- ✅ Фигуры спавнятся бесконечно
- ✅ Сложность растёт со временем
- ✅ Скорость вращения увеличивается
- ✅ Направление иногда меняется
- ✅ Интервал спавна уменьшается
- ✅ При game over показывается счёт и время
- ✅ Новый рекорд отмечается
- ✅ Рекорды сохраняются между сессиями
- ✅ Таймер отображается в HUD

---

## Следующая итерация

**Итерация 10:** Финальная полировка
- Туториал
- UI polish
- Баланс
- Звуки для endless
