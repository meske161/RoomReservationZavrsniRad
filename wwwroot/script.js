/* script.js */

$(document).ready(function () {
    const pageId = $('body').attr('id');

    if (pageId === 'usersPage') {
        loadUsers();
        loadBandsIntoUserForm();
        $('#userForm').on('submit', saveUser);
        $('#cancelUserEdit').on('click', resetUserForm);
    } else if (pageId === 'bandsPage') {
        loadBands();
        $('#bandForm').on('submit', saveBand);
        $('#cancelBandEdit').on('click', resetBandForm);
    } else if (pageId === 'roomsPage') {
        loadRooms();
        $('#roomForm').on('submit', saveRoom);
        $('#cancelRoomEdit').on('click', resetRoomForm);
    } else if (pageId === 'reservationsPage') {
        loadReservations();
    } else if (pageId === 'roomDetailsPage') {
        const urlParams = new URLSearchParams(window.location.search);
        const roomId = urlParams.get('id');
        loadRoomDetails(roomId);
    }
});

// Users

function loadUsers() {
    $.getJSON('/api/User', function (users) {
        const usersTableBody = $('#usersTableBody');
        usersTableBody.empty();
        $.each(users, function (index, user) {
            const row = $('<tr></tr>');

            row.append(`<td>${user.id}</td>`);
            row.append(`<td>${user.firstName}</td>`);
            row.append(`<td>${user.lastName}</td>`);
            row.append(`<td>${user.band ? user.band.name : 'N/A'}</td>`);
            row.append(`
                <td>
                    <button onclick="editUser(${user.id})">Edit</button>
                    <button onclick="deleteUser(${user.id})">Delete</button>
                </td>
            `);

            usersTableBody.append(row);
        });
    }).fail(function (jqxhr, textStatus, error) {
        console.error('Error loading users:', error);
    });
}

function loadBandsIntoUserForm() {
    $.getJSON('/api/Band', function (bands) {
        const bandSelect = $('#bandId');
        bandSelect.empty();
        bandSelect.append('<option value="">Select a Band</option>');
        $.each(bands, function (index, band) {
            bandSelect.append(`<option value="${band.id}">${band.name}</option>`);
        });
    });
}

function saveUser(event) {
    event.preventDefault();
    const userId = $('#userId').val();
    const user = {
        id: userId ? parseInt(userId) : 0,
        firstName: $('#firstName').val(),
        lastName: $('#lastName').val(),
        bandId: $('#bandId').val() ? parseInt($('#bandId').val()) : null
    };

    const method = userId ? 'PUT' : 'POST';
    const url = userId ? `/api/User/${userId}` : '/api/User';

    $.ajax({
        url: url,
        type: method,
        contentType: 'application/json',
        data: JSON.stringify(user),
        success: function () {
            alert('User saved successfully.');
            resetUserForm();
            loadUsers();
        },
        error: function (jqxhr) {
            alert('Error saving user: ' + jqxhr.responseText);
        }
    });
}

function editUser(userId) {
    $.getJSON(`/api/User/${userId}`, function (user) {
        $('#userId').val(user.id);
        $('#firstName').val(user.firstName);
        $('#lastName').val(user.lastName);
        $('#bandId').val(user.bandId);
    });
}

function deleteUser(userId) {
    if (confirm('Are you sure you want to delete this user?')) {
        $.ajax({
            url: `/api/User/${userId}`,
            type: 'DELETE',
            success: function () {
                alert('User deleted successfully.');
                loadUsers();
            },
            error: function (jqxhr) {
                alert('Error deleting user: ' + jqxhr.responseText);
            }
        });
    }
}

function resetUserForm() {
    $('#userForm')[0].reset();
    $('#userId').val('');
}

// Bands

function loadBands() {
    $.getJSON('/api/Band', function (bands) {
        const bandsTableBody = $('#bandsTableBody');
        bandsTableBody.empty();
        $.each(bands, function (index, band) {
            const row = $('<tr></tr>');

            row.append(`<td>${band.id}</td>`);
            row.append(`<td>${band.name}</td>`);
            row.append(`
                <td>
                    <button onclick="editBand(${band.id})">Edit</button>
                    <button onclick="deleteBand(${band.id})">Delete</button>
                </td>
            `);

            bandsTableBody.append(row);
        });
    }).fail(function (jqxhr, textStatus, error) {
        console.error('Error loading bands:', error);
    });
}

function saveBand(event) {
    event.preventDefault();
    const bandId = $('#bandId').val();
    const band = {
        id: bandId ? parseInt(bandId) : 0,
        name: $('#bandName').val()
    };

    const method = bandId ? 'PUT' : 'POST';
    const url = bandId ? `/api/Band/${bandId}` : '/api/Band';

    $.ajax({
        url: url,
        type: method,
        contentType: 'application/json',
        data: JSON.stringify(band),
        success: function () {
            alert('Band saved successfully.');
            resetBandForm();
            loadBands();
        },
        error: function (jqxhr) {
            alert('Error saving band: ' + jqxhr.responseText);
        }
    });
}

function editBand(bandId) {
    $.getJSON(`/api/Band/${bandId}`, function (band) {
        $('#bandId').val(band.id);
        $('#bandName').val(band.name);
    });
}

function deleteBand(bandId) {
    if (confirm('Are you sure you want to delete this band?')) {
        $.ajax({
            url: `/api/Band/${bandId}`,
            type: 'DELETE',
            success: function () {
                alert('Band deleted successfully.');
                loadBands();
            },
            error: function (jqxhr) {
                alert('Error deleting band: ' + jqxhr.responseText);
            }
        });
    }
}

function resetBandForm() {
    $('#bandForm')[0].reset();
    $('#bandId').val('');
}

