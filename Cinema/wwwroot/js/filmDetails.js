function onFilterClick() {
	const date = new Date($('#dateFilter').val()).getTime();
	const room = $('#roomFilter').val();

	const isRoomDefault = room == "--Scegli una sala--";

	$.each($('.show-card'), function (i, item) {
		var id = item.id.toString().split('~');
		var cardDate = new Date(id[1]).getTime();

		if ((id[0] === room || isRoomDefault) && cardDate > date) {
			item.style.setProperty('display', 'block', 'important');
		} else {
			item.style.setProperty('display', 'none', 'important');
		}
	});
}
