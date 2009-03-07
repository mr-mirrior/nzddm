using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DM.Geo
{
    public struct DMMatrix
    {
        public DMRectangle boundary;
        public Coord origin;

        // rotate
        public double degrees;
        public Coord at;
        // scale
        public double zoom;
        // offset
        public Coord offset;

        public DMMatrix(DMMatrix mtx)
        {
            this.boundary = new DMRectangle(mtx.boundary);
            this.origin = new Coord(mtx.origin);
            this.degrees = mtx.degrees;
            this.at = new Coord(mtx.at);
            this.zoom = mtx.zoom;
            this.offset = new Coord(mtx.offset);
        }
    }
}
