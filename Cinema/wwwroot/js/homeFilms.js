function onFilterClick() {
	const title = $('#titleFilter').val().toLowerCase();
	const genre = $('#genreFilter').val();

	const isTitleEmpty = title == "";
	const isGenreDefault = genre === "--Scegli un genere--";

	$.each($('.film-card'), function (i, item) {
		var id = item.id.toString().split('~');
		
		if ((id[0].includes(title) || isTitleEmpty) && (id[1] === genre || isGenreDefault)) {
			item.style.display = 'block';
		} else {
			item.style.display = 'none';
		}
	});
}
