using CubeQuest.Account.Interface;

namespace CubeQuest.Account.Companions
{
    public class Walrus : ICompanion
    {
        public int Health => 4;
        public int Armor  => 4;
        public int Attack => 2;

        public float Evasion => -0.8f;

        public string Icon => "walrus";

        public string Name => "Walrus";
        public string Info => "Provides health, armor and attack at the cost of evasion";

        public ECompanionType Type => ECompanionType.Defensive;

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