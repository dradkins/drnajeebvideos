'use strict';

(function (app) {

    var UserStatsController = function ($scope, $filter, toastr, ReportsService) {

        $scope.pagingInfo = {
            page: 1,
            itemsPerPage: 20,
            search: '',
            totalItems: 0,
            isActiveUser: true,
            sortBy: 'createdOn',
            reverse: true,
            dateTo: new Date(),
            dateFrom: new Date(new Date() - (24 * 60 * 60 * 1000)),
        };

        $scope.users = [];
        $scope.reportGet = false;

        $scope.search = function () {
            $scope.pagingInfo.page = 1;
            loadUsers();
        };

        $scope.selectPage = function (page) {
            $scope.pagingInfo.page = page;
            loadUsers();
        };

        $scope.getReport = function () {
            loadUsers();
        }

        $scope.downloadFile = function () {
            var fromDate = $filter('date')($scope.pagingInfo.dateFrom, "yyyy-MM-dd");;
            var toDate = $filter('date')($scope.pagingInfo.dateTo, "yyyy-MM-dd");;
            var isActive = $scope.pagingInfo.isActiveUser;
            var downloadPath = "/home/GetUsersEmails?fromDate=" + fromDate + "&toDate=" + toDate + "&isActive=" + isActive;
            window.open(downloadPath, '_blank', '');
        }

        $scope.downloadAll = function (value) {
            if (value) {
                var downloadPath = "/home/GetAllAtiveUsers";
                window.open(downloadPath, '_blank', '');
            }
            else {
                var downloadPath = "/home/GetAllInactiveUsers";
                window.open(downloadPath, '_blank', '');
            }
        }

        var onError = function (error) {
            console.info("Error in UserStatsController");
            console.error(error);
            toastr.error("Unable to load users at this time");
        }

        var onUsers = function (data) {
            $scope.users = null;
            $scope.reportGet = true;
            $scope.users = data.data;
            $scope.pagingInfo.totalItems = data.count;
        }

        var loadUsers = function () {
            ReportsService.getUsersStatsReport($scope.pagingInfo).then(onUsers, onError);
        }

        loadUsers();
    };

    UserStatsController.$inject = ["$scope", "$filter", "toastr", "ReportsService"];

    app.controller("UserStatsController", UserStatsController);

}(angular.module("DrNajeebAdmin")));