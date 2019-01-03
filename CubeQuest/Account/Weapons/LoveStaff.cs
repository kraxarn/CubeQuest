using CubeQuest.Account.Interface;

namespace CubeQuest.Account.Weapons
{
    public class LoveStaff : IWeapon
    {
        public string Icon => "love_staff";

        public string Name => "Staff of Love";
        public string Info => "Allows you to spare enemies";

        public int Health => 0;
        public int Armor  => 0;
        public int Attack => 0;
    }
}