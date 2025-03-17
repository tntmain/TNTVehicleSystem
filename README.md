# 🚗 Vehicle System Plugin

## 📽️ Demonstration
[Watch on YouTube](https://www.youtube.com/watch?v=i3FtyoChhSo)

## 🔧 Requirements
To use this plugin, you need:
- **TNTPlus plugin**
- **Harmony lib**

## 📜 Plugin Commands
### 🔹 Basic Commands:
- `/mv` — Opens the vehicle management interface.
- `/mv reg` — Registers a vehicle.
- `/mv unreg` — Unregisters a vehicle.
- `/mv transfer` — Transfers vehicle ownership to another player.

### 🔹 Admin Commands:
- `/vs givekey` — Gives a vehicle key.
- `/vs tp` — Teleports a player to their vehicle.
- `/vs tpv` — Teleports a vehicle to the player.
- `/vs setowner` — Assigns a new vehicle owner.
- `/vs setnumber` — Sets a vehicle's registration number.
- `/vs getground` — Retrieves information about the surface the player is standing on.

## ⚙️ Plugin Configuration
### 🔧 Main Settings:
1. **LockWithKeyOnly** — Access to the vehicle is only possible with a key.
2. **NumberType** — Vehicle plate format (`{charEN:1}{number:3}{charRU:2}`).
3. **RepairCost** — Repair cost (0 for free repairs).
4. **AutoRegisterOnFirstEntry** — Automatically registers the vehicle upon first entry.
5. **AutoIssueKey** — Automatically gives a key when registering a vehicle.
6. **EmptyKeyId** — Identifier for an empty vehicle key.
7. **KeyIds** — List of key IDs for vehicle access.
8. **KeyActionDistance** — Distance at which a key can be used.
9. **ExcludedVehicles** — List of ignored vehicles.
10. **Carjacking** — Determines whether jacking up the vehicle is allowed.
11. **CarjackingOwnerOnly** — Jacking up the vehicle is allowed only for the owner.
12. **RoadSurfaceTypes** — Types of road surfaces where the vehicle can be used.
13. **OffRoadTireDamage** — Whether tire damage is possible when driving off-road.
14. **OffRoadTireBreakSpeed** — Speed at which tires break off-road.
15. **OffRoadCheckInterval** — Time interval for checking vehicle condition when driving off-road.

## 📌
This plugin provides convenient vehicle management, key support, security settings, and a variety of useful commands. If you have any questions or suggestions, create an issue on GitHub! 🚀

##Ru

## 🔧 Требования
Для работы плагина необходимы:
- **TNTPlus plugin**
- **Harmony lib**

## 📜 Команды плагина
### 🚗 Основные команды
- `/mv` – Открывает интерфейс автопарка.
- `/mv reg` – Регистрирует автомобиль.
- `/mv unreg` – Отменяет регистрацию автомобиля.
- `/mv transfer` – Передает владение автомобилем другому игроку.

### 🛠️ Команды для администраторов
- `/vs givekey` – Выдает ключ от автомобиля.
- `/vs tp` – Телепортирует игрока к машине.
- `/vs tpv` – Телепортирует машину к игроку.
- `/vs setowner` – Устанавливает нового владельца автомобиля.
- `/vs setnumber` – Устанавливает номер автомобиля.
- `/vs getground` – Получает информацию о поверхности, на которой вы стоите.

## ⚙️ Конфигурация плагина
### 🔑 Ключевые параметры
- **`LockWithKeyOnly`** – Доступ к транспорту возможен только с ключом.
- **`NumberType`** – Формат номера автомобиля: `{charEN:1}{number:3}{charRU:2}` (пример: A123BC).
- **`RepairCost`** – Стоимость ремонта (0 – бесплатный ремонт).
- **`AutoRegisterOnFirstEntry`** – Автоматическая регистрация при первой посадке.
- **`AutoIssueKey`** – Автоматическая выдача ключа при регистрации.

### 🚘 Дополнительные настройки
- **`EmptyKeyId`** – ID пустого ключа.
- **`KeyIds`** – Список идентификаторов ключей.
- **`KeyActionDistance`** – Радиус использования ключа.
- **`ExcludedVehicles`** – Игнорируемые транспортные средства.
- **`Carjacking`** – Разрешено ли использование домкрата.
- **`CarjackingOwnerOnly`** – Домкрат доступен только владельцу.
- **`RoadSurfaceTypes`** – Разрешенные типы дорог для езды.
- **`OffRoadTireDamage`** – Возможность повреждения шин при езде вне дорог.
- **`OffRoadTireBreakSpeed`** – Скорость, при которой шины ломаются вне дорог.
- **`OffRoadCheckInterval`** – Интервал проверки состояния транспорта вне дорог.

## 📌
Этот плагин добавляет удобное управление транспортными средствами, поддержку ключей, настройки безопасности и множество полезных команд. Если у вас есть вопросы или предложения, создайте issue на GitHub! 🚀
