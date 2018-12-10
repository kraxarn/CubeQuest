using CubeQuest.Account.Interface;
using fastJSON;
using System.Collections.Generic;

namespace CubeQuest.Account
{
    public class User
    {
        private IWeapon primaryWeapon;
        private IWeapon secondaryWeapon;

        private ISpell spell;

        private readonly List<ICompanion> companions;

        public IEnumerable<ICompanion> Companions => companions;

        private uint level;

        public User()
        {
            companions = new List<ICompanion>();

            level = 0;
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
    }
}