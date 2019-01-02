using CubeQuest.Account.Interface;

namespace CubeQuest.Account.Weapons
{
    public class Staff : IWeapon
    {
        public string Icon => "staff";

        public string Name => "Magic Staff";
        public string Info => "Magic staff for... well, magic";

        public int Health => -2;
        public int Armor  => -2;
        public int Attack => 8;
    }
}