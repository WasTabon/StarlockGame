# Итерация 2: Центральный круг + Система вращения

## Что добавлено

### Новые скрипты

**Data:**
- `RotationPreset.cs` — ScriptableObject для пресетов скорости вращения

**Gameplay:**
- `RotationController.cs` — вращает объект с заданной скоростью
- `CircleContainer.cs` — центральный круг, граница для фигур, счётчик
- `OuterZone.cs` — внешняя кольцевая зона
- `GameplayController.cs` — точка входа, инициализация геймплея

**Editor:**
- `StarlockGameplaySetup.cs` — дополняет Gameplay сцену (не пересоздаёт!)

### Исправления

- `UIPanel.cs` — добавлена ленивая инициализация (фикс NullReferenceException)

---

## Установка

### 1. Скопируйте новые файлы

Скопируйте всё содержимое `Assets/StarlockGame/` в ваш проект (с заменой файлов)

### 2. Откройте Gameplay сцену

`Assets/StarlockGame/Scenes/Gameplay.unity`

### 3. Запустите Editor скрипт

В Unity меню: **Starlock → Setup Gameplay Objects**

Это автоматически:
- Создаст 4 пресета вращения (Slow, Medium, Fast, Insane)
- Добавит RotationPivot с RotationController
- Добавит CircleContainer с визуалом и коллайдером
- Добавит OuterZone с внешним коллайдером
- Создаст спрайт круга
- Прокинет все ссылки в GameplayController
- НЕ тронет существующий UI

### 4. Сохраните сцену

Ctrl+S (или Cmd+S на Mac)

---

## Как тестировать

1. Откройте сцену **Gameplay**
2. Нажмите **Play**
3. Круг и внешняя зона вращаются вместе (скорость Medium по умолчанию)

### Проверка разных скоростей:

1. Остановите Play mode
2. Выберите объект **RotationPivot** в Hierarchy
3. В компоненте **RotationController** → поле **Current Preset**
4. Назначьте другой пресет из `Assets/StarlockGame/Data/`
5. Нажмите Play — скорость изменится

### Проверка через MainMenu:

1. Откройте сцену **MainMenu**
2. Play
3. Выберите уровень (1-3 = Slow, 4-6 = Medium, 7-9 = Fast, 10 = Insane)
4. В Gameplay скорость соответствует выбранному уровню

---

## Ожидаемый результат

- ✅ Центральный круг с контуром (голубой)
- ✅ Круг и внешняя зона вращаются как единое целое
- ✅ EdgeCollider2D для границы круга
- ✅ EdgeCollider2D для внешней границы
- ✅ Gizmos в Scene view показывают зоны
- ✅ 4 пресета скорости
- ✅ Скорость зависит от уровня

---

## Структура файлов (новое)

```
Assets/
  StarlockGame/
    Scripts/
      Data/
        RotationPreset.cs          (NEW)
      Gameplay/
        RotationController.cs      (NEW)
        CircleContainer.cs         (NEW)
        OuterZone.cs               (NEW)
        GameplayController.cs      (NEW)
      UI/
        UIPanel.cs                 (UPDATED - lazy init)
    Editor/
      StarlockGameplaySetup.cs     (NEW)
    Data/
      RotationPreset_Slow.asset    (создаётся автоматически)
      RotationPreset_Medium.asset  (создаётся автоматически)
      RotationPreset_Fast.asset    (создаётся автоматически)
      RotationPreset_Insane.asset  (создаётся автоматически)
    Sprites/
      Circle.png                   (создаётся автоматически)
```

---

## Иерархия Gameplay сцены

```
Gameplay Scene
├── Main Camera
├── EventSystem
├── GameplayController
├── RotationPivot              (вращается)
│   ├── CircleContainer
│   │   ├── CircleVisual       (SpriteRenderer)
│   │   └── CircleBoundary     (EdgeCollider2D)
│   └── OuterZone
│       └── OuterBoundary      (EdgeCollider2D)
├── UI_Canvas
│   └── ... (из итерации 1)
```

---

## Пресеты скорости

| Пресет | Скорость (°/сек) | Уровни |
|--------|------------------|--------|
| Slow   | 30               | 1-3    |
| Medium | 60               | 4-6    |
| Fast   | 100              | 7-9    |
| Insane | 150              | 10+    |

---

## Следующая итерация

**Итерация 3:** Фигуры — данные и визуал
- ShapeType enum (Circle, Square, Triangle, Diamond, Hexagon)
- ShapeColor enum (5 цветов)
- Shape.cs — компонент фигуры
- Программная генерация спрайтов
- ShapeFactory — создание фигур
