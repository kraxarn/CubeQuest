using CubeQuest.Account.Interface;

namespace CubeQuest.Account.Companions
{
    public class Buffalo : ICompanion
    {
        public int Health => 0;
        public int Armor  => 0;
        public int Attack => 10;

        public float Evasion => -0.2f;

        public string Icon => "buffalo";

        public string Name => "Buffalo";
        public string Info => "Provides a lot of attack at the cost of some evasion";

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