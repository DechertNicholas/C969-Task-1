using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C969_Task_1.Models
{
    public class Country
    {
        public int CountryId;
        public string CountryName; // 'Country' in DB
        public DateTime CreateDate;
        public string CreatedBy;
        public DateTime LastUpdate;
        public string LastUpdateBy;

        public Country() { }

        public Country(int id, string name, DateTime createDate, string createBy, DateTime lastUpdate, string updateBy)
        {
            CountryId = id;
            CountryName = name;
            CreateDate = createDate;
            CreatedBy = createBy;
            LastUpdate = lastUpdate;
            LastUpdateBy = updateBy;
        }
    }
}