// Rooms

function loadRooms() {
    $.getJSON('/api/Room', function (rooms) {
        const pageId = $('body').attr('id');

        if (pageId === 'roomsPage') {
            const roomsTableBody = $('#roomsTableBody');
            roomsTableBody.empty();
            $.each(rooms, function (index, room) {
                const row = $('<tr></tr>');

                row.append(`<td>${room.id}</td>`);
                row.append(`<td>${room.name}</td>`);
                row.append(`<td>${room.description}</td>`);
                row.append(`
                    <td>
                        <button onclick="editRoom(${room.id})">Edit</button>
                        <button onclick="deleteRoom(${room.id})">Delete</button>
                        <a href="roomDetails.html?id=${room.id}">View</a>
                    </td>
                `);

                roomsTableBody.append(row);
            });
        } else {
            const roomsList = $('#roomsList');
            roomsList.empty();
            $.each(rooms, function (index, room) {
                const listItem = $('<li></li>');

                listItem.html(`
                    <a href="roomDetails.html?id=${room.id}">${room.name}</a>
                `);

                roomsList.append(listItem);
            });
        }
    }).fail(function (jqxhr, textStatus, error) {
        console.error('Error loading rooms:', error);
    });
}

function saveRoom(event) {
    event.preventDefault();
    const roomId = $('#roomId').val();
    const room = {
        id: roomId ? parseInt(roomId) : 0,
        name: $('#roomName').val(),
        description: $('#roomDescription').val()
    };

    const method = roomId ? 'PUT' : 'POST';
    const url = roomId ? `/api/Room/${roomId}` : '/api/Room';

    $.ajax({
        url: url,
        type: method,
        contentType: 'application/json',
        data: JSON.stringify(room),
        success: function () {
            alert('Room saved successfully.');
            resetRoomForm();
            loadRooms();
        },
        error: function (jqxhr) {
            alert('Error saving room: ' + jqxhr.responseText);
        }
    });
}

function editRoom(roomId) {
    $.getJSON(`/api/Room/${roomId}`, function (room) {
        $('#roomId').val(room.id);
        $('#roomName').val(room.name);
        $('#roomDescription').val(room.description);
    });
}

function deleteRoom(roomId) {
    if (confirm('Are you sure you want to delete this room?')) {
        $.ajax({
            url: `/api/Room/${roomId}`,
            type: 'DELETE',
            success: function () {
                alert('Room deleted successfully.');
                loadRooms();
            },
            error: function (jqxhr) {
                alert('Error deleting room: ' + jqxhr.responseText);
            }
        });
    }
}

function resetRoomForm() {
    $('#roomForm')[0].reset();
    $('#roomId').val('');
}

// Room Details and Reservations

function loadRoomDetails(roomId) {
    $.getJSON(`/api/Room/${roomId}`, function (room) {
        $('#roomName').text(room.name);
        $('#roomDescription').text(room.description);
    }).fail(function (jqxhr, textStatus, error) {
        console.error('Error loading room details:', error);
    });

    // Load reservations for the room
    $.getJSON(`/api/Reservation/room/${roomId}`, function (reservations) {
        const reservationsList = $('#reservationsList');
        reservationsList.empty();
        $.each(reservations, function (index, reservation) {
            const listItem = $('<li></li>');

            listItem.text(`Reserved by ${reservation.band.name} from ${formatDateTime(reservation.startTime)} to ${formatDateTime(reservation.endTime)}`);

            reservationsList.append(listItem);
        });
    }).fail(function (jqxhr, textStatus, error) {
        console.error('Error loading reservations:', error);
    });

    // Handle reservation form submission
    $('#reservationForm').on('submit', function (event) {
        event.preventDefault();
        const reservation = {
            bandId: parseInt($('#bandId').val()),
            roomId: parseInt(roomId),
            startTime: $('#startTime').val(),
            endTime: $('#endTime').val()
        };

        $.ajax({
            url: '/api/Reservation',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(reservation),
            success: function () {
                alert('Reservation successful!');
                loadRoomDetails(roomId);
            },
            error: function (jqxhr) {
                alert('Error making reservation: ' + jqxhr.responseText);
            }
        });
    });
}

// Reservations

function loadReservations() {
    $.getJSON('/api/Reservation', function (reservations) {
        const reservationsTableBody = $('#reservationsTableBody');
        reservationsTableBody.empty();
        $.each(reservations, function (index, reservation) {
            const row = $('<tr></tr>');

            row.append(`<td>${reservation.id}</td>`);
            row.append(`<td>${reservation.band.name}</td>`);
            row.append(`<td>${reservation.room.name}</td>`);
            row.append(`<td>${formatDateTime(reservation.startTime)}</td>`);
            row.append(`<td>${formatDateTime(reservation.endTime)}</td>`);
            row.append(`
                <td>
                    <button onclick="deleteReservation(${reservation.id})">Delete</button>
                </td>
            `);

            reservationsTableBody.append(row);
        });
    }).fail(function (jqxhr, textStatus, error) {
        console.error('Error loading reservations:', error);
    });
}

function deleteReservation(reservationId) {
    if (confirm('Are you sure you want to delete this reservation?')) {
        $.ajax({
            url: `/api/Reservation/${reservationId}`,
            type: 'DELETE',
            success: function () {
                alert('Reservation deleted successfully.');
                loadReservations();
            },
            error: function (jqxhr) {
                alert('Error deleting reservation: ' + jqxhr.responseText);
            }
        });
    }
}

// Helper Functions

function formatDateTime(dateTimeString) {
    const options = { dateStyle: 'short', timeStyle: 'short' };
    return new Date(dateTimeString).toLocaleString(undefined, options);
}
