using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SohatNoteBook.Entities.DbSet
{
    public class HealthData : BaseEntity
    {
        public decimal  Height { get; set; }
        public decimal Weight { get; set; }
        public bool BloodType { get; set; }     // TODO: make this information base on enum (present the dropdown)
        public string  Race { get; set; }
        public bool UseGlasses { get; set; }
    }
}
