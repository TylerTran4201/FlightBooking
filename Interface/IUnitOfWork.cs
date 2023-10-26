namespace FlightBooking.Interface
{
    public interface IUnitOfWork
    {
        IAirportRepository airportRepository {get;}
        Task<bool> Complete();
        bool HasChanges();
    }
}