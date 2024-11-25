let timerID;
function getDate() {
	let dateNow = new Date();

	let dayOfMonth = dateNow.getDate();
	let year = dateNow.getFullYear();

	let monthArray = [
		'January',
		'February',
		'March',
		'April',
		'May',
		'June',
		'July',
		'August',
		'September',
		'October',
		'November',
		'December'
	];
	let month = monthArray[dateNow.getMonth()];
	let todayArray = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];
	let today = todayArray[dateNow.getDay()];

	let date = today + ', ' + month + ' ' + dayOfMonth + ', ' + year;

	$("#date").html(date);
	//console.log(today + ', ' + month + ' ' + dayOfMonth + ', ' + year);
}

function clockLocal() {
	let dateNow = new Date();

	let suffix;

	let hour = dateNow.getHours();
	let minute = dateNow.getMinutes();
	let second = dateNow.getSeconds();

	//convert to 12 hr format
	if (hour === 0) {
		hour = hour + 12;
		suffix = 'AM';
	} else if (hour >= 1 && hour <= 11) {
		suffix = 'AM';
	} else if (hour === 12) {
		suffix = 'PM';
	} else if (hour >= 13 && hour <= 23) {
		hour = hour - 12;
		suffix = 'PM';
	}

	//format minute and second if less than 10
	if (minute < 10) {
		minute = '0' + minute;
	}
	if (second < 10) {
		second = '0' + second;
	}

	let time = hour + ':' + minute + ':' + second + ' ' + suffix;

	$("#time").html(time);
	//console.log(hour + ':' + minute + ':' + second + ' '+ suffix);
}

(function startClock() {
	window.addEventListener('DOMContentLoaded', (event) => {
		timerID = setInterval(clockLocal, 1000);
		getDate();
	});

})();

// Clear the interval when navigating away
window.addEventListener('beforeunload', (event) => {
	clearInterval(timerID);
});