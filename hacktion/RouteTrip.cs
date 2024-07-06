namespace hacktion
{
    public class RouteTrip
    {
        public decimal TotalCost
        {
            get
            {
                decimal total = 0;
                foreach (var segment in Segments)
                {
                    total += segment.price;
                }
                return total;
            }
        }

        public List<Segment> Segments { get; set; } = new List<Segment>();

    }
}
