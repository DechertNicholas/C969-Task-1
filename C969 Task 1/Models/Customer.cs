using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C969_Task_1.Models
{
    /// <summary>
    /// A consolidated OldCustomer class containing all the information relavent to the customer, even
    /// if the information is separate from the database model. Meant for general use in the program.
    /// </summary>
    //public class OldCustomer
    //{
    //    public int? Id;
    //    public string Name;
    //    public string PhoneNumber;
    //    public string Address;
    //    public string City;
    //    public int ZipCode;
    //    public string Country;

    //    public OldCustomer(int? id, string name, string phoneNumber, string address, string city, int zipCode, string country)
    //    {
    //        Id = id;
    //        Name = name;
    //        PhoneNumber = phoneNumber;
    //        Address = address;
    //        City = city;
    //        ZipCode = zipCode;
    //        Country = country;
    //    }
    //}

    /// <summary>
    /// An unvalidated version of the OldCustomer class. Meant to be used for validation and converted into a validated OldCustomer class.
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
    public class Customer
    {
        public int Id;
        public string CustomerName;
        public int AddressId;
        public int Active;
        public DateTime CreateDate;
        public string CreatedBy;
        public DateTime LastUpdate;
        public string LastUpdateBy;

        public Customer() { }

        public Customer(int id, string name, int addressId, int active, DateTime createDate, string createBy, DateTime lastUpdate, string lastUpdateBy)
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
