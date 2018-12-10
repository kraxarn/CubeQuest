using CubeQuest.Account.Interface;

namespace CubeQuest.Account.Companions
{
    public class CompanionExample : ICompanion
    {
        public int Health => 0;
        public int Armor  => 0;
        public int Attack => 0;

        public string Name => "Example Companion";
        public string Info => "Does nothing, for testing only";

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