var seats;
var laneSelect;
var numberSelect;
var defaultNumber;
var defaultLane;

$(document).ready(function () {
	onLoad();
});

function onLaneSelect() {
	const lane = laneSelect.value;
	while (numberSelect.options.length > 0) {
		numberSelect.remove(0);
	}
	if (lane === defaultLane) {
		$('#selectNumber').append(`<option disabled selected value="${defaultNumber}">${defaultNumber}</option>`);
	}

	$.each(seats, function (i, seat) {
		if (seat.lane === lane) {
			console.log(lane, seat.number)
			$('#selectNumber').append(new Option(seat.number, seat.number));
		}
	});
}

function onNumberSelect() {
	const number = numberSelect.value;
	const lane = laneSelect.value;

	for (seat in seats) {
		if (seat.lane === lane && seat.number === number) {
			$('#costInput').val(seat.cost);
		}
	}
}

function onLoad() {
	const showId = document.getElementById('showInput').value;
	laneSelect = document.getElementById('selectLane');
	numberSelect = document.getElementById('selectNumber');

	
	defaultLane = laneSelect.value;
	defaultNumber = numberSelect.value;

	$.get(`/Api/Ticket/GetAvailableSeats?showId=${showId}`, function (data) {
		seats = data['data'];
		var addedLanes = new Set();
		addedLanes.add(defaultLane);

		$.each(seats, function (i, seat) {
			var lane = seat.lane;
			if (!addedLanes.has(lane)) {
				$('#selectLane').append(new Option(lane, lane));
				addedLanes.add(lane);
			}
		});

		onLaneSelect();
	});
}