using CubeQuest.Account.Interface;

namespace CubeQuest.Account.Companions
{
    public class Rabbit : ICompanion
    {
        public int Health => -4;
        public int Armor  => -2;
        public int Attack => -4;

        public float Evasion => 0.8f;

        public string Icon => "rabbit";

        public string Name => "Rabbit";
        public string Info => "Provides a lot of evasion at the cost of reduced stats";

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