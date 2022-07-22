using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C969_Task_1.Models
{
    /// <summary>
    /// A customer model as it exists in the MySQL database. Meant for interfacing with the customer table in the MySQL database.
    /// </summary>
    public class Customer
    {
        public int Id;
        public string CustomerName;
        public int AddressId;
        public bool Active;
        public DateTime CreateDate;
        public string CreatedBy;
        public DateTime LastUpdate;
        public string LastUpdateBy;

        public Customer() { }

        public Customer(int id, string name, int addressId, bool active, DateTime createDate, string createBy, DateTime lastUpdate, string lastUpdateBy)
        {
            Id = id;
            CustomerName = name;
            AddressId = addressId;
            Active = active;
            CreateDate = createDate;
            CreatedBy = createBy;
            LastUpdate = lastUpdate;
            LastUpdateBy = lastUpdateBy;
        }
    }
}
