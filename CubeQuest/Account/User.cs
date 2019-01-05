﻿using System;
using CubeQuest.Account.Interface;
using fastJSON;
using System.Collections.Generic;

namespace CubeQuest.Account
{
    public class User
    {
        /// <summary>
        /// All companions the player has
        /// </summary>
        private readonly List<ICompanion> companions;

        /// <summary>
        /// All companions currently equipped by the user (max 3)
        /// </summary>
        private readonly List<ICompanion> equippedCompanions;
		
		/// <summary>
		/// Add experience to the user
		/// </summary>
        public void AddExperience(int amount) => 
	        experience += amount;

		/// <summary>
		/// Current user experience
		/// </summary>
        private float experience;

        /// <summary>
        /// Level of the user, decides base stats
        /// </summary>
        public uint Level =>
	        (uint) experience / 100;

        /// <summary>
        /// Damage reduction from attacks
        /// </summary>
        private int armor;

        private int health;

        /// <summary>
        /// User's current health
        /// </summary>
        public int Health
        {
	        get => health;

	        set
	        {
		        health = value;

		        if (health < 0)
			        health = 0;
				else if (health > MaxHealth)
			        health = MaxHealth;
	        }
        }

        /// <summary>
        /// Current health in percentage
        /// </summary>
        public int HealthPercentage => 
            (int) ((float) Health / MaxHealth) * 100;

        /// <summary>
        /// User's maximum health
        /// TODO
        /// </summary>
        public int MaxHealth
        {
            get
            {
                var h = 100 + (int) Level * 5;
				equippedCompanions.ForEach(c => h += c.Health);
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
		        var a = (int) Level * 2;
				equippedCompanions.ForEach(c => a += c.Attack);
		        return a;
			}
        }

		/// <summary>
		/// Percentage evasion from companions
		/// </summary>
        private float Evasion
		{
	        get
	        {
		        var e = 0f;
				
				// TODO: We probably don't want evasion stacking like this
				equippedCompanions.ForEach(c => e += c.Evasion);

		        if (e > 1f)
			        e = 1f;

		        return e;
	        }
        }

		/// <summary>
		/// If the next attack should hit based on the current evasion
		/// </summary>
        private bool ShouldHit => 
			new Random().NextDouble() <= Evasion;

		public User()
        {
            companions = new List<ICompanion>();
            equippedCompanions = new List<ICompanion>(3);
            experience = 100;
            Health = MaxHealth;
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