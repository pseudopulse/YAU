# ContentScanner
A class that provides a method for scanning an assembly for ContentBase-like classes and instantiating them.

### Methods:
* ScanContent(Assembly, YAUContentPack, ConfigFile) : void

# ItemBase
The base class for an ItemDef, inherit from this when making one. Auto-generates an enabled/disable config and handles language tokens.

### Fields and Properties
* Name : string
* Lore : string
* FullDescription : string
* PickupDescription : string
* PickupModelPrefab : GameObject
* ItemTags : Enum[]
* RequiredExpansion : ExpansionDef
* ConfigSafeName : string
* CanRemove : bool
* ItemTier : ItemTier
* ItemTierDef : ItemTierDef
* Unlockable : UnlockableDef
* Icon : Sprite
* TokenName : string
* ItemDef : ItemDef (stores the created ItemDef)


### Methods
* Instantiate(YAUContentPack, ConfigFile) : void
