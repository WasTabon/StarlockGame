# Итерация 4: Внешняя зона — физика "стиральной машины"

## Что добавлено

### Новые скрипты

**Gameplay:**
- `OuterZonePhysics.cs` — эффект центрифуги, силы от вращения
- `ShapeSpawner.cs` — спавн фигур парами во внешней зоне

**Editor:**
- `StarlockSetup_04_Physics.cs` — настройка физики

### Обновлённые скрипты

- `ShapeFactory.cs` — добавлен PhysicsMaterial2D с bounce

---

## Установка

### 1. Скопируйте файлы

| Файл | Куда |
|------|------|
| `ShapeFactory.cs` | `Assets/StarlockGame/Scripts/Gameplay/` (заменить!) |
| `OuterZonePhysics.cs` | `Assets/StarlockGame/Scripts/Gameplay/` |
| `ShapeSpawner.cs` | `Assets/StarlockGame/Scripts/Gameplay/` |
| `StarlockSetup_04_Physics.cs` | `Assets/StarlockGame/Editor/` |

### 2. Откройте Gameplay сцену

### 3. Запустите Editor скрипт

**Starlock → Setup Physics (Iteration 4)**

### 4. Сохраните сцену

---

## Что делает Editor скрипт

- Создаёт `OuterZonePhysics` и настраивает ссылки
- Создаёт `ShapeSpawner` (5 пар фигур на старте)
- Создаёт `PhysicsMaterial2D` с bounce = 0.8
- Применяет материал к `CircleBoundary` и `OuterBoundary`
- Обновляет настройки `ShapeFactory`

---

## Настройки в инспекторе

### OuterZonePhysics

| Параметр | Описание | По умолчанию |
|----------|----------|--------------|
| Tangential Force Multiplier | Сила по касательной (вращение) | 2 |
| Centrifugal Force Multiplier | Центробежная сила | 0.5 |
| Max Force | Максимальная сила | 10 |
| Velocity Damping | Затухание скорости | 0.98 |
| Max Velocity | Максимальная скорость | 8 |

### ShapeSpawner

| Параметр | Описание | По умолчанию |
|----------|----------|--------------|
| Pairs To Spawn | Количество пар фигур | 5 |
| Min Distance Between Shapes | Минимальное расстояние между фигурами | 0.5 |
| Spawn On Start | Спавнить при запуске | true |

### ShapeFactory

| Параметр | Описание | По умолчанию |
|----------|----------|--------------|
| Bounciness | Упругость отскока (0-1) | 0.8 |
| Friction | Трение (0-1) | 0.1 |

---

## Иерархия после настройки

```
Gameplay Scene
├── ...
├── RotationPivot
│   ├── CircleContainer
│   │   ├── CircleVisual
│   │   └── CircleBoundary (+ bouncy material)
│   ├── OuterZone
│   │   └── OuterBoundary (+ bouncy material)
│   └── ShapesParent
│       ├── Shape_Circle_Red
│       ├── Shape_Circle_Red
│       ├── Shape_Square_Blue
│       └── ...
├── ShapeFactory
├── OuterZonePhysics      (NEW)
└── ShapeSpawner          (NEW)
```

---

## Как тестировать

### В Play mode:

1. Нажмите Play
2. 10 фигур (5 пар) появятся во внешней зоне
3. Фигуры будут "плавать" и сталкиваться
4. При столкновении со стенками — упругий отскок
5. Вращение создаёт эффект центрифуги

### Тестовые команды:

- **Starlock → Test: Respawn All Shapes** — пересоздать все фигуры

---

## Ожидаемый результат

- ✅ При запуске 10 фигур (5 пар) появляются во внешней зоне
- ✅ Фигуры "плавают" и сталкиваются друг с другом
- ✅ Отскакивают от внешней границы как мячики (bounce = 0.8)
- ✅ Отскакивают от границы центрального круга
- ✅ Вращение системы создаёт эффект центрифуги / стиралки
- ✅ Фигуры не проваливаются сквозь стенки
- ✅ Скорость фигур ограничена (не разгоняются бесконечно)

---

## Следующая итерация

**Итерация 5:** Ввод и перемещение фигур внутрь круга
- Тап по фигуре → она летит внутрь круга
- Физика внутри круга
- Базовая логика матчинга
