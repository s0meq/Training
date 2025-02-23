﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Training
{
    public struct CurrentPlayers
    {
        public PlayerProfile PlayerOne {  get; set; }
        public PlayerProfile PlayerTwo {  get; set; }

        public CurrentPlayers UpdatePlayers(PlayerProfile playerOne, PlayerProfile playerTwo)
        {
            return new CurrentPlayers
            {
                PlayerOne = playerOne,
                PlayerTwo = playerTwo
            };
        }
    }
}
