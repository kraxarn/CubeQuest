namespace CubeQuest.Account.Interface
{
    /// <summary>
    /// For anything that provides the base stats, name and icon
    /// </summary>
    public interface IItem
    {
        /// <summary>
        /// Name with relative path of the icon without extension
        /// </summary>
        string Icon { get; }

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