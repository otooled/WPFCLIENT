using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelecTunes_Client
{
    internal class Track
    {
        public String Name { get; set; }
        public String Artist { get; set; }
        public String Album { get; set; }
        public String Location { get; set; }
        public TimeSpan Duration { get; set; }

        public Track()
        {

        }

        public Track(string nme, string art, string alb, string loc, TimeSpan dur)
        {
            Name = nme;
            Artist = art;
            Album = alb;
            Location = loc;
            Duration = dur;
        }

        public override string ToString()
        {
            return String.Format("{0}", Name);
        }
    }
}
