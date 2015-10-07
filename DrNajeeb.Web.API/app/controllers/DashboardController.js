/// <reference path="C:\Users\Xavier\Desktop\DrNajeeb\DrNajeeb.Web.API\Scripts/js/skycons.js" />
(function (app) {

    var DashboardController = function ($scope, $filter, DashboardService) {

        $scope.dashboardData = {
            totalUsers: 0,
            totalVideos: 0,
            totalMessages:0,
            latestUsers: []
        };

        $scope.chartLabels = [];
        $scope.chartSeries = ["Active Users", "InActive Users", "Free Users"];
        $scope.chartData = [];

        $scope.search = "";

        var onDashboardData = function (response) {
            $scope.dashboardData.totalUsers = response[0].data;
            $scope.dashboardData.totalVideos = response[1].data;
            $scope.dashboardData.latestUsers = response[2].data;
            $scope.dashboardData.totalMessages = response[3].data;

            var chartsData = response[4].data;
            var freeUsers = [];
            var activeUsers = [];
            var inActiveUsers = [];

            angular.forEach(chartsData, function (item) {

                $scope.chartLabels.push($filter('date')(item.day, 'mediumDate'));
                freeUsers.push(item.freeUsers);
                activeUsers.push(item.activeUsers);
                inActiveUsers.push(item.inActiveUsers);

            });

            $scope.chartData.push(freeUsers);
            $scope.chartData.push(activeUsers);
            $scope.chartData.push(inActiveUsers);
        }

        var onError = function (error) {
            console.log(data);
        }

        $scope.labels = ['2006', '2007', '2008', '2009', '2010', '2011', '2012'];
        $scope.series = ['Series A', 'Series B'];

        $scope.data = [
          [65, 59, 80, 81, 56, 55, 40],
          [28, 48, 40, 19, 86, 27, 90]
        ];

        function init() {
            DashboardService.getDashboardData().then(onDashboardData, onError);
        }

        init()
    };

    DashboardController.$inject = ["$scope","$filter", "DashboardService"];
    app.controller("DashboardController", DashboardController);

}(angular.module("DrNajeebAdmin")));