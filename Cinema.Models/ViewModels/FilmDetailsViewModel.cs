using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cinema.Models.ViewModels
{
    public sealed class FilmDetailsViewModel
    {
        public Film Film { get; set; } = null!;

        public double Rating { get; set; } = 1.0;

        public int ShowId { get; set; } = 0;

        [ValidateNever]
        public IEnumerable<Show> Shows { get; set; } = null!;

        [ValidateNever]
        public IEnumerable<SelectListItem> Rooms { get; set; } = null!;
    }
}
