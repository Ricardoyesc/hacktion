namespace hacktion
{
    public class Trip
    {
        public string name { get; set; }
        public decimal flightPrice { get; set; }
        public List<Accommodations> accommodations { get; set; }
        public List<Place> places { get; set; }
    }
}
