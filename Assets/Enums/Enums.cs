
namespace Enums
{
    public enum ModifiesStat
    {
        None,
        Damage,
        BulletCount,
        BulletSpeed,
        FireRate,
        Spread,
        CooldownDuration,
        Bounces,
        HomingRange,
        Gravity
    }

    public enum Rarity
    {
        Frequent,
        Infrequent,
        Jolly,
        Stupendous,
        Outstanding,
        Majestic
    }

    public enum EnhanceableItem
    {
        Gun,
        Sword
    }

    public enum ModifierApplyType
    {
        None,
        Additive,
        Multiplicative
    }

    public enum GearType
    {
        Head = 1,
        Chest = 2,
        Legs = 3,
        Feet = 4,
        Accessory = 5
    }

    public enum GunType
    {
        Burst,        //Burst Firing
        Single,       //Semi Automatic
        BarrelSpin,   //Mini-gun Type
        ChargeUp      //Charge before shot
    }

    public enum Team
    {
        Player,
        Enemy,
        None
    }

    public enum EnemyState
    {
        Idle,
        Chasing,
        Attacking
    }

    public enum LimbType
    {
        Head,
        Body,
        Limb,
        Armor
    }

    public enum ObjectiveType
    {
        ClearRooms,
        FindXItemsSmall,
        FindXItemsLarge,
        SeekEnemy,
        BossStage
    }

    public enum CraftableUsing
    {
        Hand,
        Machine
    }

    public enum LevelType
    {
        Interior,
        Exterior,
        InAndOut
    }

    public enum LevelExpansionType
    {
        Expansive,
        Linear
    }
}