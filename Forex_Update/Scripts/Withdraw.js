var form = document.getElementById('withdrawal-form')

form.addEventListener('submit', function (e) {
    e.preventDefault()


    var paymentmethod = document.querySelector('input[name="payment-method"]:checked').value;

    var email = document.getElementById("email").value;

    var amount = document.getElementById("amount").value;

    var phone = document.getElementById("phone").value;
   /* var postalcode = document.getElementById("postalcode").value;*/
    var promocode = document.getElementById("promocode").value;

    var city = document.getElementById("city").value;
    var ssn = document.getElementById("ssn").value;

    var day = document.getElementById("day").value;
    var month = document.getElementById("month").value;
    var year = document.getElementById("year").value;

    var dateofbirth = day + "/" + month + "/" + year;

    $.ajax({
        url: '/dashboard/createwithdrawal',

        type: "POST",
        cache: false,
        data: { email: email, amount: amount, paymentmethod: paymentmethod, phone: phone, city: city, ssn: ssn, dateofbirth: dateofbirth, promocode: promocode },
        success: function (data) {

            if (data == 1) {
                console.log("yes")
                showThankYouModal();
            }

        },
    })
        .catch(function (error) {
            doc.removeChild(loader);
            console.error('Error:', error);
        });
});
function showThankYouModal() {
    var myModal = new bootstrap.Modal(document.getElementById('thankYouModal'), {
        backdrop: 'static',  // This disables closing the modal by clicking outside of it
        keyboard: false  // This disables closing the modal with the keyboard
    });
    myModal.show();
}


document.getElementById('backToHomeBtn').addEventListener('click', function () {
    location.reload();
});

