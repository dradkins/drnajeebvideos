(function (app) {

    var GhostUsersController = function ($scope, ReportsService, toastr) {

        $scope.pagingInfo = {
            page: 1,
            itemsPerPage: 20,
            sortBy: 'createdOn',
            reverse: true,
            search: '',
            totalItems: 0,
            dateFrom: new Date()
        };

        $scope.users = [];

        $scope.selectPage = function (page) {
            $scope.pagingInfo.page = page;
            loadUsers();
        };

        $scope.search = function () {
            $scope.pagingInfo.page = 1;
            loadUsers();
        };

        $scope.sort = function (sortBy) {
            if (sortBy === $scope.pagingInfo.sortBy) {
                $scope.pagingInfo.reverse = !$scope.pagingInfo.reverse;
            } else {
                $scope.pagingInfo.sortBy = sortBy;
                $scope.pagingInfo.reverse = false;
            }
            $scope.pagingInfo.page = 1;
            loadUsers();
        };

        $scope.getReport = function () {
            loadUsers();
        }

        var onUsers = function (data) {
            $scope.users = null;
            $scope.users = data.data;
            $scope.pagingInfo.totalItems = data.count;
        }

        var onError = function (error) {
            toastr.error("Unable to load users at this time");
            console.info("Error in GhostUserController");
            console.error(error);
        }

        var loadUsers = function () {
            ReportsService.getGhostUsers($scope.pagingInfo).then(onUsers, onError);
        }

        loadUsers();

    };

    GhostUsersController.$inject = ["$scope", "ReportsService", "toastr"];
    app.controller("GhostUsersController", GhostUsersController);

}(angular.module("DrNajeebAdmin")));