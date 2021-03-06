﻿using CubeQuest.Account.Interface;

namespace CubeQuest.Account.Companions
{
    public class Narwhal : ICompanion
    {
        public int Health => 0;
        public int Armor  => 0;
        public int Attack => 4;

        public float Evasion => 0f;

        public string Icon => "narwhal";

        public string Name => "Narwhal";
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