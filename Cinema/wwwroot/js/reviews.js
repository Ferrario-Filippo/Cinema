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
			url: "/Admin/Review/GetAll"
		},
		columns: [
			{ data: "Rating", width: "15%" },
			{ data: "user", width: "15%" },
			{ data: "film", width: "15%" },
			{
				data: "reviewId",
				render: function (data) {
					return `
						<div class="w-75 btn-group" role="group">
						<a href="/Admin/Review/Upsert?id=${data}" class="btn btn-primary mx-2">
							<i class="bi bi-pencil-square"></i>Modifica</a>
						<a onClick=Delete("/Admin/Review/Delete/${data}") class="btn btn-danger mx-2">
							<i class="bi bi-trash-fill"></i>Elimina</a>
						</div>
					`
				},
				width: "15%"
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
