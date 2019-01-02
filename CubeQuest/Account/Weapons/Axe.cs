using CubeQuest.Account.Interface;

namespace CubeQuest.Account.Weapons
{
    public class Axe : IWeapon
    {
        public string Icon => "axe";

        public string Name => "Axe";
        public string Info => "Basic axe";

        public int Health => 0;
        public int Armor  => 2;
        public int Attack => 2;
    }
}