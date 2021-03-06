﻿using CubeQuest.Account.Interface;

namespace CubeQuest.Account.Companions
{
    public class Penguin : ICompanion
    {
        public int Health => 0;
        public int Armor  => 0;
        public int Attack => 2;

        public float Evasion => 0f;

        public string Icon => "penguin";

        public string Name => "Penguin";
        public string Info => "Provides attack";

        public ECompanionType Type => ECompanionType.Offensive;

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