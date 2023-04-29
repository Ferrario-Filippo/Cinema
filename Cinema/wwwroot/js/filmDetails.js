function onFilterClick() {
	const date = new Date($('#dateFilter').val()).getTime();
	const room = $('#roomFilter').val();

	const isRoomDefault = room === "--Scegli una sala--";

	$.each($('.show-card'), function(i, item) {
		var id = item.id.toString().split('~');
		var cardDate = new Date(id[1]).getTime();

		if ((id[0] === room || isRoomDefault) && cardDate > date) {
			item.style.setProperty('display', 'block', 'important');
		} else {
			item.style.setProperty('display', 'none', 'important');
		}
	});
}

function onShowClicked(sender) {
	$.each($('.show-card'), function(i, item) {
		item.style.setProperty('border-color', '');
	});
	sender.style.setProperty('border-color', '#78c2ad', 'important');

	var showId = sender.id.toString().split('~')[2];

	$('#chosenShow').val(showId);
	$('#submitBtn').prop('disabled', false);
}
