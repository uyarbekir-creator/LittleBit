public class Consts
{

    public struct SceneNames
    {
        public const string MENU_SCENE = "MenuScene";
        public const string GAME_SCENE = "GameScene";
    }
    public struct Layers
    {
        public const string GROUND_LAYER = "Ground";
        public const string FLOOR_LAYER = "Floor";
    }

    public struct PlayerAnimations
    {
        public const string IS_MOVING = "isMoving";
        public const string IS_JUMPING = "isJumping";
        public const string IS_SLIDING = "isSliding";
        public const string IS_SLIDING_ACTIVE = "isSlidingActive";
    }

    public struct CatAnimations
    {
        public const string IS_IDLING = "IsIdling";
        public const string IS_WALKING = "IsWalking";
        public const string IS_RUNNING = "IsRunning";
        public const string IS_ATTACKING = "IsAttacking";
    }

    public struct OtherAnimations
    {
        public const string IS_SPATULA_JUMPING = "IsSpatulaJumping";
    }

    public struct WheatTypes
    {
        public const string GOLD_WHEAT = "GoldWheat";
        public const string HOLY_WHEAT = "HolyWheat";
        public const string ROTTEN_WHEAT = "RottenWheat";
    }
}
