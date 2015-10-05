/// <reference path="C:\Users\Xavier\Desktop\DrNajeeb\DrNajeeb.Web.API\Scripts/js/skycons.js" />
(function (app) {

    var DashboardController = function ($scope, DashboardService) {

        $scope.dashboardData = {
            totalUsers: 0,
            totalVideos: 0,
            totalMessages:0,
            latestUsers: []
        };

        $scope.search = "";

        var onDashboardData = function (response) {
            $scope.dashboardData.totalUsers = response[0].data;
            $scope.dashboardData.totalVideos = response[1].data;
            $scope.dashboardData.latestUsers = response[2].data;
            console.log(response[3]);
            $scope.dashboardData.totalMessages = response[3].data;
        }

        var onError = function (error) {
            console.log(data);
        }

        var drawCharts = function () {

            var revenuesValues = [8, 3, 2, 6, 4, 9, 1, 10, 8, 2, 5, 8, 6, 9, 3, 4, 2, 3, 7];

            $('.spark-revenues').sparkline(revenuesValues, {
                type: 'bar',
                barColor: '#ced9e2',
                height: 25,
                barWidth: 6
            });

            //CHARTS
            function gd(year, day, month) {
                return new Date(year, month - 1, day).getTime();
            }

            if ($('#graph-bar').length) {
                var data1 = [
                    [gd(2015, 1, 1), 838], [gd(2015, 1, 2), 749], [gd(2015, 1, 3), 634], [gd(2015, 1, 4), 1080], [gd(2015, 1, 5), 850], [gd(2015, 1, 6), 465], [gd(2015, 1, 7), 453], [gd(2015, 1, 8), 646], [gd(2015, 1, 9), 738], [gd(2015, 1, 10), 899], [gd(2015, 1, 11), 830], [gd(2015, 1, 12), 789]
                ];

                var data2 = [
                    [gd(2015, 1, 1), 342], [gd(2015, 1, 2), 721], [gd(2015, 1, 3), 493], [gd(2015, 1, 4), 403], [gd(2015, 1, 5), 657], [gd(2015, 1, 6), 782], [gd(2015, 1, 7), 609], [gd(2015, 1, 8), 543], [gd(2015, 1, 9), 599], [gd(2015, 1, 10), 359], [gd(2015, 1, 11), 783], [gd(2015, 1, 12), 680]
                ];

                var series = new Array();

                series.push({
                    data: data1,
                    bars: {
                        show: true,
                        barWidth: 24 * 60 * 60 * 12000,
                        lineWidth: 1,
                        fill: 1,
                        align: 'center'
                    },
                    label: 'Revenues'
                });
                series.push({
                    data: data2,
                    color: '#e84e40',
                    lines: {
                        show: true,
                        lineWidth: 3,
                    },
                    points: {
                        fillColor: "#e84e40",
                        fillColor: '#ffffff',
                        pointWidth: 1,
                        show: true
                    },
                    label: 'Orders'
                });

                $.plot("#graph-bar", series, {
                    colors: ['#03a9f4', '#f1c40f', '#2ecc71', '#3498db', '#9b59b6', '#95a5a6'],
                    grid: {
                        tickColor: "#f2f2f2",
                        borderWidth: 0,
                        hoverable: true,
                        clickable: true
                    },
                    legend: {
                        noColumns: 1,
                        labelBoxBorderColor: "#000000",
                        position: "ne"
                    },
                    shadowSize: 0,
                    xaxis: {
                        mode: "time",
                        tickSize: [1, "month"],
                        tickLength: 0,
                        // axisLabel: "Date",
                        axisLabelUseCanvas: true,
                        axisLabelFontSizePixels: 12,
                        axisLabelFontFamily: 'Open Sans, sans-serif',
                        axisLabelPadding: 10
                    }
                });

                var previousPoint = null;
                $("#graph-bar").bind("plothover", function (event, pos, item) {
                    if (item) {
                        if (previousPoint != item.dataIndex) {

                            previousPoint = item.dataIndex;

                            $("#flot-tooltip").remove();
                            var x = item.datapoint[0],
                            y = item.datapoint[1];

                            showTooltip(item.pageX, item.pageY, item.series.label, y);
                        }
                    }
                    else {
                        $("#flot-tooltip").remove();
                        previousPoint = [0, 0, 0];
                    }
                });

                function showTooltip(x, y, label, data) {
                    $('<div id="flot-tooltip">' + '<b>' + label + ': </b><i>' + data + '</i>' + '</div>').css({
                        top: y + 5,
                        left: x + 20
                    }).appendTo("body").fadeIn(200);
                }
            }
        }

        function init() {
            drawCharts();
            DashboardService.getDashboardData().then(onDashboardData, onError);
        }

        init()
    };

    DashboardController.$inject = ["$scope", "DashboardService"];
    app.controller("DashboardController", DashboardController);

}(angular.module("DrNajeebAdmin")));