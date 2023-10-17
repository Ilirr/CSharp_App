using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using ObservationCSharp.DatabaseConnect;
using ObservationCSharp.Models;
using System.Data.Common;

namespace ObservationCSharp.Controllers
{
    public class ObservationController : Controller
    {

        [HttpPost]
        public ActionResult ObservationsByDate(ViewModel model)
        {
            try
            {
                List<Observation> observations = new List<Observation>();

                observations = GetAllObservationsBeforeDate(model.Observation.Datum);


                return View("~/Views/Observation/ObservationByDate.cshtml", observations);

            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }

        }


        [HttpGet]
        public ActionResult GetAllObservations()
        {
            List<Observation> observations = new List<Observation>();
            try
            {
                using (MySqlConnection connection = DBConnecter.ConnectDatabase())
                {
                    string query = "SELECT * FROM Observation";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Observation observation = new Observation
                                {
                                    ID = Convert.ToInt32(reader["id"]),
                                    Media_Namn = reader["media_namn"].ToString(),
                                    Kvalite = Convert.ToInt16(Request.Form["kvalite"]),
                                    Datum = Convert.ToDateTime(reader["datum"]),
                                    Grad = Convert.ToInt32(reader["grad"]),
                                    Sakerhet = Convert.ToInt32(reader["säkerhet"])
                                };
                                observations.Add(observation);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }

            return View("ObservationList", observations);
        }

        [HttpGet]
        public ActionResult GetObservationLinks()
        {
            List<Observation> observations = new List<Observation>();

            using (MySqlConnection connection = DBConnecter.ConnectDatabase())
            {
                string query = "SELECT id, säkerhet FROM Observation";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            observations.Add(new Observation
                            {
                                ID = Convert.ToInt32(reader["id"]),
                                Sakerhet = Convert.ToInt32(reader["säkerhet"])
                            });
                        }
                    }
                }
            }
            return View(observations);
        }





        [HttpPost]
        public ActionResult AddObservation(Observation observation)
        {
            try
            {
                using (MySqlConnection connection = DBConnecter.ConnectDatabase())
                {
                    string checkQuery = "SELECT COUNT(*) FROM Observation WHERE id = @id";
                    using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, connection))
                    {
                        checkCmd.Parameters.AddWithValue("@id", observation.ID);

                        if (Convert.ToInt32(checkCmd.ExecuteScalar()) > 0)
                        {
                            TempData["ErrorMessage"] = "Already exists an Observation with submitted ID";
                            return RedirectToAction("Index", "Home");
                        }
                    }

                    string insertQuery = "INSERT INTO Observation (id, media_namn, kvalite, datum, grad, säkerhet) VALUES (@id, @media_namn, @kvalite, @datum, @grad, @säkerhet)";
                    using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, connection))
                    {
                        insertCmd.Parameters.AddWithValue("@id", observation.ID);
                        insertCmd.Parameters.AddWithValue("@media_namn", observation.Media_Namn);
                        insertCmd.Parameters.AddWithValue("@kvalite", observation.Kvalite);
                        insertCmd.Parameters.AddWithValue("@säkerhet", observation.Sakerhet);
                        insertCmd.Parameters.AddWithValue("@datum", observation.Datum);
                        insertCmd.Parameters.AddWithValue("@grad", observation.Grad);

                        insertCmd.ExecuteNonQuery();
                    }
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.ToString();
                return RedirectToAction("Index", "Home");
            }
        }


        [HttpPost]
        public ActionResult Search(short kvalite)
        {
            List<Observation> observations = new List<Observation>();
            try
            {
                using (MySqlConnection connection = DBConnecter.ConnectDatabase())
                {
                    string query = "SELECT * FROM Observation WHERE kvalite = @kvalite";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@kvalite", kvalite);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Observation observation = new Observation
                                {

                                    ID = Convert.ToInt32(reader["id"]),
                                    Media_Namn = Convert.ToString(reader["media_namn"]),
                                    Kvalite = Convert.ToInt16(reader["kvalite"]),
                                    Datum = Convert.ToDateTime(reader["datum"]),
                                    Grad = Convert.ToInt32(reader["grad"]),
                                    Sakerhet = Convert.ToInt32(reader["säkerhet"])
                                };
                                observations.Add(observation);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }

            if (observations.Count == 0)
            {
                ViewBag.msg = $"No results found for Media Kvalite: {kvalite}";
            }

            return View("ObservationList", observations);
        }

        public ActionResult UpdateSecurity(int observationID)
        {

            using (MySqlConnection conn = DBConnecter.ConnectDatabase())
            {
                string query = "UPDATE Observation SET säkerhet = säkerhet + 10 WHERE id = @id";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", observationID);
                    try
                    {
                        cmd.ExecuteNonQuery();
                        ViewBag.Message = "Säkerheten har ändrats för observationen.";
                    }
                    catch (Exception e)
                    {
                        return ViewBag.Message = e.Message;
                    }
                }
                return RedirectToAction("Index", new { message = ViewBag.Message });
            }
        }

        private List<Observation> GetAllObservationsBeforeDate(DateTime date)
        {
            List<Observation> observations = new List<Observation>();
            using (MySqlConnection conn = DBConnecter.ConnectDatabase())
            {
                string query = "CALL Alla_Observationer_Innan_Datum(@datum)";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@datum", date);
                    try
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Observation observation = new Observation
                                {
                                    ID = Convert.ToInt32(reader["id"]),
                                    Media_Namn = reader["media_namn"].ToString(),
                                    Kvalite = Convert.ToInt16(reader["kvalite"]),
                                    Datum = Convert.ToDateTime(reader["datum"]),
                                    Grad = Convert.ToInt32(reader["grad"]),
                                    Sakerhet = Convert.ToInt32(reader["säkerhet"])
                                };
                                observations.Add(observation);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;


                    }
                }
            }
            return observations;
        }
    }


}
