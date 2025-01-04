using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Training
{
    public struct PlayerProfile
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DateOfBirth { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Draws { get; set; }
        public int MinutesPlayed { get; set; }
        public int SecondsPlayed { get; set; }

        public PlayerProfile AddWin(int minutesPlayed, int secondsPlayed)
        {
            var updated = this;
            updated.Wins += 1;
            updated.MinutesPlayed += minutesPlayed;
            updated.SecondsPlayed += secondsPlayed;
            if (updated.SecondsPlayed >= 60)
            {
                updated.MinutesPlayed++;
                updated.SecondsPlayed -= 60;
            }
            return updated;
        }

        public PlayerProfile AddLoss(int minutesPlayed, int secondsPlayed)
        {
            var updated = this;
            updated.Losses += 1;
            updated.MinutesPlayed += minutesPlayed;
            updated.SecondsPlayed += secondsPlayed;
            if (updated.SecondsPlayed >= 60)
            {
                updated.MinutesPlayed++;
                updated.SecondsPlayed -= 60;
            }
            return updated;
        }

        public PlayerProfile AddDraw(int minutesPlayed, int secondsPlayed)
        {
            var updated = this;
            updated.Draws += 1;
            updated.MinutesPlayed += minutesPlayed;
            
            updated.SecondsPlayed += secondsPlayed;
            if(updated.SecondsPlayed >= 60)
            {
                updated.MinutesPlayed++;
                updated.SecondsPlayed -= 60;
            }
            
            return updated;
        }
    }
}
