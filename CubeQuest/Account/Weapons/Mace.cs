using CubeQuest.Account.Interface;

namespace CubeQuest.Account.Weapons
{
    public class Mace : IWeapon
    {
        public string Icon => "mace";

        public string Name => "Mace";
        public string Info => "Very spiky";

        public int Health => -4;
        public int Armor  => 2;
        public int Attack => 6;
    }
}