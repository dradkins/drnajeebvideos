/// <reference path="C:\Users\Xavier\Desktop\DrNajeeb\DrNajeeb.Web.API\Scripts/js/skycons.js" />
(function (app) {

    var RevenueController = function ($scope, toastr, ReportsService) {

        $scope.pagingInfo = {
            dateTo: new Date(),
            dateFrom: new Date(new Date() - (24 * 60 * 60 * 1000)),
        };

        $scope.totalRevenue = 0;
        $scope.subscriptions = [];

        $scope.getReport = function () {
            loadRevenue();
        }

        var onRevenue = function (response) {
            $scope.totalRevenue = response.totalRevenue;
            $scope.subscriptions = response.subscriptions;
        }

        var onError = function (error) {
            console.log(error);
        }


        var loadRevenue = function () {
            ReportsService.getRevenue($scope.pagingInfo).then(onRevenue, onError);
        }

        loadRevenue()
    };

    RevenueController.$inject = ["$scope", "toastr", "ReportsService"];
    app.controller("RevenueController", RevenueController);

}(angular.module("DrNajeebAdmin")));