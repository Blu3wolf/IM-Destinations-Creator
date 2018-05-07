using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM_Destinations_Creator
{
    public class Yard : IEquatable<Yard>
    {
        public Yard(int yardID, string yardName)
        {
            YardID = yardID;
            if (yardName == null)
            {
                yardName = "No Such Yard Found";
            }
            YardName = yardName;
        }

        public int YardID { get; }
        public string YardName { get; }

        bool IEquatable<Yard>.Equals(Yard other)
        {
            if (other == null)
            {
                return false;
            }
            if (this.YardID == other.YardID)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public class SourceYard : Yard
    {
        public SourceYard(int yardID, string yardName, ObservableCollection<Yard> destYards) : base(yardID, yardName)
        {
            DestYards = destYards;
        }

        public ObservableCollection<Yard> DestYards { get; set; }
    }
}
