function getuserballance() {
    let pr

    let mainballance = document.getElementById("ballanceajax")

    Moneyinstock();
    EquityCalculator();

    $.ajax({

        // Our sample url to make request
        url: '/stock/getballance',


        // Type of Request
        type: "GET",

        // Function to call when to
        // request is ok
        success: function (data) {
            pr = "$" + data;
            mainballance.textContent = pr.trim()
            gettotalprofit();
            getmystock();
        },

        // Error handling
        error: function (error) {
            console.log(`Error ${error}`);
        }
    });


}
getuserballance();
setInterval(getuserballance, 2000);

function Moneyinstock() {
    let pr
    let moneyinstock = document.getElementById("money-instock")
/*    let loader = document.getElementById("loader-before")*/

    $.ajax({

        // Our sample url to make request
        url: '/stock/Moneyinstock',


        // Type of Request
        type: "GET",

        // Function to call when to
        // request is ok
        success: function (data) {
            if (data >= null) {
               /* loader.style.display = "none";*/
                pr = "$" + data;
                moneyinstock.textContent = pr.trim()
            }
            else {

            }

        },

        // Error handling
        error: function (error) {
            console.log(`Error ${error}`);
        }
    });



}


setInterval(Moneyinstock, 8000);


function EquityCalculator() {
    let pr
    let equty = document.getElementById("equity-calc")


    $.ajax({

        // Our sample url to make request
        url: '/stock/EquityCalculator',


        // Type of Request
        type: "GET",

        // Function to call when to
        // request is ok
        success: function (data) {
            pr = "$" + data;
            equty.textContent = pr.trim()
        },

        // Error handling
        error: function (error) {
            console.log(`Error ${error}`);
        }
    });



}

/*EquityCalculator();*/

setInterval(EquityCalculator, 3000);


function gettotalprofit() {
    let pr

    let mainprofit = document.getElementById("total-proft")
    /*let loader = document.getElementById("loader-before2")*/
    $.ajax({

        // Our sample url to make request
        url: '/stock/totalprofit',


        // Type of Request
        type: "GET",

        // Function to call when to
        // request is ok
        success: function (data) {
            /*loader.style.display = "none";*/
            console.log(data)

            pr = "$" + data;
            mainprofit.textContent = pr.trim();

            // Check if data is 0 and change text color to red
            if (data >= 0) {
                mainprofit.style.color = 'green';
            } else {
                mainprofit.style.color = 'red'; // Reset to default color if data is not 0
            }
        },


        // Error handling
        error: function (error) {
            console.log(`Error ${error}`);
        }
    });


}

/*gettotalprofit();*/

setInterval(gettotalprofit, 10000);

function getmystock() {

    fetch("/dashboard/getmystock", {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
        },

    })
        .then(response => response.json())
        .then(data => {

            emptytable();


            const tableBody = document.querySelector('#mystock tbody');
            data.forEach(item => {

                const row = document.createElement('tr');
                //row.innerHTML = `
                //        <td>${item.StockName}</td>                        
                //        <td>${item.StockSymbol}</td>
                //        <td>${item.TotalVolume.toFixed(1)}</td>                                                                      
                //        <td>$${item.LivePrice}</td>
                //        <td>$${item.PL.toFixed(2)}</td>     
                //    `;


                const typeCell = document.createElement('td');
                typeCell.textContent = item.Name;
                row.appendChild(typeCell);

                const stockNameCell = document.createElement('td');
                stockNameCell.textContent = item.Symbol;
                row.appendChild(stockNameCell);

                const symbolCell = document.createElement('td');
                symbolCell.textContent = item.TotalVolume;
                row.appendChild(symbolCell);

                const stockShareCell = document.createElement('td');
                stockShareCell.textContent = "$" + item.LivePrice.toFixed(6);
                row.appendChild(stockShareCell);

                const openPriceCell = document.createElement('td');
                if (item.PL.toFixed(2) > 0) {

                    openPriceCell.textContent = `$${item.PL.toFixed(2)}`;
                    openPriceCell.style.color = 'green';
                    row.appendChild(openPriceCell);
                }
                else {

                    openPriceCell.textContent = `$${item.PL.toFixed(2)}`;
                    openPriceCell.style.color = 'red';
                    row.appendChild(openPriceCell);
                }

                tableBody.appendChild(row);

            });

        })
        .catch(error => console.error('Error fetching data:', error));
}

function emptytable() {

    const tableBody = document.querySelector('#mystock tbody');
    tableBody.innerHTML = "";
}

setInterval(getmystock, 4000);

function updatelivevalue() {

    $.ajax({

        // Our sample url to make request
        url: '/yahoo/LiveAsync',


        // Type of Request
        type: "GET",

        // Function to call when to
        // request is ok
        success: function (data) {


        },

        // Error handling
        error: function (error) {
            console.log(`Error ${error}`);
        }
    });


}

updatelivevalue();
setInterval(updatelivevalue, 8000);

function updateprofit() {

    $.ajax({

        // Our sample url to make request
        url: '/yahoo/PL',


        // Type of Request
        type: "GET",

        // Function to call when to
        // request is ok
        success: function (data) {

        },

        // Error handling
        error: function (error) {
            console.log(`Error ${error}`);
        }
    });


}

updateprofit()
setInterval(updateprofit, 3000);


function getmystock() {

    fetch("/dashboard/getmystock", {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
        },

    })
        .then(response => response.json())
        .then(data => {

            emptytable();


            const tableBody = document.querySelector('#mystock tbody');
            data.forEach(item => {

                const row = document.createElement('tr');
    
                const typeCell = document.createElement('td');
                typeCell.textContent = item.name;
                row.appendChild(typeCell);

                const stockNameCell = document.createElement('td');
                stockNameCell.textContent = item.Symbol;
                row.appendChild(stockNameCell);

                const symbolCell = document.createElement('td');
                symbolCell.textContent = item.TotalVolume;
                row.appendChild(symbolCell);

                const stockShareCell = document.createElement('td');
                stockShareCell.textContent = "$" + item.LivePrice.toFixed(6);
                row.appendChild(stockShareCell);

                const openPriceCell = document.createElement('td');
                if (item.PL.toFixed(2) > 0) {

                    openPriceCell.textContent = `$${item.PL.toFixed(2)}`;
                    openPriceCell.style.color = 'green';
                    row.appendChild(openPriceCell);
                }
                else {

                    openPriceCell.textContent = `$${item.PL.toFixed(2)}`;
                    openPriceCell.style.color = 'red';
                    row.appendChild(openPriceCell);
                }

                tableBody.appendChild(row);

            });

        })
        .catch(error => console.error('Error fetching data:', error));
}

function emptytable() {

    const tableBody = document.querySelector('#mystock tbody');
    tableBody.innerHTML = "";
}

setInterval(getmystock, 4000);