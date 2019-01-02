using CubeQuest.Account.Interface;

namespace CubeQuest.Account.Weapons
{
    public class BigSword : IWeapon
    {
        public string Icon => "big_sword";

        public string Name => "Big Sword";
        public string Info => "More sword = more damage";

        public int Health => 0;
        public int Armor  => -1;
        public int Attack => 4;
    }
}