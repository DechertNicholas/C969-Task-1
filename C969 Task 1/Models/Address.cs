using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C969_Task_1.Models
{
    public class Address
    {
        public int AddressId;
        public string AddressName; // 'Address' in DB
        private string Address2; // Unused in this project, private prevent accidental assigning
        public int CityId;
        public string PostalCode; // stored as string in DB
        public string Phone;
        public DateTime CreateDate;
        public string CreatedBy;
        public DateTime LastUpdate;
        public string LastUpdateBy;
    }
}
