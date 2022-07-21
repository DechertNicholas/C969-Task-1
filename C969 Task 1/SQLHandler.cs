using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using C969_Task_1.Models;
using System.ComponentModel;
using System.Globalization;

namespace C969_Task_1
{
    public class SQLHandler
    {
        private Translator LocalTranslator = new Translator();


        public MySqlConnection GetConnection()
        {
            string connStr = ConfigurationManager.ConnectionStrings["RQLDEV01"].ConnectionString;
            return new MySqlConnection(connStr);
        }

        public void TestLogin(string username, string password, Translator.LanguageCode languageCode)
        {
            var conn = GetConnection();
            try
            {
                // If it can't open the connection, it'll throw an error. If we don't get an error, it worked
                conn.Open();
                var query = $"SELECT * FROM user WHERE userName = '{username}' AND password = '{password}'";
                var adapter = new MySqlDataAdapter(query, conn);
                var dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count == 0)
                {
                    throw new UnknownUserException(LocalTranslator.Translate(languageCode, "UnknownUser"));
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        // countries
        public List<Country> GetAllCountries()
        {
            var results = new List<Country>();

            var conn = GetConnection();
            try
            {
                conn.Open();

                var query = "SELECT * FROM country";
                var cmd = new MySqlCommand(query, conn);
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    results.Add(new Country(
                        int.Parse(rdr[0].ToString()), // id
                        rdr[1].ToString(), // name
                        DateTime.ParseExact(rdr[2].ToString(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                        rdr[3].ToString(),
                        DateTime.ParseExact(rdr[4].ToString(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                        rdr[5].ToString())
                    );
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return results;
        }

        public Country GetCountryById(int id)
        {
            var country = new Country();

            var conn = GetConnection();
            try
            {
                conn.Open();

                var query = $"SELECT * FROM country WHERE countryId = {id}";
                var cmd = new MySqlCommand(query, conn);
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    country.CountryId = int.Parse(rdr[0].ToString()); // id
                    country.CountryName = rdr[1].ToString(); // name
                    country.CreateDate = DateTime.ParseExact(rdr[2].ToString(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    country.CreatedBy = rdr[3].ToString();
                    country.LastUpdate = DateTime.ParseExact(rdr[4].ToString(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    country.LastUpdateBy = rdr[5].ToString();
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return country;
        }

        public Country AddCountry(Country country, string user)
        {
            var conn = GetConnection();
            try
            {
                var countries = GetAllCountries();
                country.CountryId = countries.Count == 0 ? 1 : countries.Last().CountryId + 1;
                var date = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                var query = $"INSERT INTO country VALUES ({country.CountryId}, '{country.CountryName}', '{date}', '{user}', '{date}', '{user}')";

                Console.WriteLine($"Executing Query:\n{query}");

                var cmd = new MySqlCommand(query, conn);
                var result = cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return country;
        }

        // cities
        public List<City> GetAllCities()
        {
            var results = new List<City>();

            var conn = GetConnection();
            try
            {
                conn.Open();

                var query = "SELECT * FROM city";
                var cmd = new MySqlCommand(query, conn);
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    results.Add(new City(
                        int.Parse(rdr[0].ToString()), // id
                        rdr[1].ToString(), // name
                        int.Parse(rdr[2].ToString()), // countryId
                        DateTime.ParseExact(rdr[3].ToString(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                        rdr[4].ToString(),
                        DateTime.ParseExact(rdr[5].ToString(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                        rdr[6].ToString())
                    );
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return results;
        }

        public City GetCityById(int id)
        {
            var city = new City();

            var conn = GetConnection();
            try
            {
                conn.Open();

                var query = $"SELECT * FROM city WHERE cityId = {id}";
                var cmd = new MySqlCommand(query, conn);
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    city.CityId = int.Parse(rdr[0].ToString()); // id
                    city.CityName = rdr[1].ToString(); // name
                    city.CountryId = int.Parse(rdr[2].ToString()); // countryId
                    city.CreateDate = DateTime.ParseExact(rdr[3].ToString(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    city.CreatedBy = rdr[4].ToString();
                    city.LastUpdate = DateTime.ParseExact(rdr[5].ToString(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    city.LastUpdateBy = rdr[6].ToString();
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return city;
        }

        public City AddCity(City city, string user)
        {
            var conn = GetConnection();
            try
            {
                var cities = GetAllCities();
                city.CityId = cities.Count == 0 ? 1 : cities.Last().CityId + 1;
                var date = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                var query = $"INSERT INTO city VALUES ({city.CityId}, '{city.CityName}', {city.CountryId}, '{date}', '{user}', '{date}', '{user}')";

                Console.WriteLine($"Executing Query:\n{query}");

                var cmd = new MySqlCommand(query, conn);
                var result = cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return city;
        }

        // addresses
        public List<Address> GetAllAddresses()
        {
            var results = new List<Address>();

            var conn = GetConnection();
            try
            {
                conn.Open();

                var query = "SELECT * FROM address";
                var cmd = new MySqlCommand(query, conn);
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    results.Add(new Address(
                        int.Parse(rdr[0].ToString()), // id
                        rdr[1].ToString(), // name
                        int.Parse(rdr[2].ToString()), // cityId
                        rdr[3].ToString(), // postal code
                        rdr[4].ToString(), // phone
                        DateTime.ParseExact(rdr[5].ToString(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                        rdr[6].ToString(),
                        DateTime.ParseExact(rdr[7].ToString(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                        rdr[8].ToString())
                    );
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return results;
        }

        public Address GetAddressById(int id)
        {
            var address = new Address();

            var conn = GetConnection();
            try
            {
                conn.Open();

                var query = $"SELECT * FROM address WHERE addressId = {id}";
                var cmd = new MySqlCommand(query, conn);
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    int.Parse(rdr[0].ToString()); // id
                    rdr[1].ToString(); // name
                    int.Parse(rdr[2].ToString()); // cityId
                    rdr[3].ToString(); // postal code
                    rdr[4].ToString(); // phone
                    DateTime.ParseExact(rdr[5].ToString(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    rdr[6].ToString();
                    DateTime.ParseExact(rdr[7].ToString(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    rdr[8].ToString();
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return address;
        }

        public Address AddAddress(Address address, string user)
        {
            var conn = GetConnection();
            try
            {
                var addresses = GetAllAddresses();
                address.AddressId = addresses.Count == 0 ? 1 : addresses.Last().AddressId + 1;
                var date = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                var query = $"INSERT INTO address VALUES ({address.AddressId}, '{address.AddressName}', '{address.Address2}', {address.CityId}, '{address.PostalCode}', '{address.Phone}', '{date}', '{user}', '{date}', '{user}')";

                Console.WriteLine($"Executing Query:\n{query}");

                var cmd = new MySqlCommand(query, conn);
                var result = cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return address;
        }

        // customers
        public List<Customer> GetAllCustomers()
        {
            var results = new List<Customer>();

            var conn = GetConnection();
            try
            {
                conn.Open();

                var query = "SELECT * FROM customer";
                var cmd = new MySqlCommand(query, conn);
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    results.Add(new Customer(
                        int.Parse(rdr[0].ToString()), // id
                        rdr[1].ToString(), // name
                        int.Parse(rdr[2].ToString()), // address id
                        int.Parse(rdr[3].ToString()), // active (0, 1)
                        DateTime.ParseExact(rdr[4].ToString(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                        rdr[5].ToString(),
                        DateTime.ParseExact(rdr[6].ToString(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                        rdr[7].ToString())
                    );
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return results;
        }

        public Customer GetCustomerById(int id)
        {
            var customer = new Customer();
            var conn = GetConnection();

            try
            {
                conn.Open();

                var query = $"SELECT * FROM customer WHERE customerId = {id}";
                var cmd = new MySqlCommand(query, conn);
                var rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    customer.Id = int.Parse(rdr[0].ToString()); // id
                    customer.CustomerName = rdr[1].ToString(); // name
                    customer.AddressId = int.Parse(rdr[2].ToString()); // address id
                    customer.Active = int.Parse(rdr[3].ToString()); // active (0, 1)
                    customer.CreateDate = DateTime.ParseExact(rdr[4].ToString(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    customer.CreatedBy = rdr[5].ToString();
                    customer.LastUpdate = DateTime.ParseExact(rdr[6].ToString(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    customer.LastUpdateBy = rdr[7].ToString();
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return customer;
        }

        public Customer AddCustomer(Customer customer, Address address, City city, Country country, string user)
        {
            var conn = GetConnection();
            try
            {
                // a country name is globally unique, so checking by name should only ever find one unique result, or nothing
                var dbCountry = GetAllCountries().Where(c => c.CountryName == country.CountryName).FirstOrDefault();
                dbCountry ??= AddCountry(country, user);

                // city names can be duplicated once per country (in this implementation), so if one if found, it must have an associated countryId
                // the city name and country id will create a unique composite key
                var dbCity = GetAllCities().Where(c => c.CityName == city.CityName).Where(c => c.CountryId == dbCountry.CountryId).FirstOrDefault();
                dbCity ??= AddCity(city, user);

                // address is the same as city above. A composite key of address name and city id will be unique
                var dbAddress = GetAllAddresses().Where(a => a.AddressName == address.AddressName).Where(a => a.CityId == dbCity.CityId).FirstOrDefault();
                dbAddress ??= AddAddress(address, user);

                // customer will be the same as address, with a composite key of name and addressId
                var dbCustomer = GetAllCustomers().Where(c => c.CustomerName == customer.CustomerName).Where(c => c.AddressId == dbAddress.AddressId).FirstOrDefault();
                if (dbCustomer != null)
                {
                    throw new DuplicateRecordException("Customer is already present in the database:\n" +
                        $"Name: {customer.CustomerName}\nPhone: {address.Phone}\nAddress: {address.AddressName}\nCity: {city.CityName}\n" +
                        $"Zip: {address.PostalCode}\nCountry: {country.CountryName}");
                }

                var customers = GetAllCustomers();
                customer.Id = customers.Count == 0 ? 1 : customers.Last().Id + 1;
                var date = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                var query = $"INSERT INTO customer VALUES ({customer.Id}, '{customer.CustomerName}', {customer.AddressId}, {customer.Active}, '{date}', '{user}', '{date}', '{user}')";

                Console.WriteLine($"Executing Query:\n{query}");

                var cmd = new MySqlCommand(query, conn);
                var result = cmd.ExecuteNonQuery();

            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return customer;
        }

        public Customer UpdateCustomer(Customer updatedCustomer, Customer originalCustomer)
        {
            var conn = GetConnection();
            try
            {
                conn.Open();

                // any part of customer could have changed, from name to address to city to country
                // this logic should go into FormScheduling instead. These update functions will be for pure updates
                // use bit flags to check if country, city, or address changed, as those will need changes in the classes that rely on those keys
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        public List<string> GetConsultants()
        {
            var conn = GetConnection();

            var results = new List<string>();
            try
            {
                conn.Open();
                
                var query = "SELECT userName FROM user";
                var cmd = new MySqlCommand(query, conn);
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    results.Add(rdr[0].ToString());
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return results;
        }

        public BindingList<Appointment> GetAppointments(DateTime startDate, DateTime endDate)
        {
            var appts = new BindingList<Appointment>();

            var conn = GetConnection();

            try
            {
                conn.Open();

                var query = $"SELECT appointmentId, customerId, userId, type, start, end, createDate, createdBy, lastUpdate, lastUpdateBy FROM appointment\n" +
                             $"WHERE start BETWEEN '{startDate:yyyy-MM-dd}' AND DATE_ADD('{endDate:yyyy-MM-dd}', INTERVAL 1 DAY)";
                Console.WriteLine($"Executing query: {query}");
                var cmd = new MySqlCommand(query, conn);
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    appts.Add(new Appointment
                    {
                        AppointmentId = int.Parse(rdr[0].ToString()),
                        CustomerId = int.Parse(rdr[1].ToString()),
                        UserId = int.Parse(rdr[2].ToString()),
                        Type = rdr[3].ToString(),
                        Start = DateTime.Parse(rdr[4].ToString()).ToLocalTime(),
                        End = DateTime.Parse(rdr[5].ToString()).ToLocalTime(),
                        CreateDate = DateTime.Parse(rdr[6].ToString()).ToLocalTime(),
                        CreatedBy = rdr[7].ToString(),
                        LastUpdate = DateTime.Parse(rdr[8].ToString()).ToLocalTime(),
                        LastUpdateBy = rdr[9].ToString()
                    }) ;
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            Console.WriteLine($"Got {appts.Count} appointments");
            return appts;
        }

        public BindingList<Appointment> GetAppointments(DateTime startDate, DateTime endDate, int consultantId)
        {
            var appts = new BindingList<Appointment>();

            var conn = GetConnection();

            try
            {
                conn.Open();

                var query = $"SELECT appointmentId, customerId, userId, type, start, end, createDate, createdBy, lastUpdate, lastUpdateBy FROM appointment\n" +
                             $"WHERE start BETWEEN '{startDate:yyyy-MM-dd}' AND DATE_ADD('{endDate:yyyy-MM-dd}', INTERVAL 1 DAY)";
                Console.WriteLine($"Executing query: {query}");
                var cmd = new MySqlCommand(query, conn);
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    appts.Add(new Appointment
                    {
                        AppointmentId = int.Parse(rdr[0].ToString()),
                        CustomerId = int.Parse(rdr[1].ToString()),
                        UserId = int.Parse(rdr[2].ToString()),
                        Type = rdr[3].ToString(),
                        Start = DateTime.Parse(rdr[4].ToString()).ToLocalTime(),
                        End = DateTime.Parse(rdr[5].ToString()).ToLocalTime(),
                        CreateDate = DateTime.Parse(rdr[6].ToString()).ToLocalTime(),
                        CreatedBy = rdr[7].ToString(),
                        LastUpdate = DateTime.Parse(rdr[8].ToString()).ToLocalTime(),
                        LastUpdateBy = rdr[9].ToString()
                    });
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            Console.WriteLine($"Got {appts.Count} appointments");
            return appts;
        }

        public List<Appointment> GetAppointmentsById(int? customerId)
        {
            if (customerId == null)
            {
                throw new ArgumentNullException("customerId", "customerId cannot be null.");
            }
            var appts = new List<Appointment>();

            var conn = GetConnection();

            try
            {
                conn.Open();

                var query = $"SELECT appointmentId, customerId, userId, type, start, end, createDate, createdBy, lastUpdate, lastUpdateBy FROM appointment\n" +
                             $"WHERE customerId = {customerId}";
                Console.WriteLine($"Executing query: {query}");
                var cmd = new MySqlCommand(query, conn);
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    appts.Add(new Appointment
                    {
                        AppointmentId = int.Parse(rdr[0].ToString()),
                        CustomerId = int.Parse(rdr[1].ToString()),
                        UserId = int.Parse(rdr[2].ToString()),
                        Type = rdr[3].ToString(),
                        Start = DateTime.Parse(rdr[4].ToString()).ToLocalTime(),
                        End = DateTime.Parse(rdr[5].ToString()).ToLocalTime(),
                        CreateDate = DateTime.Parse(rdr[6].ToString()).ToLocalTime(),
                        CreatedBy = rdr[7].ToString(),
                        LastUpdate = DateTime.Parse(rdr[8].ToString()).ToLocalTime(),
                        LastUpdateBy = rdr[9].ToString()
                    });
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            Console.WriteLine($"Got {appts.Count} appointments");
            return appts;
        }

        public void DeleteAppointmentById(int apptId)
        {
            var conn = GetConnection();

            try
            {
                conn.Open();

                var query = $"DELETE FROM appointment WHERE appointmentId = {apptId}";
                Console.WriteLine($"Executing query: {query}");
                var cmd = new MySqlCommand(query, conn);
                var result = cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            Console.WriteLine($"Deleted appointment with ID {apptId}");
        }

        public void DeleteCustomerById(int? custId)
        {
            if (custId == null)
            {
                throw new ArgumentNullException("custId", "custId cannot be null.");
            }
            var conn = GetConnection();

            try
            {
                conn.Open();

                var query = $"DELETE FROM customer WHERE customerId = {custId}";
                Console.WriteLine($"Executing query: {query}");
                var cmd = new MySqlCommand(query, conn);
                var result = cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            Console.WriteLine($"Deleted customer with ID {custId}");
        }


        public void GetAppointmentsByConsultantId(int consultantId)
        {

        }
    }
}
