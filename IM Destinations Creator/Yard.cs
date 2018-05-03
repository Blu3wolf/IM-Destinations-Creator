using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM_Destinations_Creator
{
    public class Yard
    {
        public Yard(int yardID, string yardName)
        {
            YardID = yardID;
            YardName = yardName;
        }

        public int YardID { get; }
        public string YardName { get; }
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
