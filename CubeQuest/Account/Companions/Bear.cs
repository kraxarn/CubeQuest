using CubeQuest.Account.Interface;

namespace CubeQuest.Account.Companions
{
    public class Bear : ICompanion
    {
        public int Health => 10;
        public int Armor  => 5;
        public int Attack => 2;

        public float Evasion => -0.5f;

        public string Icon => "bear";

        public string Name => "Bear";
        public string Info => "Provides a lot of strength at the cost of evasion";

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