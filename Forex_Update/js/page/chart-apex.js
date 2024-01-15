// SIMPLE DONUT
$(document).ready(function () {
    var options = {
        chart: {
            height: 250,
            type: 'donut',
        },
        dataLabels: {
            enabled: false,
        },
        legend: {
            position: 'top',
            horizontalAlign: 'center',
            show: true,
        },
        colors: ['var(--chart-color1)', 'var(--chart-color2)', 'var(--chart-color3)', 'var(--chart-color4)'],
        series: [44, 55, 41, 17],
        responsive: [{
            breakpoint: 480,
            options: {
                chart: {
                    width: 200
                },
                legend: {
                    position: 'bottom'
                }
            }
        }]
    }

    var chart = new ApexCharts(
        document.querySelector("#apex-simple-donut"),
        options
    );

    chart.render();
});