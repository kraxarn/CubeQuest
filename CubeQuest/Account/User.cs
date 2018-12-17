using CubeQuest.Account.Interface;
using fastJSON;
using System.Collections.Generic;

namespace CubeQuest.Account
{
    public class User
    {
        /// <summary>
        /// Primary weapon equipped, like a sword
        /// </summary>
        private IWeapon primaryWeapon;

        /// <summary>
        /// Secondary weapon equipped, like a shield
        /// </summary>
        private IWeapon secondaryWeapon;

        /// <summary>
        /// Magic spell equipped
        /// </summary>
        private ISpell spell;

        /// <summary>
        /// All companions the player has
        /// </summary>
        private readonly List<ICompanion> companions;

        /// <summary>
        /// All companions currently equipped by the user (max 3)
        /// </summary>
        private readonly List<ICompanion> equippedCompanions;

        /// <summary>
        /// Level of the user, decides base stats - TODO
        /// </summary>
        private uint level;

        /// <summary>
        /// Damage reduction from attacks
        /// </summary>
        private int armor;

        /// <summary>
        /// User's current health
        /// </summary>
        public int CurrentHealth;

        /// <summary>
        /// User's maximum health
        /// TODO
        /// </summary>
        public int MaxHealth
        {
            get
            {
                var h = 100 + (int) level * 5;

                foreach (var companion in equippedCompanions)
                    h += companion.Health;

                return h;
            }
        }

        public User()
        {
            companions = new List<ICompanion>();
            equippedCompanions = new List<ICompanion>(3);
            level = 1;
        }

        /// <summary>
        /// Serializes <see cref="User"/> to a JSON string
        /// </summary>
        public override string ToString() => 
            JSON.ToJSON(this);

        /// <summary>
        /// Deserializes a JSON into a <see cref="User"/> 
        /// </summary>
        public static User FromString(string json) => 
            JSON.ToObject<User>(json);

        public void Attack()
        {
            // TODO
        }

        public void Damage()
        {
            // TODO
        }
    }
}