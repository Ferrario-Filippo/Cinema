var revenuesTable;
var bookingsTable;

$(document).ready(function () {
	loadTables();
})

function loadTables() {
	const date = new Date(Date.now());

	revenuesTable = $('#revenuesTbl').DataTable({
		language: {
			url: "https://cdn.datatables.net/plug-ins/1.13.2/i18n/it-IT.json"
		},
		ajax: {
			url: `/Admin/Statistics/GetAverageDailyRevenues?day=${date.getDate()}&month=${date.getMonth() + 1}&year=${date.getFullYear()}`
		},
		columns: [
			{ data: "filmId", width: "10%" },
			{ data: "title", width: "25%" },
			{ data: "revenues", width: "10%" },
		]
	});

	bookingsTable = $('#bookingsTbl').DataTable({
		language: {
			url: "https://cdn.datatables.net/plug-ins/1.13.2/i18n/it-IT.json"
		},
		ajax: {
			url: `/Admin/Statistics/GetBookedSeatsPerRoom?day=${date.getDate()}&month=${date.getMonth() + 1}&year=${date.getFullYear()}`
		},
		columns: [
			{ data: "showId", width: "10%" },
			{ data: "booked", width: "10%" },
			{ data: "seats", width: "10%" },
		]
	});
}

function updateTables() {
	if (revenuesTable === null || bookingsTable === null) {
		console.log('error')
		return;
	}

	const date = new Date(document.getElementById('dateFilter').value);

	$.get(`/Admin/Statistics/GetAverageDailyRevenues?day=${date.getDate()}&month=${date.getMonth() + 1}&year=${date.getFullYear()}`, function (data) {
		revenuesTable.clear();
		revenuesTable.rows.add(data['data']);
		revenuesTable.draw();
	})

	$.get(`/Admin/Statistics/GetBookedSeatsPerRoom?day=${date.getDate()}&month=${date.getMonth() + 1}&year=${date.getFullYear()}`, function (data) {
		console.log(data);
		bookingsTable.clear();
		bookingsTable.rows.add(data['data']);
		bookingsTable.draw();
	})
}
