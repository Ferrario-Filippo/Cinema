function onRadioSelectChanged(sender) {
	const container = sender.parentElement;
	const filmId = container.id;
	const value = sender.value;

	var i = 0;
	for (; i < value; i++) {
		const starIndex = i * 2 + 1;
		container.children[starIndex].children[0].classList.remove('bi-star');
		container.children[starIndex].children[0].classList.add('bi-star-filled');
	}
	for (; i < 5; i++) {
		const starIndex = i * 2 + 1;
		container.children[starIndex].children[0].classList.remove('bi-star-filled');
		container.children[starIndex].children[0].classList.add('bi-star');
	}

	$.get(`/Customer/User/SaveReview?filmId=${filmId}&rating=${value}`, function () { });
}