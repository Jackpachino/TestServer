
    document.getElementById('editprofile').addEventListener('submit', function(event) {
        event.preventDefault(); // Prevent the default form submission

    var firstName = document.getElementById('firstname').value;
    var lastName = document.getElementById('lastname').value;
    var email = document.getElementById('email').value;
    var phone = document.getElementById('phone').value;

    $.ajax({
        url: '/manage/editporfile', // Replace with your controller and action method name
        type: 'POST',
        data: {
            firstname: firstName,
            lastname: lastName,
            email: email,
            phone: phone 
        },
        success: function (response) {
            if (response.success) {
                showToast(); // Call your notification function
            } else {
                // Handle failure
                showToast2()
                console.error('Profile update failed');
            }
        },
        error: function (xhr, status, error) {
            // Handle error
            console.error('Error sending data', xhr, status, error);
        }
    });
});



function showToast() {
    const toast = document.getElementById("toast");
    const message = "Profile updated successfully";
    toast.textContent = message;
    toast.style.display = "block";

    // Hide the toast after 6 seconds and then refresh the page
    setTimeout(() => {
        toast.style.display = "none";

        // Refresh the page
        window.location.reload();
    }, 4000); // 6000 milliseconds (6 seconds) timeout
}


function showToast2() {
    const toast = document.getElementById("toast2");

    // Customize your toast message here
    const message = "Profile update failed !";

    // Set the toast message
    toast.textContent = message;

    // Show the toast
    toast.style.display = "block";

    // Hide the toast after a few seconds (adjust the timeout as needed)
    setTimeout(() => {
        toast.style.display = "none";
    }, 4000);

   
    window.location.reload();

};