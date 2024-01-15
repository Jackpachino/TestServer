function getuserballance() {
    let pr

    let mainballance = document.getElementById("getballance")

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
        },

        // Error handling
        error: function (error) {
            console.log(`Error ${error}`);
        }
    });


}
getuserballance();