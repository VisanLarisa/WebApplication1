using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class CountryController : Controller
    {
        public string connectionString = "server=localhost; user=root; database=world2; port=3306; password=Oracle!Password19";


        public CountryController()
        {
        }

        public IActionResult Index()
        {
            List<Country> countries = selectAll();
            return View("Index", countries);
        }

        public List<Country> selectAll()
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

        public List<Continent> selectAllContinenents()
        {
            List<Continent> continents = new List<Continent>();
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand("select * from continent", con);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    //Extract / fetch my data
                    Continent continent = new Continent();
                    continent.Code = reader["Code"].ToString();
                    continent.Name = reader["Name"].ToString();

                    //also extract the countries
                    //continent.countries=

                    continents.Add(continent);
                }
                reader.Close();
            }
            return continents;
        }


        public ActionResult AddCountry()
        {
            List<Continent> continents = selectAllContinenents();
            List<string> continentCodes = new List<string>();
            foreach (Continent c in continents)
                continentCodes.Add(c.Code);
            return View(continentCodes);
        }

        public ActionResult EditCountry(string Code, string Name, string Continent, string Region, float SurfaceArea, int IndepYear, int Population, float LifeExpectancy, float GNP, float GNPOId, string localName, string GovernmentForm, string HeadOfState, int Capital, string Code2, string ContinentCode)
        {
            Country countryToUpdate = new Country();
            countryToUpdate.Code = Code;
            countryToUpdate.Name = Name;
            countryToUpdate.Continent = Continent;
            countryToUpdate.Region = Region;
            countryToUpdate.SurfaceArea = SurfaceArea;
            countryToUpdate.IndepYear = IndepYear;
            countryToUpdate.Population = Population;
            countryToUpdate.LifeExpectency = LifeExpectancy;
            countryToUpdate.GNP = GNP;
            countryToUpdate.GNPOId = GNPOId;
            countryToUpdate.LocalName = localName;
            countryToUpdate.GovernmentForm = GovernmentForm;
            countryToUpdate.HeadOfState = HeadOfState;
            countryToUpdate.Capital = Capital;
            countryToUpdate.Code2 = Code2;
            countryToUpdate.ContinentCode = ContinentCode;
            
            List<Continent> continents = selectAllContinenents();
            List<string> continentCodes = new List<string>();
            foreach (Continent c in continents)
                continentCodes.Add(c.Code);
            EditHelper<string, Country> lists = new EditHelper<string, Country>();
            lists.fkList = continentCodes;
            lists.editableObj = countryToUpdate;
            return View(lists);
        }

        public bool fkValidation(string continentCode)
        {          
            bool isFound = false;
            List<Continent> continents = selectAllContinenents();
            foreach(Continent c in continents)
            {
                if (c.Code == continentCode)
                {
                    isFound = true;
                    break;
                }
            }
            return isFound;
        }

        public bool pkValidation(string countryCode)
        {
            bool countryValid = true;
            if (countryCode.Length <= 3 && countryCode.Length > 0)
            {
                List<Country> countries = selectAll();
                foreach (Country c in countries)
                {
                    if (c.Code == countryCode)
                    {
                        countryValid = false; break;
                    }
                }
            }
            else countryValid = false;
            return countryValid;
        }

        public string getContinent(string code)
        {
            string name = "";
            List<Continent> continents = selectAllContinenents();
            foreach (Continent c in continents)
                if (code == c.Code)
                    name = c.Name;
            return name;
        }

        public int countryCodeUpdateValidation(string code)
        {
            int exists = 0;
            List<Country> countries = selectAll();
            foreach (Country c in countries)
            {
                if (c.Code == code)
                {
                    exists++;
                    if (exists > 1)
                        break;
                }
            }
            return exists;
        }
        public int countryNameUpdateValidation( string name)
        {
            int exists = 0;
            List<Country> countries = selectAll();
            foreach (Country c in countries)
            {
                if (c.Name == name)
                {
                    exists++;
                    if (exists > 1)
                        break;
                }
            }
            return exists;
        }

        public IActionResult FinishCountry()
        {
            try
            {
                string code = Request.Form["Code"].ToString();
                string name = Request.Form["Name"].ToString();
                string region = Request.Form["Region"].ToString();
                float surfaceArea = (float)Convert.ToDouble(Request.Form["SurfaceArea"]);       //eroare la null input
                int indepYear = Convert.ToInt32(Request.Form["IndepYear"]);
                int population = Convert.ToInt32(Request.Form["Population"]);
                float lifeExpectency = (float)Convert.ToDouble(Request.Form["LifeExpectency"]);
                string gnp = Request.Form["GNP"].ToString();
                float GNP = 0;
                if (gnp != "")
                { GNP = (float)Convert.ToDouble(gnp); }
                string gnpoid = Request.Form["GNPOId"].ToString();
                float GNPOId = 0;
                if (gnpoid != "")
                { GNPOId = (float)Convert.ToDouble(gnpoid); }
                string localName = Request.Form["LocalName"].ToString();
                string governmentForm = Request.Form["GovernmentForm"].ToString();
                string headOfState = Request.Form["HeadOfState"].ToString();
                string Capital = Request.Form["Capital"].ToString();
                int capital = 0;
                if (Capital != "")
                    capital = Convert.ToInt32(Capital);
                string code2 = Request.Form["Code2"].ToString();
                string continentCode = Request.Form["Continents"].ToString();
                string continent = getContinent(continentCode);

                if (pkValidation(code))    
                {
                    if (fkValidation(continentCode))  
                        using (MySqlConnection con = new MySqlConnection(connectionString))
                        {
                            con.Open();
                            string insertCommand = "insert into country (Code, Name, Continent, Region, SurfaceArea, IndepYear, Population, LifeExpectancy, GNP, GNPOld, LocalName, GovernmentForm, HeadOfState, Capital, Code2, ContinentCode) values ('" + code + "', '" + name + "', '" + continent + "', '" + region + "', '" + surfaceArea + "', '" + indepYear + "', '" + population + "', '" + lifeExpectency + "', '" + GNP + "', '" + GNPOId + "', '" + localName + "', '" + governmentForm + "', '" + headOfState + "', '" + capital + "', '" + code2 + "', '" + continentCode + "')";
                            MySqlCommand cmdInsert = new MySqlCommand(insertCommand, con);
                            cmdInsert.ExecuteNonQuery();
                            TempData["AlertSuccessMessage"] = "Country " + code + " successfuly added!";
                        }
                }
                else
                { //raise pk violation error
                    TempData["AlertFailedMessage"] = "Country " + code + " already exists!";                    
                }

            }
            catch
            {
                TempData["AlertFailedMessage"] = " Please complete all the mandatory fields correctly! ";
                //return AddCountry();
            }
            return Index();
        }

        public IActionResult FinishUpdateCountry()
        {
            try
            {
                string oldCode = Request.Form["oldCode"].ToString();
                string code = Request.Form["Code"].ToString();
                string name = Request.Form["Name"].ToString();
                string region = Request.Form["Region"].ToString();
                float surfaceArea = (float)Convert.ToDouble(Request.Form["SurfaceArea"]);
                int indepYear = Convert.ToInt32(Request.Form["IndepYear"]);
                int population = Convert.ToInt32(Request.Form["Population"]);
                float lifeExpectency = (float)Convert.ToDouble(Request.Form["LifeExpectency"]);
                string gnp = Request.Form["GNP"].ToString();
                float GNP = 0;
                if (gnp != "")
                { GNP = (float)Convert.ToDouble(gnp); }
                string gnpoid = Request.Form["GNPOId"].ToString();
                float GNPOId = 0;
                if (gnpoid != "")
                { GNPOId = (float)Convert.ToDouble(gnpoid); }
                string localName = Request.Form["LocalName"].ToString();
                string governmentForm = Request.Form["GovernmentForm"].ToString();
                string headOfState = Request.Form["HeadOfState"].ToString();
                string Capital = Request.Form["Capital"].ToString();
                int capital = 0;
                if (Capital != "")
                    capital = Convert.ToInt32(Capital);
                string code2 = Request.Form["Code2"].ToString();
                string continentCode = Request.Form["Continents"].ToString();   //choose from dropdownlist
                string continent = getContinent(continentCode);

                if (countryCodeUpdateValidation(code)<2)
                {
                    if (countryNameUpdateValidation(code) < 2)
                    {
                        if (fkValidation(continentCode))
                            using (MySqlConnection con = new MySqlConnection(connectionString))
                            {
                                con.Open();
                                string updateCommand = "update country set Code='"+code+"',Name='"+name+"', Continent='" + continent + "', Region='" + region + "', SurfaceArea=" + surfaceArea + ", IndepYear=" + indepYear + ", Population=" + population + ", LifeExpectancy=" + lifeExpectency + ", GNP=" + gnp + ", GNPOld=" + gnpoid + ", LocalName='" + localName + "', GovernmentForm='" + governmentForm + "', HeadOfState='" + headOfState + "', Capital=" + capital + ", Code2='" + code2 + "', ContinentCode='" + continentCode + "' where Code='" + oldCode + "'";
                                MySqlCommand cmdInsert = new MySqlCommand(updateCommand, con);
                                cmdInsert.ExecuteNonQuery();
                                TempData["AlertSuccessMessage"] = "Country " + code + " successfuly updated!";
                            }
                        else
                        {
                            TempData["AlertFailedMessage"] = " Please complete all the fields correctly!";
                        }
                    }
                    else
                    {
                        TempData["AlertFailedMessage"] = "Country with name " + name + " already exists!";
                    }
                }
                else
                {
                    TempData["AlertFailedMessage"] = "Country with code" + code + " already exists!";
                }

            }
            catch
            {
                TempData["AlertFailedMessage"] = " Please complete all the mandatory fields correctly! ";
            }
            return Index();
        }

        public IActionResult DeleteCountry(string country)
        {
            try { 
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();
                string command = "delete from country where Code = '" + country + "'";
                MySqlCommand cmd = new MySqlCommand(command, con);
                cmd.ExecuteNonQuery();
                TempData["AlertSuccessMessage"] = " Country "+ country+" deleted! ";
                con.Close();
            }
            }
            catch
            {
                TempData["AlertFailedMessage"] = " An error occured and stopped the deletion.";
            }
            return Index();
        }

        public ActionResult DisplayCountriesByContinent(string continentCode)
        {
            List<Country> countries = new List<Country>();

            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();
                string command = "select * from country where ContinentCode = '" + continentCode + "'";
                MySqlCommand cmd = new MySqlCommand(command, con);
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


                    countries.Add(country);
                }
                reader.Close();
            }
            return View("Index", countries);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
