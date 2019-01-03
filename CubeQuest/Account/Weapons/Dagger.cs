using CubeQuest.Account.Interface;

namespace CubeQuest.Account.Weapons
{
    public class Dagger : IWeapon
    {
        public string Icon => "dagger";

        public string Name => "Dagger";
        public string Info => "Like a mini sword";

        public int Health => 0;
        public int Armor  => 1;
        public int Attack => 1;
    }
}