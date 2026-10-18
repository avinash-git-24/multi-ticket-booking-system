namespace TicketBooking.Web.ViewModels.Events;

using System.ComponentModel.DataAnnotations;
using TicketBooking.Web.Models;

public class EventListViewModel
{
    public IReadOnlyList<EventCardDto> Events { get; set; } = [];
    public string? SelectedCategory { get; set; }
    public string? SelectedCity { get; set; }
    public string? SearchQuery { get; set; }
    public IReadOnlyList<string> Categories { get; set; } = [];
    public IReadOnlyList<string> Cities { get; set; } = [];
}

public class EventDetailsViewModel
{
    public EventDetailsDto Event { get; set; } = new();
    
    [Required(ErrorMessage = "Please select a ticket tier.")]
    public int SelectedCategoryId { get; set; }

    [Required(ErrorMessage = "Please select at least 1 ticket.")]
    [Range(1, 10, ErrorMessage = "You can book between 1 and 10 tickets per transaction.")]
    public int SelectedQuantity { get; set; } = 1;

    public int? SelectedSessionId { get; set; }

    public string? ErrorMessage { get; set; }
}

public class HoldEventTicketsRequest
{
    [Required]
    public int EventId { get; set; }

    [Required]
    public int CategoryId { get; set; }

    [Required]
    [Range(1, 10)]
    public int Quantity { get; set; }

    public int? SessionId { get; set; }
}
