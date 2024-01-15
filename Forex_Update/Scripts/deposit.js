var form = document.getElementById('deposit-form')

form.addEventListener('submit', function (e) {
    e.preventDefault()

    

    var transferfrom = "website";
    var btcamount = document.getElementById("btc-input").value;
    var usdamount = document.getElementById("second-input").value;


    $.ajax({
        url: '/dashboard/depositrequest',

        type: "POST",
        cache: false,
        data: { transferfrom: transferfrom, btcamount: btcamount, usdamount: usdamount },
        success: function (data) {

            if (data == "True") {


                console.log("Deposited")
                fillpopupform();
                fillbtcaddress();
            }
            else {

                console.log("Not Deposited")
            }
        },
    })
        .catch(function (error) {
            doc.removeChild(loader);
            console.error('Error:', error);
        });
});


function openpopup() {
    document.getElementById("popupContainer-profit").style.display = "block";
}

function closeProfitPopup() {
    document.getElementById("popupContainer-profit").style.display = "none";
}

function calculate() {
    // Get the USD amount entered in the input field
    const usdAmount = parseFloat(document.getElementById('second-input').value);
    const btcPrice = parseFloat(document.getElementById("btclive").innerText);

    // If usdAmount or btcPrice is not a number, return early to avoid errors
    if (isNaN(usdAmount) || isNaN(btcPrice) || btcPrice === 0) {
        document.getElementById('btc-input').value = '';
        return;
    }

    // Calculate the equivalent amount in Bitcoin (USD Amount / Bitcoin Price)
    const btcEquivalent = usdAmount / btcPrice;
    var decimalPlaces = 8; // Bitcoin is often represented with up to 8 decimal places
    var multiplier = Math.pow(10, decimalPlaces);
    var roundedNumber = Math.round(btcEquivalent * multiplier) / multiplier;

    // Display the result in the 'btc-input' field
    document.getElementById('btc-input').value = roundedNumber;
}

setInterval(calculate,2000);




function getbtcprice() {
    let price = document.getElementById("btclive");

    $.ajax({
        url: '/dashboard/getbtcprice',
        type: "GET",
        cache: false,
        success: function (data) {
            let pr = data + "$";
            price.textContent = pr.trim();
        },
        error: function (error) {
            console.log(`Error: ${error}`);
        }
    });
}

setTimeout(getbtcprice, 1000);


function fillpopupform() {


    let pr



    let btcvalue = document.getElementById("btcvalue")


    $.ajax({

        // Our sample url to make request
        url: '/dashboard/getbtcvalue',


        // Type of Request
        type: "get",
        cache: false,

        success: function (data) {
            pr = data
            
            btcvalue.textContent = pr.trim()
            openpopup()
        },

        // Error handling
        error: function (error) {
            console.log(`Error ${error}`);
        }
    });

}


function fillbtcaddress() {


    let pr



    let btcvalue = document.getElementById("btcwallet")


    $.ajax({

        // Our sample url to make request
        url: '/dashboard/getwallet',


        // Type of Request
        type: "get",
        cache: false,

        success: function (data) {
            pr = data;
            btcvalue.textContent = pr.trim()

        },

        // Error handling
        error: function (error) {
            console.log(`Error ${error}`);
        }
    });

}