var selectedSeats = [];
var count = 0;

function toggleSeat(seat, seatName, seatType, scheduleId, numTicket){
    if(seat.classList.contains('bg-red-500')){
        seat.classList.remove('bg-red-500');
        seat.classList.remove('hover:bg-red-800');

        if(seatType === 'Business'){
            seat.classList.add('bg-green-500');
            seat.classList.add('hover:bg-green-800');
        }                            
        else{
            seat.classList.add('bg-indigo-500');
            seat.classList.add('hover:bg-indigo-500');
        }
        var index = selectedSeats.indexOf(seatName);
        if(index !== -1){
            selectedSeats.splice(index, 1);
            count--;
        }
    }
    else{
        if(selectedSeats.length < numTicket){
            seat.classList.remove('bg-green-500', 'bg-indigo-500', 'hover:bg-green-800', 'hover:bg-indigo-800');
            seat.classList.add('bg-red-500');
            seat.classList.add('hover:bg-red-800');
            // Add seat to selectedSeats list
            selectedSeats.push(seatName);
            count++;
        }               
        else
            alert('You can only select up to '+numTicket+' seats.');
    }
    updateSelectedSeatsInfo();
    updateFinishButtonLink(scheduleId, numTicket);
}
function updateSelectedSeatsInfo() {
    var infoElement = document.getElementById('selectedSeatsInfo');
    infoElement.textContent = 'Selected Seats: ' + selectedSeats.join(', ');
}
function updateFinishButtonLink(scheduleId, numTicket) {
    var finishButton = document.getElementById('finishButton');
    if (finishButton) {

        finishButton.href = '/Payment/Index?scheduleId='
            + scheduleId
            + '&listSeatSelected=' + encodeURIComponent(selectedSeats.join(','));

        // Show/hide the finishButton based on the selected seats count
        finishButton.style.display = selectedSeats.length === numTicket ? 'block' : 'none';
    }
}