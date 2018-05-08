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

        public override bool Equals(object obj)
        {
            Yard other = obj as Yard;
            if (other == null)
            {
                return false;
            }
            return Equals(other);
        }

        public bool Equals(Yard other)
        {
            if (other == null)
            {
                return false;
            }
            return this.YardID == other.YardID;
        }

        public static bool operator == (Yard yard1, Yard yard2)
        {
            if (((object)yard1) == null || ((object)yard2) == null)
            {
                return Object.Equals(yard1, yard2);
            }
            return yard1.Equals(yard2);
        }

        public static bool operator !=(Yard yard1, Yard yard2)
        {
            if (((object)yard1) == null || ((object)yard2) == null)
                return !Object.Equals(yard1, yard2);

            return !(yard1.Equals(yard2));
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
