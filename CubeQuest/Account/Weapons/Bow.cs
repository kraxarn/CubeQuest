using CubeQuest.Account.Interface;

namespace CubeQuest.Account.Weapons
{
    public class Bow : IWeapon
    {
        public string Icon => "bow";

        public string Name => "Bow and Arrow";
        public string Info => "Bow and arrow for \"ranged\" attacks";

        public int Health => 0;
        public int Armor  => -2;
        public int Attack => 6;
    }
}