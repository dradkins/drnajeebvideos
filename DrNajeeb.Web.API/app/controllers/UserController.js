'use strict';

(function (app) {

    var UserController = function ($scope, $modal, $log, UserService, toastr, $filter) {

        $scope.user = {
            id: 0,
            fullName: null,
            password: null,
            confirmPassword: null,
            isPasswordReset: false,
            noOfConcurrentViews: null,
            isFilterByIP: false,
            isActiveUser: true,
            isFreeUser: false,
            countryId: null,
            subscriptionId: null,
            roles: [],
            filteredIPs: [],
            isInstitutionalAccount:false,
        };

        $scope.pagingInfo = {
            page: 1,
            itemsPerPage: 20,
            sortBy: 'createdOn',
            reverse: true,
            search: '',
            totalItems: 0
        };

        $scope.oldUserName = "";

        $scope.reportDate = new Date();

        $scope.addOrEditModal = function (user) {

            var modalInstance = $modal.open({
                animation: true,
                templateUrl: (user) ? 'edituser.html' : 'adduser.html',
                controller: (user) ? 'EditUserController' : 'AddUserController',
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

        $scope.deleteUser = function (user) {
            if (confirm("Are you sure to delete this user..?")) {
                UserService.deleteUser(user.id).then(function (response) {
                    $scope.users.splice($scope.users.indexOf(user), 1);
                    toastr.info("user deleted successfully");
                }, onDeleteUserError);
            }
        }

        $scope.downloadFile = function (value) {
            if (value) {
                var downloadPath = "/home/GetInactiveUsers/" + $filter('date')($scope.reportDate, "yyyy-MM-dd");
                window.open(downloadPath, '_blank', '');
            }
            else {
                var downloadPath = "/home/GetAllInactiveUsers";
                window.open(downloadPath, '_blank', '');
            }
        }

        $scope.changeUserName = function (oldUserName) {
            $scope.oldUserName = oldUserName;
            var newName = prompt("Please enter new email address");
            if (newName && validateEmail(newName)) {
                UserService.UpdateUserName({ OldEmail: $scope.oldUserName, NewEmail: newName }).then(function () {
                    loadUsers();
                }, function (error) {
                    toastr.error("unable to update user name");
                })
            }
            else {
                alert("Please enter valid email address");
            }
        }

        function validateEmail(email) {
            var re = /^([\w-]+(?:\.[\w-]+)*)@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$/i;
            return re.test(email);
        }

        var onDeleteUserError = function (error) {
            console.log(error);
            if (error.status && error.status === 404) {
                toastr.error("user not found");
            }
        }

        var onError = function (error) {
            console.info("Error in UserController");
            console.error(error);
        }

        var onadd = function (data) {
            toastr.info("user added successfully");
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

    UserController.$inject = ["$scope", "$modal", "$log", "UserService", "toastr", "$filter"];



    var AddUserController = function ($scope, $modalInstance, $filter, UserService, CountryService, SubscriptionService, RolesService, user, toastr) {

        $scope.user = user;
        $scope.countries = [];
        $scope.subscriptions = [];
        $scope.roles = [];


        var onCountries = function (response) {
            $scope.countries = null;
            $scope.countries = response;
        }

        var onSubscriptions = function (response) {
            $scope.subscriptions = null;
            $scope.subscriptions = response.data;
        }

        var onRoles = function (response) {
            $scope.roles = null;
            $scope.roles = response;
        }

        var onUserAddError = function (error) {
            console.log(error);
            var message = "can't able to create user.";
            if (error.data.modelState) {
                message = error.data.modelState.modelError.join("<br />");
            }
            toastr.error(message, error.data.message);
        }

        var onError = function (error) {
            console.log(error);
        }

        function init() {
            CountryService.getAll().then(onCountries, onError);
            SubscriptionService.getSubscriptions().then(onSubscriptions, onError);
            RolesService.getAll().then(onRoles, onError);
        }

        init();

        $scope.removeFilterdIP = function (index) {
            $scope.user.filteredIPs.splice(index, 1);
        }

        $scope.ipAddress = "";

        $scope.addIpAddress = function () {
            if (!ValidateIPaddress($scope.ipAddress)) {
                alert("Ip Address is not valid");
                return false;
            }
            $scope.user.filteredIPs.push($scope.ipAddress);
            $scope.ipAddress = "";
        }

        function ValidateIPaddress(ipaddress) {
            if (/^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$/.test(ipaddress)) {
                return (true)
            }
            return (false)
        }

        $scope.ok = function () {
            //var re = /^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).*$/;
            //if (re.test($scope.user.password) == false) {
            //    toastr.warning("Password must contain at least one upper case letter, one lower case letter and one number and minimum 6 characters long.");
            //    return false;
            //}
            //console.log(user);

            UserService.addUser($scope.user).then(function (response) {
                toastr.info("user added successfully");
                $modalInstance.close();
            }, onUserAddError);
        };

        $scope.cancel = function () {
            $modalInstance.dismiss('cancel');
        };
    }

    var EditUserController = function ($scope, $modalInstance, $filter, UserService, CountryService, SubscriptionService, RolesService, user, toastr) {

        $scope.user = user;
        $scope.countries = [];
        $scope.subscriptions = [];
        $scope.roles = [];
        $scope.selectedCountry;
        $scope.selectedSubscription;


        var onCountries = function (response) {
            $scope.countries = null;
            $scope.countries = response;
            angular.forEach($scope.countries, function (item) {
                if (item.id == $scope.user.countryID) {
                    $scope.selectedCountry = item;
                    return false;
                }
            });
        }

        var onSubscriptions = function (response) {
            $scope.subscriptions = null;
            $scope.subscriptions = response.data;
            angular.forEach($scope.subscriptions, function (item) {
                if (item.id == user.subscriptionID) {
                    $scope.selectedSubscription = item;
                    return false;
                }
            });
            console.log($scope.selectedSubscription);
        }

        var onRoles = function (response) {
            $scope.roles = null;
            $scope.roles = response;
        }

        var onError = function (error) {
            console.log(error);
        }

        var onUserUpdateError = function (error) {
            console.log(error);
            var message = "can't able to create user.";
            if (error.data.modelState) {
                message = error.data.modelState.modelError.join("<br />");
            }
            toastr.error(message, error.data.message);
        }


        function init() {
            console.log(user);
            CountryService.getAll().then(onCountries, onError);
            SubscriptionService.getSubscriptions().then(onSubscriptions, onError);
            RolesService.getAll().then(onRoles, onError);
        }

        init();

        $scope.removeFilterdIP = function (index) {
            $scope.user.filteredIPs.splice(index, 1);
        }

        $scope.ipAddress = "";

        $scope.addIpAddress = function () {
            if (!ValidateIPaddress($scope.ipAddress)) {
                alert("Ip Address is not valid");
                return false;
            }
            $scope.user.filteredIPs.push($scope.ipAddress);
            $scope.ipAddress = "";
        }

        function ValidateIPaddress(ipaddress) {
            if (/^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$/.test(ipaddress)) {
                return (true)
            }
            return (false)
        }

        $scope.ok = function () {
            //if ($scope.user.password) {
            //    var re = /^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).*$/;
            //    if (re.test($scope.user.password) == false) {
            //        toastr.warning("Password must contain at least one upper case letter, one lower case letter and one number and minimum 6 characters long.");
            //        return false;
            //    }
            //}
            $scope.user.countryID = $scope.selectedCountry.id;
            $scope.user.subscriptionID = $scope.selectedSubscription.id;
            UserService.editUser($scope.user).then(function (response) {
                toastr.info("user updated successfully");
                $modalInstance.close();
            }, onUserUpdateError);
        };

        $scope.cancel = function () {
            $modalInstance.dismiss('cancel');
        };
    }

    AddUserController.$inject = ["$scope", "$modalInstance", "$filter", "UserService", "CountryService", "SubscriptionService", "RolesService", "user", "toastr"];
    EditUserController.$inject = ["$scope", "$modalInstance", "$filter", "UserService", "CountryService", "SubscriptionService", "RolesService", "user", "toastr"];


    app.controller("UserController", UserController);

    app.controller('AddUserController', AddUserController);
    app.controller('EditUserController', EditUserController);

}(angular.module("DrNajeebAdmin")));