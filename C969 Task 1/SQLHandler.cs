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

namespace C969_Task_1
{
    public class SQLHandler
    {
        private Translator LocalTranslator = new Translator();

        public void TestLogin(string username, string password, Translator.LanguageCode languageCode)
        {
            string connStr = ConfigurationManager.ConnectionStrings["RQLDEV01"].ConnectionString;
            var conn = new MySqlConnection(connStr);
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

        public List<Customer> GetCustomers()
        {
            var results = new List<Customer>();

            string connStr = ConfigurationManager.ConnectionStrings["RQLDEV01"].ConnectionString;
            var conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();

                var query = @"SELECT customer.customerId, customer.customerName, address.phone, address.address, city.city, address.postalCode, country.country FROM customer
                            INNER JOIN address ON customer.addressId = address.addressId
                            INNER JOIN city on city.cityId = address.cityId
                            INNER JOIN country on city.countryId = country.countryId";
                var cmd = new MySqlCommand(query, conn);
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    results.Add(new Customer(
                        int.Parse(rdr[0].ToString()),
                        rdr[1].ToString(),
                        rdr[2].ToString(),
                        rdr[3].ToString(),
                        rdr[4].ToString(),
                        int.Parse(rdr[5].ToString()),
                        rdr[6].ToString()));
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

        public void AddClient(string name, string phoneNumber, string address, string city, string zip, string country)
        {
            string connStr = ConfigurationManager.ConnectionStrings["RQLDEV01"].ConnectionString;
            var conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();

                // check for customer table duplication
                var query = @$"SELECT customer.customerName, address.phone, address.address, city.city, address.postalCode, country.country FROM customer
                            INNER JOIN address ON customer.addressId = address.addressId
                            INNER JOIN city on city.cityId = address.cityId
                            INNER JOIN country on city.countryId = country.countryId
                            WHERE customerName = '{name}'
                            AND address.phone = '{phoneNumber}'
                            AND address.address = '{address}'
                            AND city.city = '{city}'
                            AND address.postalCode = '{zip}'
                            AND country.country = '{country}'";
                var cmd = new MySqlCommand(query, conn);
                var rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    throw new DuplicateRecordException("Customer is already present in the database:\n" +
                                                      $"Name: {name}\nPhone: {phoneNumber}\nAddress: {address}\nCity: {city}\n" +
                                                      $"Zip: {zip}\nCountry: {country}");
                }

                // get the highest ID from the table
                query = "SELECT MAX(customerId) FROM customer";
                cmd = new MySqlCommand(query, conn);
                rdr = cmd.ExecuteReader();
                int id = 1; // initialize to something
                while (rdr.Read())
                {
                    id = int.Parse(rdr[0].ToString());
                }

                id++;

                query = $"INSERT INTO customer VALUES ('{id}, '{name}')";
                cmd = new MySqlCommand(query, conn);
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
            string connStr = ConfigurationManager.ConnectionStrings["RQLDEV01"].ConnectionString;
            var conn = new MySqlConnection(connStr);

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

            string connStr = ConfigurationManager.ConnectionStrings["RQLDEV01"].ConnectionString;
            var conn = new MySqlConnection(connStr);

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
                        Start = DateTime.Parse(rdr[4].ToString()),
                        End = DateTime.Parse(rdr[5].ToString()),
                        CreateDate = DateTime.Parse(rdr[6].ToString()),
                        CreatedBy = rdr[7].ToString(),
                        LastUpdate = DateTime.Parse(rdr[8].ToString()),
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
    }
}
