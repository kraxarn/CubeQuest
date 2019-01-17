namespace CubeQuest.Account.Interface
{
	public interface ICompanion : IItem
    {
		/// <summary>
		/// Percentage evasion from 0.0-1.0
		/// </summary>
	    float Evasion { get; }

		/// <summary>
		/// The companion's primary ability
		/// </summary>
		ECompanionType Type { get; }

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

    public enum ECompanionType
    {
	    Defensive,
	    Offensive,
	    Passive
    }
}