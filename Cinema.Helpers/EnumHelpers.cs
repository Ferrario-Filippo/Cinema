using Cinema.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Immutable;

namespace Cinema.Helpers
{
	public static class EnumHelpers
	{
		private static IImmutableDictionary<string, string> _genresITLocalization = new Dictionary<string, string>() 
		{
			{ "Action", "Azione" },
			{ "Adventure", "Avventura" },
			{ "Animation", "Animazione" },
			{ "Comedy", "Commedia" },
			{ "Crime", "Giallo" },
			{ "Drama", "Dramma" },
			{ "Erotic", "Erotico" },
			{ "Fantasy", "Fantasy" },
			{ "Heimat", "Heimat" },
			{ "History", "Storico" },
			{ "Horror", "Horror" },
			{ "Mystery", "Mistero" },
			{ "Romance", "Romatico" },
			{ "ScienceFiction", "Fantascienza" },
			{ "Thriller", "Thriller" },
			{ "War", "Guerra" },
			{ "Western", "Western" }
		}.ToImmutableDictionary();

		public static IEnumerable<SelectListItem> GetFilmGenres()
		{
			var genres = Enum.GetNames(typeof(FilmGenre));
			for(int i = 0; i < genres.Length; i++)
			{
				yield return new SelectListItem(
					_genresITLocalization[genres[i]], 
					genres[i]);
			}
		}

        private static IImmutableDictionary<string, string> _gendersITLocalization = new Dictionary<string, string>()
        {
            { "NonBinary", "Non binario" },
            { "Male", "Maschio" },
            { "Female", "Femmina" }
        }.ToImmutableDictionary();

        public static IEnumerable<SelectListItem> GetGenders()
        {
            var genders = Enum.GetNames(typeof(Gender));
            for (int i = 0; i < genders.Length; i++)
            {
                yield return new SelectListItem(
                    _gendersITLocalization[genders[i]],
                    genders[i]);
            }
        }
	}
}
