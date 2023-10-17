    namespace ObservationCSharp.Models
    {
        public class Observation
        {
            public int ID { get; set; }
            public string Media_Namn { get; set; }
            public short Kvalite { get; set; }
            public DateTime Datum { get; set; }
            public int Grad { get; set; }
            public int Sakerhet { get; set; }
        }
    }
