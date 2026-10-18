namespace TicketBooking.Web.ViewModels.Home;

using TicketBooking.Web.Models;

public class HomeIndexViewModel
{
    public IReadOnlyList<Movie> RecommendedMovies { get; set; } = [];
    public IReadOnlyList<Movie> PremiereMovies { get; set; } = [];
    public IReadOnlyList<EventCardDto> MusicEvents { get; set; } = [];
    public IReadOnlyList<EventCardDto> ComedyEvents { get; set; } = [];
    public IReadOnlyList<EventCardDto> SportsEvents { get; set; } = [];
    public IReadOnlyList<EventCardDto> TheatreEvents { get; set; } = [];
    public IReadOnlyList<EventCardDto> WorkshopEvents { get; set; } = [];
    public IReadOnlyList<EventCardDto> PopularEvents { get; set; } = [];
    public IReadOnlyList<EventCardDto> OutdoorEvents { get; set; } = [];
    public string SelectedCity { get; set; } = "Rajkot";
}
