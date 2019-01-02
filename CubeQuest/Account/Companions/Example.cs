using CubeQuest.Account.Interface;

namespace CubeQuest.Account.Companions
{
    public class Example : ICompanion
    {
        public int Health => 0;
        public int Armor  => 0;
        public int Attack => 0;

        public float Evasion => 0f;

        public string Icon => "companion/example";

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