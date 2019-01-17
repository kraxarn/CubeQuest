using CubeQuest.Account.Interface;

namespace CubeQuest.Account.Companions
{
    public class Horse : ICompanion
    {
        public int Health => 5;
        public int Armor  => 5;
        public int Attack => 0;

        public float Evasion => 0.1f;

        public string Icon => "horse";

        public string Name => "Horse";
        public string Info => "Provides health and armor";

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