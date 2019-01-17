using CubeQuest.Account.Interface;
using CubeQuest.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CubeQuest.Account
{
	public class User
    {
        public delegate void OnHealthChangeEvent(int newHealth);

        public event OnHealthChangeEvent OnHealthChange;

        public delegate void OnDeadChangeEvent(bool isAlive);

        public event OnDeadChangeEvent OnDeadChange;

        /// <summary>
        /// All companions the player has
        /// </summary>
        public List<ICompanion> Companions { get; private set; }
        
        /// <summary>
        /// All companions currently equipped by the user (max 3)
        /// </summary>
        public List<ICompanion> EquippedCompanions { get; private set; }

        /// <summary>
        /// Total number of equipped companions (max 3)
        /// </summary>
        public int EquippedCompanionCount =>
            EquippedCompanions.Count;

        /// <summary>
        /// Total number of companions in our inventory
        /// </summary>
        public int CompanionCount =>
            Companions.Count;

        /// <summary>
        /// Add experience to the user
        /// </summary>
        public void AddExperience(int amount) =>
            experience += amount;

        /// <summary>
        /// Current user experience
        /// </summary>
        private float experience;

		public int ExperienceToNextLevel => 
			100 - (int) (experience % 100);

        /// <summary>
        /// Level of the user, decides base stats
        /// </summary>
        public uint Level =>
            (uint) experience / 100;

        /// <summary>
        /// Damage reduction from attacks
        /// </summary>
        public int Armor
        {
            get
            {
                var a = 0;
                EquippedCompanions.ForEach(c => a += c.Armor);
                return a;
            }
        }

        public bool IsAlive
        {
            get
            {
                if (isAlive == false && HealthPercentage >= 25)
                {
                    isAlive = true;
                    OnDeadChange?.Invoke(isAlive);
                }

                return isAlive;
            }
        }

        private bool isAlive;

        private int health;

        /// <summary>
        /// User's current health
        /// </summary>
        public int Health
        {
	        get
	        {
				// Update health if companions affected our max health
		        if (health > MaxHealth)
			        health = MaxHealth;

		        return health;
	        }

	        set
            {
                health = value;

                if (health <= 0)
                {
                    health = 0;
                    isAlive = false;
                    OnDeadChange?.Invoke(isAlive);
                }
                else if (!isAlive && HealthPercentage >= 25)
                {
                    isAlive = true;
                    OnDeadChange?.Invoke(isAlive);
                }
                else if (health > MaxHealth)
                    health = MaxHealth;

                OnHealthChange?.Invoke(health);
            }
        }

        /// <summary>
        /// Current health in percentage
        /// </summary>
        public int HealthPercentage
        {
			get => (int) ((float) Health / MaxHealth * 100);

            set => Health = (int) (MaxHealth * (value / 100f));
        }
        
        /// <summary>
        /// User's maximum health
        /// TODO
        /// </summary>
        public int MaxHealth
        {
            get
            {
                var h = 100 + (int) Level * 5;
                //EquippedCompanions.ForEach(c => h += c.Health);
                return h;
            }
        }

        /// <summary>
        /// Use's attack power
        /// </summary>
        public int Attack
        {
            get
            {
                var a = (int) Level * 10;
                EquippedCompanions.ForEach(c => a += c.Attack);
                return a;
            }
        }

        /// <summary>
        /// Percentage evasion from companions
        /// </summary>
        public float Evasion
        {
            get
            {
                var e = 0f;

                // TODO: We probably don't want evasion stacking like this
                EquippedCompanions.ForEach(c => e += c.Evasion);

                if (e > 1f)
                    e = 1f;

                return e;
            }
        }

        /// <summary>
        /// If the next attack should hit based on the current evasion
        /// </summary>
        public bool ShouldHit =>
            new Random().NextDouble() <= Evasion;

        public User()
        {
            Companions = new List<ICompanion>();
            EquippedCompanions = new List<ICompanion>(3);
            experience = 100;
            Health = MaxHealth;
            isAlive = true;

            AddStartingCompanions();
        }

        private void AddStartingCompanions()
        {
            foreach (var startingCompanion in CompanionManager.StartingCompanions)
                EquippedCompanions.Add(startingCompanion);
        }

        /// <summary>
        /// Get the total damage that should be done to the player
        /// </summary>
        public int GetDamage(int damage)
        {
            var d = damage - Armor;
            return Math.Max(1, d);
        }

        public void AddCompanion(ICompanion companion)
        {
            if (!Companions.Contains(companion))
	            Companions.Add(companion);
        }

        /// <summary>
        /// Serializes to a <see cref="SaveData"/> JSON
        /// </summary>
        /// <returns></returns>
        public override string ToString() =>
	        ToSaveData().ToString();

		/// <summary>
		/// Deserializes a <see cref="SaveData"/> string into a <see cref="User"/> 
		/// </summary>
		private static User FromText(string text) => 
			FromSaveData(new SaveData(text));

		public byte[] ToBytes() =>
	        Encoding.UTF8.GetBytes(ToString());

		/// <summary>
		/// Tries to load <see cref="SaveData"/> from string, returns null on failure
		/// </summary>
        public static User FromBytes(byte[] data) => 
	        data.Any() ? FromText(Encoding.UTF8.GetString(data)) : null;

        private SaveData ToSaveData() =>
	        new SaveData
	        {
		        Companions         = Companions.ToTypeList(),
		        EquippedCompanions = EquippedCompanions.ToTypeList(),
		        Experience         = experience,
		        Health             = Health
	        };
		
        private static User FromSaveData(SaveData save) =>
	        new User
	        {
		        Health             = save.Health,
		        experience         = save.Experience,
		        EquippedCompanions = save.EquippedCompanions.ToCompanions(),
		        Companions         = save.Companions.ToCompanions()
	        };
    }
}