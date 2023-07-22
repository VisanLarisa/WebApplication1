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
    public class ContinentController : Controller
    {
        public string connectionString = "server=localhost; user=root; database=world2; port=3306; password=Oracle!Password19";

        public ContinentController()
        {
        }

        public List<Continent> selectAll()
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
                    continents.Add(continent);
                }
                reader.Close();
            }
            return continents;
        }

        public IActionResult Index()
        {
            List<Continent> continents = selectAll();
            return View("Index", continents);
        }

        public ActionResult AddContinent()
        {
            return View();
        }

        public IActionResult EditContinent(string code, string name)
        {
            Continent continent = new Continent();
            continent.Code = code;
            continent.Name = name;
            return View(continent);
        }
        public IActionResult FinishContinent()
        {
            string code = Request.Form["code"].ToString();
            string name = Request.Form["name"].ToString();

            List<Continent> continents = selectAll();
            int exist = 0;
            foreach(Continent c in continents)
            {
                if(c.Code==code)
                {
                    exist = 1; break; 
                }    
            }
            if(exist==0 && code.Length<=3 && code.Length>0)
            {
                //insert data into table //try
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    con.Open();
                    string insertCommand = "insert into continent (Code, Name) values ('" + code + "', '" + name + "')";
                    MySqlCommand cmdInsert = new MySqlCommand(insertCommand, con);
                    cmdInsert.ExecuteNonQuery();
                    TempData["AlertSuccessMessage"] = "Continent " + name + " Saved Successfully!";
                    con.Close();
                }           
            }
            else { 
                //raise pk violation error
                TempData["AlertFailedMessage"] = "Continent "+ code +" already exists!";
                 }
            return Index();
        }

        public string getContinent(string code)
        {
            string name = "";
            List<Continent> continents = selectAll();
            foreach (Continent c in continents)
                if (code == c.Code)
                    name = c.Name;
            return name;
        }

        public IActionResult FinishUpdateContinent()
        {
            string oldCode = Request.Form["oldCode"].ToString();
            string code = Request.Form["code"].ToString();
            string name = Request.Form["name"].ToString();

            List<Continent> continents = selectAll();
            int exists = 0;
            foreach (Continent c in continents)
            {
                if (c.Code == code)
                {
                    exists++;
                    if(exists>1)
                        break;
                }
            }

            if (exists < 2)
            {
                if (code.Length <= 3 && code.Length > 0)
                {
                    using (MySqlConnection con = new MySqlConnection(connectionString))
                    {
                        con.Open();
                        string command = "update continent set Code='" + code + "', Name='" + name + "' where Code='" + oldCode + "'";
                        MySqlCommand cmd = new MySqlCommand(command, con);
                        cmd.ExecuteNonQuery();
                        string cascadeCommand = "update country set Continent='" + getContinent(code)+"' where ContinentCode='" + code + "'";
                        MySqlCommand cmdCascade = new MySqlCommand(cascadeCommand, con);
                        cmdCascade.ExecuteNonQuery();
                        TempData["AlertSuccessMessage"] = "Continent " + name + " updated Successfully!";
                        con.Close();
                    }
                }
                else
                {
                    //raise pk violation error
                    TempData["AlertFailedMessage"] = "The length of the code is not valid (1-3 ch)!";
                }
            }
            else
            {
                TempData["AlertFailedMessage"] = "Continent " + code + " already exists!";
            }
            return Index();
        }


        public IActionResult DeleteContinent(string continent)
        {
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();

                string command = "delete from continent where Code = '" + continent + "'";
                MySqlCommand cmd = new MySqlCommand(command, con);
                cmd.ExecuteNonQuery();
                TempData["AlertSuccessMessage"] = "Continent " + continent + " Deleted Successfully!";
                con.Close();
            }
            return Index();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
