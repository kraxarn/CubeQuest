using CubeQuest.Account.Interface;

namespace CubeQuest.Account.Weapons
{
    public class Hammer : IWeapon
    {
        public string Icon => "hammer";

        public string Name => "Hammer";
        public string Info => "Not for hammering spikes";

        public int Health => -2;
        public int Armor  => 2;
        public int Attack => 4;
    }
}