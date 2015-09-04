'use strict';

(function (app) {

    var UserController = function ($scope, $modal, $log, UserService) {

        $scope.user = {
            id: 0,
            name: null,
            description: null,
            price: null,
            startDate: null,
            endDate: null,
            isActiveUser: true,
            timeDurationInDays: 0,
            gatewayId: null
        };

        $scope.pagingInfo = {
            page: 1,
            itemsPerPage: 20,
            sortBy: 'name',
            reverse: false,
            search: '',
            totalItems: 0
        };

        $scope.addOrEditModal = function (user) {

            var modalInstance = $modal.open({
                animation: true,
                templateUrl: 'addoredituser.html',
                controller: 'AddOrEditUserController',
                size: 'lg',
                resolve: {
                    user: function () {
                        if (user) {
                            return angular.copy(user);
                        }
                        else {
                            return angular.copy($scope.user);
                        }
                    }
                }
            });

            modalInstance.result.then(function () {
                loadUsers();
            }, function () {
                $log.info('Modal dismissed at: ' + new Date());
            });
        };

        $scope.users = [];

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

        $scope.selectPage = function (page) {
            $scope.pagingInfo.page = page;
            loadUsers();
        };

        $scope.deleteUser = function (sub) {
            if (confirm("Are you sure to delete this user..?")) {
                UserService.deleteUser(sub.id).then(function (response) {
                    $scope.users.splice($scope.users.indexOf(sub), 1);
                }, onError);
            }
        }

        var onError = function (error) {
            console.info("Error in UserController");
            console.error(error);
        }

        var onadd = function (data) {
            loadUsers();
        }

        var onUsers = function (data) {
            $scope.users = null;
            $scope.users = data.data;
            $scope.pagingInfo.totalItems = data.count;
        }

        var loadUsers = function () {
            UserService.getUsers($scope.pagingInfo).then(onUsers, onError);
        }

        loadUsers();
    };

    UserController.$inject = ["$scope", "$modal", "$log", "UserService"];



    var AddOrEditUserController = function ($scope, $modalInstance, $filter, UserService, user) {

        $scope.user = user;

        $scope.startDate;
        $scope.endDate;

        function init() {
            if (user.startDate) {
                $scope.startDate = $filter('date')(user.startDate, 'dd-MMMM-yyyy');
            }
            if (user.endDate) {
                $scope.endDate = $filter('date')(user.endDate, 'dd-MMMM-yyyy');
            }
        }

        init();

        $scope.ok = function () {
            if ($scope.startDate) {
                $scope.user.startDate = $filter('date')($scope.startDate, 'yyyy-MM-ddTHH:mm:ss.sssZ');
            }
            if ($scope.endDate) {
                $scope.user.endDate = $filter('date')($scope.endDate, 'yyyy-MM-ddTHH:mm:ss.sssZ');
            }
            if (user.id != 0) {
                UserService.editUser($scope.user).then(function (response) {
                    $modalInstance.close();
                })
            }
            else {
                UserService.addUser($scope.user).then(function (response) {
                    $modalInstance.close();
                })
            }
        };

        $scope.cancel = function () {
            $modalInstance.dismiss('cancel');
        };
    }

    AddOrEditUserController.$inject = ["$scope", "$modalInstance", "$filter", "UserService", "user"]


    app.controller("UserController", UserController);

    app.controller('AddOrEditUserController', AddOrEditUserController);

}(angular.module("DrNajeebAdmin")));