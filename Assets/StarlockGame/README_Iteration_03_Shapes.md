# Итерация 3: Фигуры — данные и визуал

## Что добавлено

### Новые скрипты

**Data (enums):**
- `ShapeType.cs` — enum: Circle, Square, Triangle, Diamond, Hexagon
- `ShapeColor.cs` — enum: Red, Blue, Green, Yellow, Purple + extension ToColor()
- `ShapeState.cs` — enum: Outside, MovingInside, Inside, Matched

**Gameplay:**
- `Shape.cs` — компонент фигуры (тип, цвет, состояние, физика, анимации)
- `ShapeVisualGenerator.cs` — программная генерация текстур для всех форм
- `ShapeFactory.cs` — singleton фабрика для создания фигур

**Editor:**
- `StarlockSetup_03_Shapes.cs` — генерация спрайтов, создание префаба, добавление фабрики

---

## Установка

### 1. Скопируйте новые файлы

Скопируйте содержимое `Assets/StarlockGame/` в ваш проект

### 2. Откройте Gameplay сцену

### 3. Запустите Editor скрипт

**Starlock → Setup Shapes**

Это автоматически:
- Сгенерирует 5 спрайтов форм
- Создаст префаб Shape
- Добавит ShapeFactory на сцену
- Создаст ShapesParent под RotationPivot

### 4. Сохраните сцену

---

## Структура файлов (новое)

```
Assets/
  StarlockGame/
    Scripts/
      Data/
        ShapeType.cs         (NEW)
        ShapeColor.cs        (NEW)
        ShapeState.cs        (NEW)
      Gameplay/
        Shape.cs             (NEW)
        ShapeVisualGenerator.cs (NEW)
        ShapeFactory.cs      (NEW)
    Editor/
      StarlockSetup_03_Shapes.cs (NEW)
    Sprites/
      Shapes/
        Circle.png           (генерируется)
        Square.png           (генерируется)
        Triangle.png         (генерируется)
        Diamond.png          (генерируется)
        Hexagon.png          (генерируется)
    Prefabs/
      Shape.prefab           (создаётся)
  Resources/
    Shapes/
      Circle.png             (копии для Resources.Load)
      Square.png
      Triangle.png
      Diamond.png
      Hexagon.png
```

---

## Цвета фигур

| ShapeColor | HEX | RGB |
|------------|-----|-----|
| Red | #FF6B6B | (255, 107, 107) |
| Blue | #4ECDC4 | (78, 205, 196) |
| Green | #A8E6CF | (168, 230, 207) |
| Yellow | #FFE66D | (255, 230, 109) |
| Purple | #C44DFF | (196, 77, 255) |

---

## Как тестировать

### В Play mode:

1. Запустите игру
2. В меню: **Starlock → Test: Spawn Random Shape**
3. Фигура появится в центре (0, 0, 0)

### Вручную через код:

```csharp
// Создать конкретную фигуру
Shape shape = ShapeFactory.Instance.CreateShape(
    ShapeType.Circle, 
    ShapeColor.Red, 
    new Vector3(1, 0, 0)
);

// Создать случайную фигуру
Shape randomShape = ShapeFactory.Instance.CreateRandomShape(Vector3.zero);

// Создать пару для матча
Shape[] pair = ShapeFactory.Instance.CreateMatchingPair(
    ShapeType.Square, 
    ShapeColor.Blue, 
    new Vector3(-1, 0, 0), 
    new Vector3(1, 0, 0)
);
```

---

## Shape API

### Свойства:
- `Type` — ShapeType (readonly)
- `Color` — ShapeColor (readonly)
- `State` — ShapeState (readonly)

### Методы:
- `Initialize(type, color, sprite)` — инициализация
- `SetState(newState)` — изменить состояние
- `MoveInside(targetPosition, onComplete)` — анимация перемещения внутрь круга
- `PlayMatchedAnimation(onComplete)` — анимация при матче (scale up → down → destroy)
- `Matches(otherShape)` — проверка совпадения типа и цвета
- `ApplyForce(force)` — применить силу
- `SetVelocity(velocity)` — установить скорость

### События:
- `OnStateChanged` — при смене состояния
- `OnMatched` — при матче (перед уничтожением)
- `OnEnteredCircle` — при входе внутрь круга

---

## Иерархия после настройки

```
Gameplay Scene
├── ...
├── RotationPivot
│   ├── CircleContainer
│   ├── OuterZone
│   └── ShapesParent        (NEW - родитель для фигур)
│       ├── Shape_Circle_Red
│       ├── Shape_Square_Blue
│       └── ...
├── ShapeFactory            (NEW)
└── ...
```

---

## Следующая итерация

**Итерация 4:** Внешняя зона — физика "стиральной машины"
- Спавн фигур во внешней зоне
- Физика: фигуры плавают/крутятся от вращения
- Настройка bounce и трения
