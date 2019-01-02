using CubeQuest.Account.Interface;

namespace CubeQuest.Account.Companions
{
    public class Owl : ICompanion
    {
        public int Health => 0;
        public int Armor  => 1;
        public int Attack => 3;

        public float Evasion => 0.2f;

        public string Icon => "owl";

        public string Name => "Owl";
        public string Info => "Provides attack and evasion";

        public void BeforeBattle()
        {
        }

        public void DuringBattle()
        {
        }

        public void AfterBattle()
        {
        }
    }
}