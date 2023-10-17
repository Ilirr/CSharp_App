using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySql.Data.MySqlClient;
using ObservationCSharp.DatabaseConnect;
using ObservationCSharp.Models;
using System.Data;

namespace ObservationCSharp.Controllers
{
    public class ProcedureController : Controller
    {
        private readonly ILogger<ProcedureController> _logger;

        public ProcedureController(ILogger<ProcedureController> logger)
        {
            _logger = logger;
        }


        [HttpPost]
        public ActionResult Index(ViewModel model)
        {
            _logger.LogWarning("THIS HAPPENED");
            try
            {
                List<Observation> observations = new List<Observation>();

                observations = ExecuteProcedure(model.SelectedProcedure);


                return View("~/Views/Observation/ObservationList.cshtml", observations);

            }
            catch(Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }
           
        }
        private List<Observation> ExecuteProcedure(string procedureName)
        {
            _logger.LogWarning("THIS HAPPENED");

            List<Observation> observations = new List<Observation>();
            using (MySqlConnection conn = DBConnecter.ConnectDatabase())
            {

                using (MySqlCommand cmd = new MySqlCommand(procedureName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Observation observation = new Observation
                            {
                                ID = Convert.ToInt32(reader["id"]),
                                Media_Namn = reader["media_namn"].ToString(),
                                Kvalite = reader.GetInt16("kvalite"),
                                Datum = Convert.ToDateTime(reader["datum"]),
                                Grad = Convert.ToInt32(reader["grad"]),
                                Sakerhet = Convert.ToInt32(reader["säkerhet"])

                            };
                            observations.Add(observation);
                        }
                    }
                }

            }
            return observations;
        }

    }
}
