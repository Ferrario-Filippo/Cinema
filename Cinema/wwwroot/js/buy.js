var selectedSeats = [];
var seats;

$(document).ready(function () {
	onLoad();
});

function onPaymentChanged() {
	const selection = $('#paymentSelect').val();
	const isResidual = selection === 'Residual';
	const displayPaymentForm = isResidual ? 'none' : 'flex';

	$('#ccInfo')[0].style.setProperty('display', displayPaymentForm, 'important');
}

function onSeatClicked() {
	
}

function onLaneSelect() {
	//const lane = laneSelect.value;
	//while (numberSelect.options.length > 0) {
	//	numberSelect.remove(0);
	//}
	//if (lane === defaultLane) {
	//	$('#selectNumber').append(`<option disabled selected value="${defaultNumber}">${defaultNumber}</option>`);
	//}

	//$.each(seats, function (i, seat) {
	//	if (seat.lane === lane) {
	//		console.log(lane, seat.number)
	//		$('#selectNumber').append(new Option(seat.number, seat.number));
	//	}
	//});
}

function onLoad() {
	const showId = $('#showIdInput').val();
	//laneSelect = document.getElementById('selectLane');
	//numberSelect = document.getElementById('selectNumber');

	$.get(`/Api/Ticket/GetAvailableSeats?showId=${showId}`, function (data) {
		seats = data['data'];
		//var addedLanes = new Set();
		//addedLanes.add(defaultLane);
		console.log(seats);
		$.each(seats, function (i, seat) {
			//var lane = seat.lane;
			//if (!addedLanes.has(lane)) {
			//	$('#selectLane').append(new Option(lane, lane));
			//	addedLanes.add(lane);
			//}
			const seatBtn = document.getElementById(`${seat.lane}~${seat.number}`);
			seatBtn.setAttribute('disabled', false);
			seatBtn.children[0].classList.remove('unavailable-seat');
		});

		//onLaneSelect();
	});
}
