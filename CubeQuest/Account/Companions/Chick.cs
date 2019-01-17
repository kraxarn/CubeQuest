using CubeQuest.Account.Interface;

namespace CubeQuest.Account.Companions
{
    public class Chick : ICompanion
    {
        public int Health => -2;
        public int Armor  => 2;
        public int Attack => -2;

        public float Evasion => 0.6f;

        public string Icon => "chick";

        public string Name => "Chicken";
        public string Info => "Provides a lot of evasion at the cost of health and attack";

        public ECompanionType Type => ECompanionType.Passive;

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