using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySql.Data.MySqlClient;
using ObservationCSharp.DatabaseConnect;
using ObservationCSharp.Models;
using System.Data;
using System.Diagnostics;
using Microsoft.AspNetCore.Session;

namespace ObservationCSharp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Login(string username, string password)
        {
            using (MySqlConnection conn = DBConnecter.ConnectDatabase())
            {
                string query = "SELECT username FROM users WHERE username = @username AND password = @password";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);

                    object user = cmd.ExecuteScalar();

                    if (user != null)
                    {
                        if (user.ToString() == "OBSERVATÖR")
                        {
                            HttpContext.Session.SetString(SessionVariables.SessionKeyUsername, username);
                            HttpContext.Session.SetString(SessionVariables.SessionKeySessionId, Guid.NewGuid().ToString());

                            _logger.LogInformation("Logged in as OBSERVATÖR: {username}", username);

                            return RedirectToAction("Index");
                        }

                    }
                    _logger.LogWarning("Invalid login", username, password);
                    return RedirectToAction("MyPage");

                }
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            _logger.LogInformation("Username:{session}", HttpContext.Session.GetString(SessionVariables.SessionKeyUsername));
            _logger.LogInformation("SessionId:{session}", HttpContext.Session.GetString(SessionVariables.SessionKeySessionId));

            return RedirectToAction("MyPage");
        }


        public IActionResult MyPage()
        {
            ViewBag.Username = HttpContext.Session.GetString(SessionVariables.SessionKeyUsername);
            ViewBag.SessionId = HttpContext.Session.GetString(SessionVariables.SessionKeySessionId);

            _logger.LogInformation("Username:{session}", HttpContext.Session.GetString(SessionVariables.SessionKeyUsername));
            _logger.LogInformation("SessionId:{session}", HttpContext.Session.GetString(SessionVariables.SessionKeySessionId));
            ViewModel viewModel = new ViewModel();

            return View("MyPage", viewModel);
        }
        public IActionResult Index()
        {
            ViewBag.Username = HttpContext.Session.GetString(SessionVariables.SessionKeyUsername);
            ViewBag.SessionId = HttpContext.Session.GetString(SessionVariables.SessionKeySessionId);

            _logger.LogInformation("Username:{session}", HttpContext.Session.GetString(SessionVariables.SessionKeyUsername));
            _logger.LogInformation("SessionId:{session}", HttpContext.Session.GetString(SessionVariables.SessionKeySessionId));


            ViewModel viewModel = new ViewModel();


            using (MySqlConnection conn = DBConnecter.ConnectDatabase())
            {

                string query = "SELECT id, namn FROM Person";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    viewModel.Persons = new List<SelectListItem>();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            viewModel.Persons.Add(new SelectListItem
                            {
                                Value = reader["id"].ToString(),
                                Text = reader["namn"].ToString()
                            });
                        }
                    }

                }


                query = "SELECT * FROM Observation";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    viewModel.Observations = new List<Observation>();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Observation observation = new Observation
                            {
                                ID = reader.GetInt32("Id"),
                                Media_Namn = reader.GetString("media_namn"),
                                Kvalite = reader.GetInt16("kvalite"),
                                Datum = reader.GetDateTime("datum"),
                                Grad = reader.GetInt32("grad"),
                                Sakerhet = reader.GetInt32("säkerhet")
                            };
                            viewModel.Observations.Add(observation);
                        }
                    }

                }

                query = "SELECT * FROM Person_Observation";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    viewModel.PersonObservations = new List<PersonObservation>();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            viewModel.PersonObservations.Add(new PersonObservation
                            {
                                PersonId = reader.GetInt32("person_id"),
                                ObservationId = reader.GetInt32("observation_id")
                            });


                        }
                    }

                }
            }

            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}