var form = document.getElementById('buyoption')
const btnDisable = document.getElementById("buybtn")
form.addEventListener('submit', function (e) {
    e.preventDefault()



    var symbol = document.getElementById('stockSelect').value;
    var quantity = document.getElementById('quantity').value;

    console.log(symbol)

    btnDisable.disabled = true;


    $.ajax({

        url: '/crypto/buycrypto',

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
  


    var symbol = document.getElementById('stockSelect').value;
    var quantity = document.getElementById('getTotalvolume').value;
    

    console.log(symbol)

    //btnDisable.disabled = true;


    $.ajax({

        url: '/crypto/sellcrypto',

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
function getcrypto() {
    let pr
    let price = document.getElementById("stock-price")
    var symbol = document.getElementById('stockSelect').value; 

      
    $.ajax({
        url: '/crypto/getcrypto',
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

    
setInterval(getcrypto, 2000);
     





// Example of calling getcrypto() after updating 'stockSelect' dynamically
function updateStockSelectAndFetch(symbol) {
    document.getElementById('stockSelect').textContent = symbol;
    getcrypto(); // Call after setting the new symbol
}

function updateField2() {
    const field1Value = parseFloat(document.getElementById("quantity").value);
    const field2Input = document.getElementById("totalamount");
    var stockvalue = document.getElementById("stock-price");
    var numberValue = parseFloat(stockvalue.textContent);
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
    var selectedCrypto = document.getElementById('stockSelect').value;
    var inputField = document.getElementById('getTotalvolume');

  

    fetch(`/crypto/getEntireCrypto?crypto=${selectedCrypto}`)
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
    var stockPriceDiv = document.getElementById("stock-price");
    var totalVolumeInput = document.getElementById('getTotalvolume');

    if (stockPriceDiv && totalVolumeInput) {
        var stockPrice = parseFloat(stockPriceDiv.textContent || stockPriceDiv.innerText);
        var totalVolume = parseFloat(totalVolumeInput.value);
        var maxVolume = parseFloat(totalVolumeInput.getAttribute('max'));

        //console.log('Stock Price:', stockPrice);
        //console.log('Total Volume:', totalVolume);
        //console.log('Max Volume:', maxVolume);

        if (totalVolume <= maxVolume && !isNaN(stockPrice) && !isNaN(totalVolume)) {
            var calculatedTotal = stockPrice * totalVolume;
            document.getElementById('caclulateTotal').value = calculatedTotal.toFixed(4);
        } else {
            document.getElementById('caclulateTotal').value = '';
        }
    } else {
        console.log('Elements not found');
    }
}

document.getElementById('stock-price').addEventListener('input', updateCalculatedTotal);
document.getElementById('getTotalvolume').addEventListener('input', updateCalculatedTotal);



// Call updateCalculatedTotal function 2 seconds after page load
setInterval(updateCalculatedTotal, 3000);

document.addEventListener('DOMContentLoaded', (event) => {
    function calculateTotalAmount() {
        var stockPriceDiv = document.getElementById("stock-price");
        var quantityInput = document.getElementById('quantity');

        if (stockPriceDiv && quantityInput) {
            var stockPrice = parseFloat(stockPriceDiv.textContent || stockPriceDiv.innerText);
            var quantity = parseFloat(quantityInput.value);

            console.log('Stock Price:', stockPrice);
            console.log('Quantity:', quantity);

            if (!isNaN(stockPrice) && !isNaN(quantity)) {
                var totalAmount = stockPrice * quantity;
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

    fetch(`/tradings/GetJsonList?page=${page}&pageSize=${pageSize}`)
        .then(response => response.json())
        .then(data => {
            populateTable(data.Data);
        })
        .catch(error => console.error('Error:', error));
}

function populateTable(data) {
    var table = document.getElementById('cryptoTable');
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

    fetch(`/tradings/getalljson?page=${page}&pageSize=${pageSize}`)
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


