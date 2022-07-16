using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C969_Task_1.Models
{
    /// <summary>
    /// A consolidated Customer class containing all the information relavent to the customer, even
    /// if the information is separate from the database model. Meant for general use in the program.
    /// </summary>
    public class Customer
    {
        public int? Id;
        public string Name;
        public string PhoneNumber;
        public string Address;
        public string City;
        public int ZipCode;
        public string Country;

        public Customer(int? id, string name, string phoneNumber, string address, string city, int zipCode, string country)
        {
            Id = id;
            Name = name;
            PhoneNumber = phoneNumber;
            Address = address;
            City = city;
            ZipCode = zipCode;
            Country = country;
        }
    }

    /// <summary>
    /// An unvalidated version of the Customer class. Meant to be used for validation and converted into a validated Customer class.
    /// </summary>
    public class UnvalidatedCustomer
    {
        public int? Id;
        public string Name;
        public string PhoneNumber;
        public string Address;
        public string City;
        public string ZipCode;
        public string Country;

        public UnvalidatedCustomer(int? id, string name, string phoneNumber, string address, string city, string zipCode, string country)
        {
            Id = id;
            Name = name;
            PhoneNumber = phoneNumber;
            Address = address;
            City = city;
            ZipCode = zipCode;
            Country = country;
        }
    }

    /// <summary>
    /// A customer model as it exists in the MySQL database. Meant for interfacing with the customer table in the MySQL database.
    /// </summary>
    public class DBCustomer
    {
        public int Id;
        public string Name;
        public int AddressId;
        public int Active;
        public string CreateDate;
        public string CreatedBy;
        public string LastUpdate;
        public string LastUpdateBy;

        public DBCustomer(int id, string name, int addressId, int active, string createDate, string createBy, string lastUpdate, string lastUpdateBy)
        {
            Id = id;
            Name = name;
            AddressId = addressId;
            Active = active;
            CreateDate = createDate;
            CreatedBy = createBy;
            LastUpdate = lastUpdate;
            LastUpdateBy = lastUpdateBy;
        }
    }
}
