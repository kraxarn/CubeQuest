using CubeQuest.Account.Interface;

namespace CubeQuest.Account.Companions
{
    public class Parrot : ICompanion
    {
        public int Health => 0;
        public int Armor  => 2;
        public int Attack => 0;

        public float Evasion => 0.2f;

        public string Icon => "parrot";

        public string Name => "Parrot";
        public string Info => "Provides armor";

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