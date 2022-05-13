using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObjects
{
    public class Game
    {
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
        [Required]
        public int GameID { get; set; }

        [Required(ErrorMessage = "Please enter a name for the white player")]
        [StringLength(30, ErrorMessage = "The name can not be longer than 30 characters.")]
        public string PlayerWhite { get; set; }

        [Range(0, 4000, ErrorMessage = "Please enter an Elo rating for the white player")]
        public int WhiteElo { get; set; }

        [Required(ErrorMessage = "Please enter a name for the black player")]
        [StringLength(30, ErrorMessage = "The name can not be longer than 30 characters.")]
        public string PlayerBlack { get; set; }

        [Range(0, 4000, ErrorMessage = "Please enter an Elo rating for the black player")]
        public int BlackElo { get; set; }

        [RegularExpression(@"^[A-Z]\d{2}$", ErrorMessage = "You must enter an ECO.\n\nFormat should be a capital letter followed by a two digit number.\ne.g. B33")]
        public string ECO { get; set; }

        [Required(ErrorMessage = "Please enter an opening")]
        [StringLength(50, ErrorMessage = "The opening name can not be longer than 50 characters.")]
        public string Opening { get; set; }

        [Required(ErrorMessage = "Please enter a termination type")]
        [StringLength(20, ErrorMessage = "The termination type can not be longer than 20 characters.")]
        public string Termination { get; set; }

        [Required(ErrorMessage = "Please enter an outcome")]
        [RegularExpression(@"^1\-0$|^0\-1$|^1\/2\-1\/2$", ErrorMessage = "You must enter an outcome.\n\nOutcome must be either 1-0, 0-1, or 1/2-1/2")]
        public string Outcome { get; set; }

        [Required]
        [RegularExpression(@"^(\d+\+\d+)$", ErrorMessage = "You must enter a Time Control.\n\nFormat should be '[total seconds]+[increment seconds]'.\ne.g. 180+2")]
        public string TimeControl { get; set; }

        [Required(ErrorMessage = "Please enter the date the game was played")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime DatePlayed { get; set; }
        public List<Move> Moves { get; set; }
        public string DisplayDatePlayed
        {
            get
            {
                if (DatePlayed == null)
                {
                    return "";
                }
                DateTime display = (DateTime)DatePlayed;
                return display.ToString("d");
            }
        }
    }
}
