function roomChanged() {
	const roomId = $('#roomSelect').val();

	$.get(`/Api/Room/HasISense?roomId=${roomId}`, function (data) {
		const section = document.getElementById('is3d-section');
		if (data['hasIsense']) {
			section.removeAttribute('hidden');
		} else {
			section.setAttribute('hidden', 'true');
		}
	});
}
