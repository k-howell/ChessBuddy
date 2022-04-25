using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObjects
{
    public class Move
    {
        /*
         	GameID				[int]						NOT NULL,
	        Turn				[int]						NOT NULL,
	        Color				[nchar](5)					NOT NULL,
	        Piece				[nvarchar](6)				NOT NULL,
	        Notation			[nvarchar](10)				NOT NULL,
	        Origin				[nchar](2)					NOT NULL,
	        Destination			[nchar](2)					NOT NULL,
	        Capture				[nvarchar](6)				NULL,
	        Promotion			[nvarchar](6)				NULL,
	        IsCheck				[bit]						NOT NULL DEFAULT 0,
	        IsMate				[bit]						NOT NULL DEFAULT 0,
	        IsCastle			[bit]						NOT NULL DEFAULT 0,
	        IsEnPassant			[bit]						NOT NULL DEFAULT 0,
        */
        public string Origin { get; set; }
        public string Destination { get; set; }
        public PieceType PieceType { get; set; }
        public Color Color { get; set; }
        public string Notation { get; set; }
        public PieceType Promotion { get; set; }
        public PieceType Capture { get; set; }
        public bool IsCheck { get; set; }
        public bool IsCastle { get; set; }
        public bool IsEnPassant { get; set; }
    }
}
