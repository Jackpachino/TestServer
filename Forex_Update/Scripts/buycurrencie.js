var form = document.getElementById('buyoption')
const btnDisable = document.getElementById("buybtn")
form.addEventListener('submit', function (e) {
    e.preventDefault()



    var symbol = document.getElementById('currencySelect').value;
    var quantity = document.getElementById('quantity').value;

    console.log(symbol)

    btnDisable.disabled = true;


    $.ajax({

        url: '/currencies/maketransaction',

        type: "GET",
        cache: false,
        data: { symbol: symbol, quantity: quantity },
        success: function (data) {
            disablefunction();
            if (data == "yes") {

                showToast();
                updateTotalVolume();
                updateCalculatedTotal();                
                getuserballance();
    

            }
            else {

                showToast2();
                updateTotalVolume();
                updateCalculatedTotal();
                getuserballance();
            }
        },

    })


        .catch(function (error) {
            doc.removeChild(loader);
            console.error('Error:', error);
        });

    function disablefunction() {
        btnDisable.disabled = true;
    };



});

///sell function
var form = document.getElementById('sellform')
/*const btnDisable = document.getElementById("sellbtn")*/
form.addEventListener('submit', function (e) {
    e.preventDefault()



    var symbol = document.getElementById('currencySelect').value;
    var quantity = document.getElementById('getTotalvolume').value;


    console.log(symbol)

    //btnDisable.disabled = true;


    $.ajax({

        url: '/currencies/sellcurrency',

        type: "POST",
        cache: false,
        data: { symbol: symbol, quantity: quantity },
        success: function (data) {
            /* disablefunction();*/

            if (data == "1" || data == 1) {

                showToastSellApproved();
                updateTotalVolume();
                updateCalculatedTotal();
                getuserballance();
                /*  emptyTable();*/
                /* fetchData();*/

            }
            else {

                showToastSellDeclined();
                updateTotalVolume();
                updateCalculatedTotal();
                getuserballance();
            }
        },

    })


        .catch(function (error) {
            doc.removeChild(loader);
            console.error('Error:', error);
        });

    //function disablefunction() {
    //    btnDisable.disabled = true;
    //};



});
/////
function getcurrency() {
    let pr
    let price = document.getElementById("currency-price")
    var symbol = document.getElementById('currencySelect').value;


    $.ajax({
        url: '/currencies/getcurrency',
        type: 'GET',
        data: { symbol: symbol },
        success: function (data) {
            pr = data;
            price.textContent = pr.trim() // Handle the data received here
        },
        error: function (error) {
            console.error('Error:', error);
        }
    });
}


setInterval(getcurrency, 2000);






// Example of calling getcurrency() after updating 'currencySelect' dynamically
function updatecurrencySelectAndFetch(symbol) {
    document.getElementById('currencySelect').textContent = symbol;
    getcurrency(); // Call after setting the new symbol
}

function updateField2() {
    const field1Value = parseFloat(document.getElementById("quantity").value);
    const field2Input = document.getElementById("totalamount");
    var currencyvalue = document.getElementById("currency-price");
    var numberValue = parseFloat(currencyvalue.textContent);
    console.log(numberValue);


    if (!isNaN(field1Value)) {
        field2Input.value = (field1Value * numberValue).toFixed(4);
    } else {
        field2Input.value = "";
    }

}



function showToastSellApproved() {

    const toast = document.getElementById("sellApproved");

    // Customize your toast message here
    const message = "Sell Approved Check Your Ballance";

    // Set the toast message
    toast.textContent = message;

    // Show the toast
    toast.style.display = "block";

    // Hide the toast after a few seconds (adjust the timeout as needed)
    setTimeout(() => {
        toast.style.display = "none";
    }, 6000); // 6000 milliseconds (6 seconds) timeout

    updateTotalVolume();

    updateCalculatedTotal();

    /* button.disabled = false;*/



    /* closeProfitPopup();*/
}

function showToastSellDeclined() {
    const toast = document.getElementById("sellDeclined");
    const message = "Sell Declined Check Your Volume";
    toast.textContent = message;
    toast.style.display = "block";

    setTimeout(() => {
        toast.style.display = "none";
    }, 6000); // 6 seconds timeout

    // Assuming 'button' is defined and accessible in this scope
    //if (button) {
    //    button.disabled = false;
    //}

    // Call updateTotalVolume
    updateTotalVolume();
}


function showToast() {

    const toast = document.getElementById("toast");

    // Customize your toast message here
    const message = "Transaction Succeed Check Ballance";

    // Set the toast message
    toast.textContent = message;

    // Show the toast
    toast.style.display = "block";

    // Hide the toast after a few seconds (adjust the timeout as needed)
    setTimeout(() => {
        toast.style.display = "none";
    }, 6000); // 6000 milliseconds (6 seconds) timeout



    btnDisable.disabled = false;

    updateTotalVolume();

    updateCalculatedTotal();

}

function showToast2() {
    const toast = document.getElementById("toast2");

    // Customize your toast message here
    const message = "Not Enought Ballance !";

    // Set the toast message
    toast.textContent = message;

    // Show the toast
    toast.style.display = "block";

    // Hide the toast after a few seconds (adjust the timeout as needed)
    setTimeout(() => {
        toast.style.display = "none";
    }, 5000);

    btnDisable.disabled = false;


};


