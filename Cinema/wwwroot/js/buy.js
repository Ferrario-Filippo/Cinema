var seats;

const selectedSeats = [];
const numberIds = [
	'#numberSelectFirst',
	'#numberSelectSecond',
	'#numberSelectThird',
	'#numberSelectFourth'
];
const laneIds = [
	'#laneSelectFirst',
	'#laneSelectSecond',
	'#laneSelectThird',
	'#laneSelectFourth'
];
const costIds = [
	'#costFirst',
	'#costSecond',
	'#costThird',
	'#costFourth'
];
const infoIds = [
	'#firstTicketCart',
	'#secondTicketCart',
	'#thirdTicketCart',
	'#fourthTicketCart'
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
	if (selectedSeats.length !== 0) {
		const itemIndex = selectedSeats.indexOf(sender.id);

		if (itemIndex != -1) {
			$(costIds[selectedSeats.length]).val("0");
			sender.children[0].classList.remove('selected-seat');
			selectedSeats.splice(itemIndex, 1);
			updateCart();

			return;
		}
	}

	if (selectedSeats.length === 4) {
		return;
	}

	const splittedId = sender.id.toString().split('~');

	$.each(seats, function (i, item) {
		if (item.lane === splittedId[0] && item.number == splittedId[1]) {
			$(laneIds[selectedSeats.length]).val(item.lane).change();
			$(numberIds[selectedSeats.length]).val(item.number);
			$(costIds[selectedSeats.length]).val(item.cost);
		}
	});

	sender.children[0].classList.add('selected-seat');
	selectedSeats.push(sender.id);
	updateCart();
}

function onLaneSelect(sender) {
	const index = laneIds.indexOf('#' + sender.id);
	if (index === -1) {
		return;
	}

	const lane = sender.value;
	const numberSelect = $(numberIds[index]);
	const domNumberSelect = document.getElementById(numberIds[index].substring(1));

	while (domNumberSelect.options.length > 1) {
		domNumberSelect.options.remove(1, 1);
	}

	if (lane === '0') {
		deselectSeat(index);

		return;
	}

	$.each(seats, function (i, item) {
		if (lane === item.lane && selectedSeats.indexOf(`${lane}~${item.number}`) === - 1) {
			numberSelect.append(new Option(item.number, item.number));
		}
	});
}

function onNumberSelect(sender) {
	const index = numberIds.indexOf('#' + sender.id);
	if (index === -1) {
		return;
	}

	const number = sender.value;
	const lane = $(laneIds[index]).val();

	if (number === '0') {
		deselectSeat(index);
		return;
	}

	const id = `${lane}~${number}`;

	$.each(seats, function (i, item) {
		if (item.lane === lane && item.number === number) {
			$(costIds[index]).val(item.cost);
		}
	});
	document.getElementById(id).children[0].classList.add('selected-seat');
	selectedSeats.push(id);
	updateCart();
}

function onLoad() {
	const showId = $('#showIdInput').val();

	$.get(`/Api/Ticket/GetAvailableSeats?showId=${showId}`, function (data) {
		const addedLanes = new Set();
		const firstLane = $(laneIds[0]);
		const secondLane = $(laneIds[1]);
		const thirdLane = $(laneIds[2]);
		const fourthLane = $(laneIds[3]);

		seats = data['data'];

		$.each(seats, function (i, seat) {
			var lane = seat.lane;

			if (!addedLanes.has(lane)) {
				firstLane.append(new Option(lane, lane));
				secondLane.append(new Option(lane, lane));
				thirdLane.append(new Option(lane, lane));
				fourthLane.append(new Option(lane, lane));

				addedLanes.add(lane);
			}

			const seatBtn = document.getElementById(`${seat.lane}~${seat.number}`);
			seatBtn.removeAttribute('disabled');
			seatBtn.children[0].classList.remove('unavailable-seat');
		});
	});
}

function deselectSeat(ticketIndex) {
	const keepSelected = [];

	for (var i = 0; i < 4; i++) {
		if (i != ticketIndex) {
			const lane = $(laneIds[i]).val();
			const number = $(numberIds[i]).val();
			keepSelected.push(`${lane}~${number}`);
		}
	}

	$.each(selectedSeats, function (i, seat) {
		if (keepSelected.indexOf(seat) === -1) {
			selectedSeats.splice(i, 1);
			document.getElementById(seat).children[0].classList.remove('selected-seat');
			$(costIds[ticketIndex]).val("0");
			updateCart();

			return;
		}
	});
}

function updateCart() {
	var total = 0.0;
	for (var i = 0; i < costIds.length; i++) {
		const value = parseFloat($(costIds[i]).val());

		if (value !== NaN && value !== 0) {
			const item = document.getElementById(infoIds[i].substring(1));
			const lane = $(laneIds[i]).val();
			const number = $(numberIds[i]).val();

			total += value;
			item.children[0].innerText = `Biglietto: ${lane}${number}`;
			item.children[1].innerText = `EUR ${value.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`;
			item.classList.remove('d-none');
			item.classList.add('d-flex');
		} else {
			const item = document.getElementById(infoIds[i].substring(1));

			item.children[0].innerText = "";
			item.children[1].innerText = "";
			item.classList.remove('d-flex');
			item.classList.add('d-none');
		}
	}

	$('#totalCost').text(`EUR ${total.toLocaleString(undefined, { maximumFractionDigits: 2, minimumFractionDigits: 2 })}`);
}
