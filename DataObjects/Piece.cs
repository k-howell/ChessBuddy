﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObjects
{
    public class Piece
    {
        public PieceType Type { get; set; }
        public Color Color { get; set; }
        public Point Position { get; set; }
        public List<Point> Moves { get; set; }
        public List<Point> Threats { get; set; }

        public Piece(Color color, PieceType type, Point position)
        {
            Type = type;
            Color = color;
            Position = position;
        }
    }
}
