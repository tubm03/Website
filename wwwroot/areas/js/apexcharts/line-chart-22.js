let tfLineChart;
(function ($) {

    tfLineChart = (function () {
        var chart;
        var chartBar = function (days, dataProduct, dataService) {

            var options = {
                series: [{
                    name: 'Sản Phẩm',
                    data: dataProduct
                }, {
                    name: 'Dịch vụ',
                    data: dataService
                }],
                chart: {
                    height: 373,
                    type: 'area',
                    toolbar: {
                        show: false,
                    },
                },
                dataLabels: {
                    enabled: false
                },
                legend: {
                    show: false,
                },
                colors: ['#2377FC', '#8D79F6'],
                stroke: {
                    curve: 'smooth'
                },
                yaxis: {
                    //show: false,
                    labels: {
                        style: {
                            colors: '#95989D',
                            fontSize: '13px'
                        },
                        formatter: function (value) {
                            return value.toLocaleString('en-US') + ' đ';
                        }
                    }
                },
                xaxis: {
                    labels: {
                        style: {
                            colors: '#95989D',
                        },
                    },
                    categories: days
                },
                tooltip: {
                    x: {
                        format: 'dd/mm/yy'
                    },
                    y: {
                        formatter: function (value) {
                            return value.toLocaleString('en-US') + 'đ';
                        }
                    }
                },
            };

            if (chart) {
                chart.updateOptions(options);
            } else {
                chart = new ApexCharts(document.querySelector("#line-chart-22"), options);
                chart.render();
            }

        };

        /* Function ============ */
        return {
            init: function () { },

            load: function (days, dataProduct, dataService) {
                chartBar(days, dataProduct, dataService);
            },
            resize: function () { },
        };
    })();

    //jQuery(document).ready(function () { });

    jQuery(window).on("load", function () {
        var currentDate = new Date();
        var currentMonth = currentDate.getMonth() + 1;
        var currentYear = currentDate.getFullYear();
        getData(currentMonth, currentYear);
    });

    jQuery(window).on("resize", function () { });

})(jQuery);

