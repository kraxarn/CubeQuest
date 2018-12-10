namespace CubeQuest.Account.Interface
{
    public interface ICompanion : IStats
    {
        /// <summary>
        /// Trigger before the battle
        /// </summary>
        void BeforeBattle();

        /// <summary>
        /// Trigger at the end of each turn in battle
        /// </summary>
        void DuringBattle();

        /// <summary>
        /// Trigger after the battle
        /// </summary>
        void AfterBattle();
    }
}