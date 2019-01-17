using CubeQuest.Account.Interface;

namespace CubeQuest.Account.Companions
{
    public class Snake : ICompanion
    {
        public int Health => 0;
        public int Armor  => 0;
        public int Attack => 6;

        public float Evasion => 0.2f;

        public string Icon => "snake";

        public string Name => "Snake";
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