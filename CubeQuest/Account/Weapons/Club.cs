using CubeQuest.Account.Interface;

namespace CubeQuest.Account.Weapons
{
    public class Club : IWeapon
    {
        public string Icon => "club";

        public string Name => "Club";
        public string Info => "Like a poor man's axe";

        public int Health => 1;
        public int Armor  => 1;
        public int Attack => 2;
    }
}