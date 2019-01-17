using CubeQuest.Account.Interface;

namespace CubeQuest.Account.Companions
{
    public class Wolf : ICompanion
    {
        public int Health => 0;
        public int Armor  => 0;
        public int Attack => 6;

        public float Evasion => 0.2f;

        public string Icon => "wolf";

        public string Name => "Wolf";
        public string Info => "Provides attack and evasion";

        public ECompanionType Type => ECompanionType.Offensive;

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