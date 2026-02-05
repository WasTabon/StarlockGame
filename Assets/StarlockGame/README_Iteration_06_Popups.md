# Итерация 6: UI победы/поражения, рестарт

## Что добавлено

### Новые скрипты

**UI:**
- `VictoryPopup.cs` — попап победы с кнопками Next Level, Restart, Menu
- `GameOverPopup.cs` — попап поражения с кнопками Try Again, Menu

**Editor:**
- `StarlockSetup_06_Popups.cs` — создание попапов в UI

### Обновлённые скрипты

- `InputManager.cs` — фикс двойного тапа (else if), добавлен SetInputEnabled()
- `GameplayUI.cs` — управление попапами
- `GameplayController.cs` — показ попапов, рестарт, переход между уровнями

---

## Установка

### 1. Скопируйте файлы

| Файл | Куда |
|------|------|
| `VictoryPopup.cs` | `Assets/StarlockGame/Scripts/UI/` |
| `GameOverPopup.cs` | `Assets/StarlockGame/Scripts/UI/` |
| `InputManager.cs` | `Assets/StarlockGame/Scripts/Gameplay/` (заменить!) |
| `GameplayUI.cs` | `Assets/StarlockGame/Scripts/UI/` (заменить!) |
| `GameplayController.cs` | `Assets/StarlockGame/Scripts/Gameplay/` (заменить!) |
| `StarlockSetup_06_Popups.cs` | `Assets/StarlockGame/Editor/` |

### 2. Откройте Gameplay сцену

### 3. Запустите Editor скрипт

**Starlock → Setup Popups (Iteration 6)**

### 4. Сохраните сцену

---

## Как работает

### Победа:
1. Все фигуры исчезли (снаружи и внутри)
2. Вращение останавливается
3. Ввод блокируется
4. Через 0.5 сек появляется VictoryPopup

### Поражение:
1. Круг заполнен (10 фигур внутри)
2. Вращение останавливается
3. Ввод блокируется
4. Через 0.5 сек появляется GameOverPopup

### Кнопки:
- **Next Level** — переход на следующий уровень (только в режиме Levels)
- **Restart** — перезагрузка текущей сцены
- **Menu** — возврат в главное меню

---

## Настройки в инспекторе

### GameplayController

| Параметр | Описание | По умолчанию |
|----------|----------|--------------|
| Max Level | Максимальный номер уровня | 10 |
| End Game Delay | Задержка перед показом попапа | 0.5 |

---

## Иерархия после настройки

```
UI_Canvas
├── Header
├── DebugPanel
├── GameAreaPlaceholder
├── VictoryPopup          (NEW, inactive)
│   ├── Dimmer
│   └── PanelBackground
│       ├── Title
│       ├── ScoreText
│       ├── NextLevelButton
│       ├── RestartButton
│       └── MenuButton
└── GameOverPopup         (NEW, inactive)
    ├── Dimmer
    └── PanelBackground
        ├── Title
        ├── ScoreText
        ├── RestartButton
        └── MenuButton
```

---

## Как тестировать

### Тест победы:
1. Play
2. Отправляй одинаковые фигуры внутрь (они исчезнут)
3. Когда все фигуры исчезнут — появится VictoryPopup
4. Нажми "Restart" — сцена перезагрузится

### Тест поражения:
1. Play
2. Отправляй разные фигуры внутрь (не совпадающие)
3. Когда внутри будет 10 фигур — появится GameOverPopup
4. Нажми "Try Again" — сцена перезагрузится

### Тест навигации:
1. Из MainMenu выбери уровень
2. Пройди его (победа)
3. Нажми "Next Level" — загрузится следующий уровень
4. Нажми "Menu" — вернёшься в главное меню

---

## Ожидаемый результат

- ✅ Победа — появляется VictoryPopup с анимацией
- ✅ Поражение — появляется GameOverPopup с анимацией
- ✅ Во время показа попапа ввод заблокирован
- ✅ Restart перезагружает сцену
- ✅ Next Level переходит на следующий уровень
- ✅ Menu возвращает в главное меню
- ✅ Фикс двойного тапа (фигуры не дублируются)
- ✅ Затемнение фона за попапом

---

## Следующая итерация

**Итерация 7:** Полировка — эффекты, звуки, juice
- Эффекты частиц при матче
- Звуки: тап, матч, победа, поражение
- Screen shake при поражении
- Анимация счёта
