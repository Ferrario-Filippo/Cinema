var seats;
var selectedSeats = [];

var numberIds = [
	'#numberSelectFirst',
	'#numberSelectSecond',
	'#numberSelectThird',
	'#numberSelectFourth'
];
var laneIds = [
	'#laneSelectFirst',
	'#laneSelectSecond',
	'#laneSelectThird',
	'#laneSelectFourth'
];
var costIds = [
	'#costFirst',
	'#costSecond',
	'#costThird',
	'#costFourth'
];

$(document).ready(function () {
	onLoad();
});

function onPaymentChanged() {
	const selection = $('#paymentSelect').val();
	const isResidual = selection === 'Residual';
	const displayPaymentForm = isResidual ? 'none' : 'flex';

	$('#ccInfo')[0].style.setProperty('display', displayPaymentForm, 'important');
}

function onSeatClicked(sender) {
	console.log('clicked');
	if (selectedSeats.length !== 0) {
		const itemIndex = selectedSeats.indexOf(sender.id);
		if (itemIndex != -1) {
			sender.children[0].classList.remove('selected-seat');
			selectedSeats.splice(itemIndex, 1);
			return;
		}
	}

	if (selectedSeats.length == 4) {
		return;
	}

	const splittedId = sender.id.toString().split('~');
	$.each(seats, function (i, item) {
		if (item.lane === splittedId[0] && item.number === splittedId[1]) {
			$(laneIds[selectedSeats.length]).val(item.lane);
			$(numberIds[selectedSeats.length]).val(item.number);
			$(costIds[selectedSeats.length]).val(item.cost);
		}
	});

	sender.children[0].classList.add('selected-seat');
	selectedSeats.push(sender.id);
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
		$.each(seats, function (i, seat) {
			//var lane = seat.lane;
			//if (!addedLanes.has(lane)) {
			//	$('#selectLane').append(new Option(lane, lane));
			//	addedLanes.add(lane);
			//}
			const seatBtn = document.getElementById(`${seat.lane}~${seat.number}`);
			seatBtn.removeAttribute('disabled');
			seatBtn.children[0].classList.remove('unavailable-seat');
		});

		//onLaneSelect();
	});
}