function updateTotalVolume() {
    var selectedcurrency = document.getElementById('currencySelect').value;
    var inputField = document.getElementById('getTotalvolume');



    fetch(`/currencies/getEntirecurrency?currency=${selectedcurrency}`)
        .then(response => response.json())
        .then(data => {
            if (data.error) {
                console.error('Error:', data.error);
            } else {
                inputField.value = data; // Loaded value, for example, 0.5052
                inputField.step = '0.0001'; // Decrease by 0.0001
                inputField.max = data; // Set max to the loaded value
                inputField.min = 0; // Set a sensible minimum, e.g., 0
            }
        })
        .catch(error => console.error('Fetch error:', error));
}
updateTotalVolume();



function updateCalculatedTotal() {
    var currencyPriceDiv = document.getElementById("currency-price");
    var totalVolumeInput = document.getElementById('getTotalvolume');

    if (currencyPriceDiv && totalVolumeInput) {
        var currencyPrice = parseFloat(currencyPriceDiv.textContent || currencyPriceDiv.innerText);
        var totalVolume = parseFloat(totalVolumeInput.value);
        var maxVolume = parseFloat(totalVolumeInput.getAttribute('max'));

        //console.log('currency Price:', currencyPrice);
        //console.log('Total Volume:', totalVolume);
        //console.log('Max Volume:', maxVolume);

        if (totalVolume <= maxVolume && !isNaN(currencyPrice) && !isNaN(totalVolume)) {
            var calculatedTotal = currencyPrice * totalVolume;
            document.getElementById('caclulateTotal').value = calculatedTotal.toFixed(4);
        } else {
            document.getElementById('caclulateTotal').value = '';
        }
    } else {
        console.log('Elements not found');
    }
}

document.getElementById('currency-price').addEventListener('input', updateCalculatedTotal);
document.getElementById('getTotalvolume').addEventListener('input', updateCalculatedTotal);



// Call updateCalculatedTotal function 2 seconds after page load
setInterval(updateCalculatedTotal, 3000);

document.addEventListener('DOMContentLoaded', (event) => {
    function calculateTotalAmount() {
        var currencyPriceDiv = document.getElementById("currency-price");
        var quantityInput = document.getElementById('quantity');

        if (currencyPriceDiv && quantityInput) {
            var currencyPrice = parseFloat(currencyPriceDiv.textContent || currencyPriceDiv.innerText);
            var quantity = parseFloat(quantityInput.value);

            console.log('currency Price:', currencyPrice);
            console.log('Quantity:', quantity);

            if (!isNaN(currencyPrice) && !isNaN(quantity)) {
                var totalAmount = currencyPrice * quantity;
                document.getElementById('totalamount').value = totalAmount.toFixed(4); // Adjust the precision as needed
            } else {
                document.getElementById('totalamount').value = '';
            }
        } else {
            console.log('Elements not found');
        }
    }

    // Attach the calculateTotalAmount function to the 'input' event of the quantity field
    document.getElementById('quantity').addEventListener('input', calculateTotalAmount);

    // Optional: Call the function initially if needed
    calculateTotalAmount();
});

function fetchData(page) {
    var pageSize = document.getElementById('pageSize').value;

    fetch(`/tradings/getjsonlistcurrency?page=${page}&pageSize=${pageSize}`)
        .then(response => response.json())
        .then(data => {
            populateTable(data.Data);
        })
        .catch(error => console.error('Error:', error));
}

function populateTable(data) {
    var table = document.getElementById('currencyTable');
    var tbody = table.getElementsByTagName('tbody')[0] || table.appendChild(document.createElement('tbody'));
    tbody.innerHTML = ''; // Clear existing rows

    data.forEach(trade => {
        var row = tbody.insertRow();
        row.innerHTML = `
            <td>${formatDate(trade.BuyTime)}</td>
            <td>${trade.Symbol}</td>
            <td>${trade.Type}</td>
            <td>${trade.Openprice.toFixed(2)}</td>
            <td>${trade.TotalPayedAmount.toFixed(2)}</td>
        `;
    });
}

function formatDate(dateString) {
    var date = new Date(parseInt(dateString.substr(6)));
    return date.toLocaleDateString() + ' ' + date.toLocaleTimeString();
}

// Initial fetch
fetchData(1);



function fetchTradingData(page) {
    var pageSize = document.getElementById('pageSizeTrading').value;

    fetch(`/tradings/getalljsoncurrency?page=${page}&pageSize=${pageSize}`)
        .then(response => response.json())
        .then(data => {
            populateTradingTable(data.Data);
        })
        .catch(error => console.error('Error:', error));
}

function populateTradingTable(data) {
    var table = document.getElementById('tradingTable');
    var tbody = table.getElementsByTagName('tbody')[0] || table.appendChild(document.createElement('tbody'));
    tbody.innerHTML = ''; // Clear existing rows

    data.forEach(trade => {
        var row = tbody.insertRow();
        row.innerHTML = `
            <td>${formatDate(trade.BuyTime)}</td>
            <td>${trade.Symbol}</td>
            <td>${trade.Type}</td>
            <td>${trade.Openprice.toFixed(2)}</td>
            <td>${trade.TotalPayedAmount.toFixed(2)}</td>
        `;
    });
}

function formatDate(dateString) {
    var date = new Date(parseInt(dateString.substr(6)));
    return date.toLocaleDateString() + ' ' + date.toLocaleTimeString();
}

// Initial fetch for the trading table
fetchTradingData(1);

