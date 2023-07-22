using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        public string connectionString = "server=localhost; user=root; database=world2; port=3306; password=Oracle!Password19";
        private readonly ILogger<HomeController> _logger;
        public int ID=0;
        
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            currentId();
        }

        public void currentId()
        {
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand("select MAX(ID) AS id from City", con);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    this.ID = (Convert.ToInt32(reader["id"]))+1;
                reader.Close();
            }
        }


        public IActionResult Index()
        {
            List<City> cities = selectAll();
            return View("Index", cities);
        }

        public List<City> selectAll()
        {
             List<City> cities = new List<City>();

            //Connect to MySQL
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand("select * from city", con);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    //Extract / fetch my data
                    City city = new City();
                    city.Id = Convert.ToInt32(reader["ID"]);
                    city.Name = reader["Name"].ToString();
                    city.CountryCode = reader["CountryCode"].ToString();
                    city.District = reader["District"].ToString();
                    city.Population = Convert.ToInt32(reader["Population"]);

                    cities.Add(city);
                }
                reader.Close();
            }
            return cities;
        }

        public List<Country> selectAllCountries()
        {
            List<Country> countries = new List<Country>();

            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand("select * from country", con);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    //Extract / fetch my data
                    Country country = new Country();
                    country.Code = reader["Code"].ToString();
                    country.Name = reader["Name"].ToString();
                    country.Continent = reader["Continent"].ToString();
                    country.Region = reader["Region"].ToString();
                    country.SurfaceArea = (float)Convert.ToDouble(reader["SurfaceArea"]);
                    try { country.IndepYear = Convert.ToInt16(reader["IndepYear"]); }
                    catch { country.IndepYear = 0; }

                    country.Population = Convert.ToInt32(reader["Population"]);
                    try { country.LifeExpectency = (float)Convert.ToDouble(reader["LifeExpectancy"]); }
                    catch { country.LifeExpectency = 0; }
                    try { country.GNP = (float)Convert.ToDouble(reader["GNP"]); }
                    catch { country.GNP = 0; }
                    try { country.GNPOId = (float)Convert.ToDouble(reader["GNPOId"]); }
                    catch { country.GNPOId = 0; }
                    country.LocalName = reader["LocalName"].ToString();
                    country.GovernmentForm = reader["GovernmentForm"].ToString();
                    country.HeadOfState = reader["HeadOfState"].ToString();
                    try { country.Capital = Convert.ToInt32(reader["Capital"]); }
                    catch { country.Capital = 0; }
                    country.Code2 = reader["Code2"].ToString();
                    country.ContinentCode = reader["ContinentCode"].ToString();

                    //also extract the cities
                    //country.cities=

                    countries.Add(country);
                }
                reader.Close();
            }
            return countries;
        }

        public ActionResult AddCity()
        {
            List<Country> countries = selectAllCountries();
            List<string> countryCodes = new List<string>();
            foreach (Country c in countries)
                countryCodes.Add(c.Code);
            return View(countryCodes);
        }

        public ActionResult EditCity(int Id, string Name, string Population, string District, string CountryCode)
        {
            City cityToUpdate = new City();
            cityToUpdate.Id = Id;
            cityToUpdate.Name = Name;
            cityToUpdate.Population = Convert.ToInt32(Population);
            cityToUpdate.District = District;
            cityToUpdate.CountryCode = CountryCode;
            List<Country> countries = selectAllCountries();
            List<string> countryCodes = new List<string>();
            foreach (Country c in countries)
                countryCodes.Add(c.Code);
            EditHelper<string, City> lists = new EditHelper<string, City>();
            lists.fkList = countryCodes;
            lists.editableObj = cityToUpdate;
            return View(lists);
        }

        public bool fkValidation(string countryCode)
        {
            bool isFound = false;
            List<Country> countries = selectAllCountries();
            foreach (Country c in countries)
            {
                if (c.Code == countryCode)
                {
                    isFound = true;
                    break;
                }
            }
            return isFound;
        }

        public int cityValidation(string name, string district)
        {
            int cityValid = 1;
            if (name.Length > 0)
            {
                List<City> cities = selectAll();
                foreach (City c in cities)
                {
                    if (c.Name == name)
                    {
                        if (c.District == district)
                        {
                                cityValid = 0;
                                break;                          
                        }
                    }
                }
            }
            else cityValid = 0;
            return cityValid;
        }

        public int cityUpdateValidation(string name, string district, int id)
        {
            int cityValid = 1;
            if (name.Length > 0)
            {
                List<City> cities = selectAll();
                foreach (City c in cities)
                {
                    if (c.Name == name)
                    {
                        if (c.District == district)
                        {
                            if (c.Id != id)
                            {
                                cityValid = 0;
                                break;
                            }
                        }
                    }
                }
            }
            else cityValid = 0;
            return cityValid;
        }

        public IActionResult FinishCity()
        {
            string Name = Request.Form["Name"].ToString();
            string District = Request.Form["District"].ToString();
            string Population = Request.Form["Population"].ToString();
            string CountryCode = Request.Form["CountryCode"].ToString();

            if(cityValidation(Name, District)!=0)
            {
                if (fkValidation(CountryCode) && District!="" && Name!="" && Population!="")
                    using (MySqlConnection con = new MySqlConnection(connectionString))
                    {
                        con.Open();
                        string insertCommand = "insert into city (Id, Name, District, Population, CountryCode) values (" + this.ID + ", '" + Name + "', '" + District + "', '" + Population + "', '" + CountryCode + "')";
                        MySqlCommand cmdInsert = new MySqlCommand(insertCommand, con);
                        cmdInsert.ExecuteNonQuery();
                        TempData["AlertSuccessMessage"] = "City " + Name + " successfuly added!";
                    }
                else
                {
                    TempData["AlertFailedMessage"] = " Please complete all the fields!";
                }
            }
            else
            {
                TempData["AlertFailedMessage"] = " City " + Name + " already inserted!";
            }
            return Index();
        }

        public IActionResult FinishUpdateCity()
        {
            string Id = Request.Form["Id"].ToString();
            string Name = Request.Form["Name"].ToString();
            string District = Request.Form["District"].ToString();
            string Population = Request.Form["Population"].ToString();
            string CountryCode = Request.Form["CountryCode"].ToString();

            if (cityUpdateValidation(Name, District, Convert.ToInt32(Id))!=0)
            {
                if (fkValidation(CountryCode) && District != "" && Name != "" && Population != "")
                    using (MySqlConnection con = new MySqlConnection(connectionString))
                    {
                        con.Open();
                        string insertCommand = "update city set Name= '" + Name + "', District= '" + District + "', Population = '" + Population + "', CountryCode = '" + CountryCode + "' where ID = "+Id;
                        MySqlCommand cmdInsert = new MySqlCommand(insertCommand, con);
                        cmdInsert.ExecuteNonQuery();
                        TempData["AlertSuccessMessage"] = "City " + Name + " successfully updated!";
                    }
                else
                {
                    TempData["AlertFailedMessage"] = " Please complete all the fields correctly!";
                }
            }
            else
            {
                TempData["AlertFailedMessage"] = " City " + Name + " already exisits in "+District+" district!";
            }
            return Index();
        }

        public IActionResult DeleteCity(int Id)
        {
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string command = "delete from city where ID = " + Id;
                    MySqlCommand cmd = new MySqlCommand(command, con);
                    cmd.ExecuteNonQuery();
                    TempData["AlertSuccessMessage"] = " City deleted! ";
                    con.Close();
                }
                catch
                {
                    TempData["AlertFailedMessage"] = " An error occured and stopped the deletion.";
                }
            }
            return Index();
        }
        public ActionResult DisplayCitiesByCountry(string country)
        {
            List<City> cities = new List<City>();

            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();
                string command = "select * from city where CountryCode = '" + country+"'";
                MySqlCommand cmd = new MySqlCommand(command, con);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    //Extract / fetch my data
                    City city = new City();
                    city.Id = Convert.ToInt32(reader["ID"]);
                    city.Name = reader["Name"].ToString();
                    city.CountryCode = reader["CountryCode"].ToString();
                    city.District = reader["District"].ToString();
                    city.Population = Convert.ToInt32(reader["Population"]);

                    cities.Add(city);
                }
                reader.Close();
            }
            return View("Index", cities);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
