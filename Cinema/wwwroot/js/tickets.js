var dataTable;

$(document).ready(function () {
	loadDataTable();
})

function loadDataTable() {
	dataTable = $('#tblData').DataTable({
		language: {
			url: "https://cdn.datatables.net/plug-ins/1.13.2/i18n/it-IT.json"
		},
		ajax: {
			url: "/Admin/Ticket/GetAll"
		},
		columns: [
			{ data: "number", width: "10%" },
			{ data: "lane", width: "10%" },
			{ data: "showId", width: "20%" },
			{ data: "cost", width: "20%" },
			{ data: "userid", width: "10%" },
			{
				data: { "showId", "number", "lane" },
				render: function (data) {
					return `
						<div class="w-75 btn-group" role="group">
						<a href="/Admin/Ticket/Upsert?showId=${data.showId}&number=${data.number}&lane=${data.lane}" class="btn btn-primary mx-2">
							<i class="bi bi-pencil-square"></i>Modifica</a>
						<a onClick=Delete("/Admin/Ticket/Delete?showId=${data.id}&number=${data.number}&lane=${data.lane}") class="btn btn-danger mx-2">
							<i class="bi bi-trash-fill"></i>Elimina</a>
						</div>
					`
				},
				width: "40%"
			}
		]
	});
}

function Delete(url) {
	Swal.fire({
		title: 'Sei sicuro?',
		text: `Azione irreversibile`,
		icon: 'warning',
		showCancelButton: true,
		confirmButtonColor: '#b84743',
		cancelButtonColor: '#bbb',
		confirmButtonText: 'Si, cancellalo!'
	}).then((result) => {
		if (result.isConfirmed) {
			$.ajax({
				url: url,
				type: 'DELETE',
				success: function (data) {
					if (data.success) {
						dataTable.ajax.reload();
						toastr.success(data.message);
					} else {
						toastr.error(data.message);
					}
				}
			})
		}
	})
}
