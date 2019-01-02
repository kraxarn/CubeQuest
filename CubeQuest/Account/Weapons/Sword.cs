using CubeQuest.Account.Interface;

namespace CubeQuest.Account.Weapons
{
    public class Sword : IWeapon
    {
        public string Icon => "sword";

        public string Name => "Sword";
        public string Info => "Basic sword";

        public int Health => 0;
        public int Armor  => 0;
        public int Attack => 1;
    }
}