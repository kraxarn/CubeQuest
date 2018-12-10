namespace CubeQuest.Account.Interface
{
    /// <summary>
    /// For anything that provides the base stats, name and icon
    /// </summary>
    // TODO: Icon
    public interface IStats
    {
        /// <summary>
        /// Name/species of the companion
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Information shown to the user
        /// </summary>
        string Info { get; }

        /// <summary>
        /// Extra health to player
        /// </summary>
        int Health { get; }

        /// <summary>
        /// Damage reduction from enemies
        /// </summary>
        int Armor { get; }

        /// <summary>
        /// Bonus damage to enemies
        /// </summary>
        int Attack { get; }
    }
}