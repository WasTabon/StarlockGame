# Итерация 1: Базовая структура + Главное меню

## Что добавлено

### Скрипты

**Core:**
- `GameManager.cs` — синглтон, хранит режим игры (Levels/Endless), выбранный уровень, настройку звука
- `AudioManager.cs` — синглтон, воспроизведение звуков (пока заготовка без звуковых файлов)
- `SceneTransition.cs` — плавные fade переходы между сценами (DOTween)

**UI:**
- `AnimatedButton.cs` — кнопка с анимациями scale при hover/press (DOTween)
- `UIPanel.cs` — базовый класс для панелей с анимациями show/hide
- `MainMenuUI.cs` — контроллер главного меню
- `GameplayUI.cs` — контроллер UI геймплея (пока базовый)

**Editor:**
- `StarlockSceneSetup.cs` — автоматическое создание всех сцен и UI

---

## Установка

### 1. Убедитесь что DOTween установлен
Если нет — скачайте с Asset Store (бесплатная версия)

### 2. Скопируйте файлы
Скопируйте папку `Assets/StarlockGame` в ваш проект

### 3. Запустите Editor скрипт
В Unity меню: **Starlock → Setup All Scenes**

Это автоматически:
- Создаст структуру папок
- Создаст сцену MainMenu с полным UI
- Создаст сцену Gameplay с базовым UI
- Добавит сцены в Build Settings
- Прокинет все ссылки

### 4. Откройте MainMenu сцену
`Assets/StarlockGame/Scenes/MainMenu.unity`

### 5. Нажмите Play

---

## Как тестировать

1. **Главное меню:**
   - Кнопка LEVELS → открывает выбор уровней
   - Кнопка ENDLESS → переход в геймплей (Endless режим)
   - Кнопка SETTINGS → открывает настройки

2. **Выбор уровней:**
   - 10 кнопок уровней
   - Нажатие на любой → переход в геймплей с этим уровнем
   - Кнопка BACK → возврат в главное меню

3. **Настройки:**
   - Toggle для звука
   - Кнопка BACK → возврат в главное меню

4. **Геймплей:**
   - Отображает текущий режим и уровень
   - Кнопка MENU → возврат в главное меню

5. **Переходы:**
   - Все переходы между сценами с fade эффектом

---

## Ожидаемый результат

- ✅ Плавные fade переходы (0.3 сек)
- ✅ Анимации кнопок (scale on hover/press)
- ✅ Анимации панелей (fade + scale)
- ✅ GameManager сохраняется между сценами
- ✅ Корректная передача режима и уровня
- ✅ Вертикальная ориентация (1080x1920)

---

## Структура файлов

```
Assets/
  StarlockGame/
    Scripts/
      Core/
        GameManager.cs
        AudioManager.cs
        SceneTransition.cs
      UI/
        AnimatedButton.cs
        UIPanel.cs
        MainMenuUI.cs
        GameplayUI.cs
    Editor/
      StarlockSceneSetup.cs
    Scenes/
      MainMenu.unity (создаётся автоматически)
      Gameplay.unity (создаётся автоматически)
    Prefabs/
    Audio/
    Sprites/
```

---

## Примечания

- Округление углов кнопок — заглушка (нужен спрайт или shader для реальных rounded corners)
- Звуки пока не подключены (AudioManager готов, но AudioClip не назначены)
- В следующей итерации будет добавлен центральный круг и система вращения

---

## Следующая итерация

**Итерация 2:** Центральный круг + Система вращения
- CircleContainer — визуальный круг-контейнер
- RotationController — вращение всей системы
- RotationPreset (ScriptableObject) — пресеты скорости
