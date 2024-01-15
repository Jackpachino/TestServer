$(document).ready(function () {
    var allCryptos = [];

    // Fetch and store the cryptocurrency list
    $.ajax({
        url: '/currencies/getcurrencylist',
        type: 'GET',
        success: function (data) {
            allCryptos = data;
        },
        error: function (xhr, status, error) {
            console.error('Error fetching data:', error);
        }
    });

    // Function to display filtered results based on the search term
    function displayResults(filter) {
        var resultList = $('#dropdown-abc123');
        resultList.empty();

        var filteredCryptos = filter ?
            allCryptos.filter(item => item.Name.toLowerCase().includes(filter.toLowerCase())) :
            allCryptos;

        filteredCryptos.forEach(item => {
            $('<div>').addClass('dropdown-item').text(item.Name).appendTo(resultList);
        });
    }

    // Event delegation for click event on dropdown items
    $('#dropdown-abc123').on('click', '.dropdown-item', function () {
        var selectedName = $(this).text();
        var selectedCrypto = allCryptos.find(crypto => crypto.Name === selectedName);
        $('#search-abc123').val(selectedName);
        $('#dropdown-abc123').empty().hide();

        // Send the selected item's CryptoSymbol to the server
        $.ajax({
            url: '/currencies/selectcurrency',
            type: 'POST',
            data: { cryptoSymbol: selectedCrypto.CurrencySymbol },
            success: function (response) {
                console.log(response)
                if (response == "True" || response == "1") {

                    console.log('Currency selected successfully:', response);
                    location.reload();
                }
            },
            error: function (xhr, status, error) {
                console.error('Error selecting crypto:', error);
            }
        });
    });

    // Event listener for the search input
    $('#search-abc123').on('keyup', function () {
        var searchTerm = $(this).val();
        displayResults(searchTerm);
        if (searchTerm.length === 0) {
            $('#dropdown-abc123').empty().hide();
        } else {
            $('#dropdown-abc123').show();
        }
    });

    // Hide dropdown when clicking outside
    $(document).on('click', function (e) {
        if (!$(e.target).closest('#currenciessearch').length) {
            $('#dropdown-abc123').empty().hide();
        }
    });
});
