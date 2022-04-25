using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObjects
{
    public class Game
    {
        private string _openingName;
        /*
			GameID				[int] IDENTITY(100000, 1)
            PlayerWhite			[nvarchar](20)			
            EloWhite			[int]					
            PlayerBlack			[nvarchar](20)			
            EloBlack			[int]					
            ECO					[nchar](3)				
            OpeningVariation	[nvarchar](50)			
            Termination			[nvarchar](20)			
            Outcome				[nchar](3)				
            TimeControl			[nvarchar](10)			
            DatePlayed			[date]							
		*/
        DateTime _dateTime;

        public int GameID { get; set; }
        public string PlayerWhite { get; set; }
        public int WhiteElo { get; set; }
        public string PlayerBlack { get; set; }
        public int BlackElo { get; set; }
        // public Opening Opening { get; set; }
        public string ECO { get; set; }
        public string Opening { get; set; }
        public string Termination { get; set; }
        public string Outcome { get; set; }
        public string TimeControl { get; set; }
        public DateTime? DatePlayed { get; set; }
        public List<Move> Moves { get; set; }
    }
}
