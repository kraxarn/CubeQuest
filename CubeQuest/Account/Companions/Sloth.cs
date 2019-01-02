using CubeQuest.Account.Interface;

namespace CubeQuest.Account.Companions
{
    public class Sloth : ICompanion
    {
        public int Health => 4;
        public int Armor  => 6;
        public int Attack => -4;

        public float Evasion => -0.4f;

        public string Icon => "sloth";

        public string Name => "Sloth";
        public string Info => "Provides health and armor at the cost of attack and evasion";

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