using Microsoft.AspNetCore.Mvc.Rendering;

namespace ObservationCSharp.Models
{
    public class ViewModel
    {

        public Observation Observation { get; set; }

        public List<Observation> Observations { get; set; }
        public List<SelectListItem> Persons { get; set; }
        public List<PersonObservation> PersonObservations { get; set; }

        public string SelectedProcedure { get; set; }
        public List<SelectListItem> Procedures { get; set; }



        public ViewModel()
        {
            Procedures = new List<SelectListItem>
        {
            new SelectListItem { Value = "Alla_Observationer", Text = "Alla_Observationer" },
            new SelectListItem { Value = "Alla_Observationer_Vertikal", Text = "Alla_Observationer_Vertikal" }
        };
        }
    }
}
