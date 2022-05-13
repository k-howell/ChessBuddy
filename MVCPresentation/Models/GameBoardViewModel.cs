using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataObjects;

namespace MVCPresentation.Models
{
    public class GameBoardViewModel
    {
        public Board Board { get; set; }
        public Game Game { get; set; }
        public int Turn { get; set; }
        public string Side { get; set; }
    }
}