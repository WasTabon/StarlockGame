# Итерация 10: Исправления

## Что было исправлено

1. **Туториал** — теперь в MainMenu (на английском)
2. **Кнопки PausePopup** — теперь работают
3. **Endless Game Over** — попап теперь появляется
4. **EndlessManager** — рандомный спавн (не парный)

---

## Как применить если уже запускал Setup 10

### Шаг 1: Заменить скрипты

Заменить эти файлы (удалить старые, скопировать новые):

| Файл | Куда |
|------|------|
| `TutorialManager.cs` | `Assets/StarlockGame/Scripts/UI/` |
| `MainMenuUI.cs` | `Assets/StarlockGame/Scripts/UI/` |
| `PausePopup.cs` | `Assets/StarlockGame/Scripts/UI/` |
| `GameplayUI.cs` | `Assets/StarlockGame/Scripts/UI/` |
| `GameplayController.cs` | `Assets/StarlockGame/Scripts/Gameplay/` |
| `EndlessManager.cs` | `Assets/StarlockGame/Scripts/Gameplay/` |
| `StarlockSetup_10_Final.cs` | `Assets/StarlockGame/Editor/` |

### Шаг 2: Gameplay сцена

1. Открой **Gameplay** сцену
2. **Удали** TutorialManager и TutorialPanel (если есть)
3. Запусти **Starlock → Setup Gameplay Scene (Iteration 10)**
4. В GameplayUI назначь **pauseButton** (кнопка паузы в HUD)
5. Сохрани сцену

### Шаг 3: MainMenu сцена

1. Открой **MainMenu** сцену
2. Запусти **Starlock → Setup MainMenu Tutorial (Iteration 10)**
3. Сохрани сцену

### Шаг 4: Тест

1. **Starlock → Debug: Reset Tutorial**
2. Запусти MainMenu сцену
3. Туториал должен появиться
4. Пройди туториал → зайди в Endless
5. Заполни круг → должен появиться Game Over попап
6. Нажми ESC или кнопку паузы → должна быть пауза

---

## Кнопка паузы в HUD

Если у тебя нет кнопки паузы в UI:

1. В Gameplay сцене создай кнопку (например в Header)
2. Назначь её в `GameplayUI → Pause Button`

Или используй только ESC для паузы.

---

## Структура после настройки

### MainMenu сцена:
```
UI_Canvas
├── MainPanel
├── LevelSelectPanel
├── SettingsPanel
├── TutorialPanel       ← NEW
│   ├── Dimmer
│   └── Content
│       ├── TutorialText
│       ├── SkipButton
│       └── NextButton
└── TutorialManager     ← NEW (GameObject)
```

### Gameplay сцена:
```
UI_Canvas
├── Header
├── VictoryPopup
├── GameOverPopup
└── PausePopup          ← NEW
    ├── Dimmer
    └── PanelBackground
        ├── Title
        ├── ResumeButton
        ├── RestartButton
        └── MenuButton
```

---

## Частые проблемы

### Туториал не появляется
- Проверь что TutorialManager есть в MainMenu сцене
- Проверь что tutorialManager назначен в MainMenuUI
- **Starlock → Debug: Reset Tutorial**

### Кнопки в паузе не работают
- Проверь что pausePopup назначен в GameplayUI
- Проверь что кнопки назначены в PausePopup

### Game Over попап не появляется в Endless
- Проверь что gameOverPopup назначен в GameplayUI
- Проверь что CircleContainer имеет правильный maxShapes

### Кнопка паузы в HUD не работает
- Проверь что pauseButton назначен в GameplayUI
