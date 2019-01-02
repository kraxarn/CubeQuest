using CubeQuest.Account.Interface;

namespace CubeQuest.Account.Weapons
{
    public class NinjaStar : IWeapon
    {
        public string Icon => "ninja_star";

        public string Name => "Ninja Stars";
        public string Info => "Somehow, unlimited ninja stars";

        public int Health => -2;
        public int Armor  => -4;
        public int Attack => 8;
    }
}