# ShinyVillage
Proyecto con Unity y C# para un RPG2D

Diagrama de clases actual

classDiagram

    class MonoBehaviour {
        <<Unity>>
    }
    class ScriptableObject {
        <<Unity>>
    }

    class GameManager {
        <<Singleton>>
        +static GameManager instance
        +ItemManager itemManager
        +TileManager tileManager
        -Awake()
    }

    class ItemManager {
        +Item[] items
        -Dictionary~string,Item~ nameToItemDict
        -Awake()
        -AddItem(Item item)
        +GetItemByName(string key) Item
    }

    class TileManager {
        -Tilemap interactableMap
        -Tile hiddenInteractableTile
        -Tile InteractedTile
        +Start()
        +IsInteractable(Vector3Int pos) bool
        +SetInteracted(Vector3Int pos)
        +GetCellPosition(Vector3 worldPos) Vector3Int
        +GetCellCenter(Vector3Int cellPos) Vector3
    }

    class Player {
        +Inventory inventory
        -Awake()
        -Update()
        +DropItem(Item item)
    }

    class Movement {
        +float speed
        +Animator animator
        -Vector3 direction
        -Update()
        -FixedUpdate()
        -animateMovement(Vector3 direction)
    }

    class CameraFollow {
        -Transform target
        -Vector3 camOffset
        -Start()
        -FixedUpdate()
    }

    class Item {
        +ItemData data
        +Rigidbody2D rb2d
        -Awake()
    }

    class ItemData {
        <<ScriptableObject>>
        +string itemName
        +Sprite icon
    }

    class Collectable {
        -OnTriggerEnter2D(Collider2D collision)
    }

    class Inventory {
        +List~Slot~ slots
        +Inventory(int numSlots)
        +Add(Item item)
        +Remove(int index)
    }

    class Inventory_Slot {
        <<Inventory.Slot>>
        +string itemName
        +int count
        +int maxAllowed
        +Sprite icon
        +Slot()
        +CanAddItem() bool
        +AddItem(Item item)
        +RemoveItem()
    }

    class Inventory_UI {
        +GameObject inventoryPanel
        +Player player
        +List~Slot_UI~ slots
        -Update()
        +ToggleInventory()
        +Refresh()
        +Remove(int slotID)
    }

    class Slot_UI {
        +Image itemIcon
        +TextMeshProUGUI quantityText
        +SetItem(Inventory.Slot slot)
        +SetEmpty()
    }

    MonoBehaviour <|-- GameManager
    MonoBehaviour <|-- ItemManager
    MonoBehaviour <|-- TileManager
    MonoBehaviour <|-- Player
    MonoBehaviour <|-- Movement
    MonoBehaviour <|-- CameraFollow
    MonoBehaviour <|-- Item
    MonoBehaviour <|-- Collectable
    MonoBehaviour <|-- Inventory_UI
    MonoBehaviour <|-- Slot_UI
    ScriptableObject <|-- ItemData

    GameManager "1" *-- "1" ItemManager : contiene
    GameManager "1" *-- "1" TileManager : contiene
    Player "1" *-- "1" Inventory : posee
    Player ..> GameManager : usa Singleton
    Player ..> Movement : GetComponent
    Item "1" *-- "1" ItemData : datos
    Collectable "1" --> "1" Item : RequireComponent
    Collectable ..> Player : detecta colision
    Collectable ..> Inventory_UI : refresca UI
    ItemManager "1" o-- "n" Item : registra prefabs
    Inventory "1" *-- "n" Inventory_Slot : contiene
    Inventory_UI "1" o-- "1" Player : referencia
    Inventory_UI "1" *-- "n" Slot_UI : contiene
    Inventory_UI ..> GameManager : usa Singleton
    Slot_UI ..> Inventory_Slot : muestra datos
    CameraFollow ..> Player : sigue Transform
