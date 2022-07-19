using C969_Task_1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace C969_Task_1
{
    public static class Validations
    {
        public static OldCustomer ValidateCustomerData(UnvalidatedCustomer customer)
        {
            // no name validations other than not null, as name can be anything
            if (string.IsNullOrWhiteSpace(customer.Name))
            {
                throw new ValidationException("Name cannot be empty");
            }

            // phone number validations
            if (string.IsNullOrWhiteSpace(customer.PhoneNumber))
            {
                throw new ValidationException("Phone Number cannot be empty");
            }
            if (!Regex.IsMatch(customer.PhoneNumber, @"^[\d\-\s]*$"))
            {
                throw new ValidationException("Phone Number must only use 0-9, - and white space characters");
            }

            // address validations, also checking for data present
            if (string.IsNullOrWhiteSpace(customer.Address))
            {
                throw new ValidationException("Address cannot be empty");
            }

            // city validations, also checking for data present
            // cities in other countries may have numbers in their name (City 17)
            if (string.IsNullOrWhiteSpace(customer.City))
            {
                throw new ValidationException("City cannot be empty");
            }

            // zip code validations, should only ever be digits
            if (string.IsNullOrWhiteSpace(customer.ZipCode))
            {
                throw new ValidationException("Zip Code cannot be empty");
            }
            if (!Regex.IsMatch(customer.ZipCode, @"^\d*$"))
            {
                throw new ValidationException("Zip Code must only use 0-9");
            }

            // country validations, should only ever be two letters
            if (string.IsNullOrWhiteSpace(customer.Country))
            {
                throw new ValidationException("Country cannot be empty");
            }
            if (!Regex.IsMatch(customer.Country, @"^\w\w$"))
            {
                throw new ValidationException("Country should use a two letter abbreviation (FR, US, etc)");
            }

            return new OldCustomer(
                customer.Id,
                customer.Name,
                customer.PhoneNumber,
                customer.Address,
                customer.City,
                int.Parse(customer.ZipCode),
                customer.Country);
        }
    }
}
