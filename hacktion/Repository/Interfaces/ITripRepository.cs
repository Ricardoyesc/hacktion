namespace hacktion.Repository.Interfaces
{
    public interface ITripRepository
    {
        public Trip? Get(string name);
        public Trip? Save(Trip name);
    }
}
