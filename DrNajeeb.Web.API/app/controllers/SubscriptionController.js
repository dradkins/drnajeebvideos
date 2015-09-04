'use strict';

(function (app) {

    var SubscriptionController = function ($scope, $modal, $log, $timeout, SubscriptionService) {

        $scope.dt = new Date();

        $scope.open = function () {
            $timeout(function () {
                $scope.opened = true;
            });
        };

        $scope.subscription = {
            id: 0,
            name: null,
            description: null,
            price: null,
            startDate: null,
            endDate: null,
            isActiveSubscription: true,
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

        $scope.addOrEditModal = function (subscription) {

            var modalInstance = $modal.open({
                animation: true,
                templateUrl: 'addoreditsubscription.html',
                controller: 'AddOrEditSubscriptionController',
                //size: 'lg',
                resolve: {
                    subscription: function () {
                        if (subscription) {
                            return angular.copy(subscription);
                        }
                        else {
                            return angular.copy($scope.subscription);
                        }
                    }
                }
            });

            modalInstance.result.then(function () {
                loadSubscription();
            }, function () {
                $log.info('Modal dismissed at: ' + new Date());
            });
        };

        $scope.Subscriptions = [];

        $scope.search = function () {
            $scope.pagingInfo.page = 1;
            loadSubscription();
        };

        $scope.sort = function (sortBy) {
            if (sortBy === $scope.pagingInfo.sortBy) {
                $scope.pagingInfo.reverse = !$scope.pagingInfo.reverse;
            } else {
                $scope.pagingInfo.sortBy = sortBy;
                $scope.pagingInfo.reverse = false;
            }
            $scope.pagingInfo.page = 1;
            loadSubscription();
        };

        $scope.selectPage = function (page) {
            $scope.pagingInfo.page = page;
            loadSubscription();
        };

        $scope.deleteSubscription = function (sub) {
            if (confirm("Are you sure to delete this subscription..?")) {
                SubscriptionService.deleteSubscription(sub.id).then(function (response) {
                    $scope.subscriptions.splice($scope.subscriptions.indexOf(sub), 1);
                }, onError);
            }
        }

        var onError = function (error) {
            console.info("Error in SubscriptionController");
            console.error(error);
        }

        var onadd = function (data) {
            loadSubscription();
        }

        var onSubscriptions = function (data) {
            $scope.subscriptions = null;
            $scope.subscriptions = data.data;
            $scope.pagingInfo.totalItems = data.count;
        }

        var loadSubscription = function () {
            SubscriptionService.getSubscriptions($scope.pagingInfo).then(onSubscriptions, onError);
        }

        loadSubscription();
    };

    SubscriptionController.$inject = ["$scope", "$modal", "$log", "$timeout", "SubscriptionService"];



    var AddOrEditSubscriptionController = function ($scope, $modalInstance, $filter, SubscriptionService, subscription) {

        $scope.subscription = subscription;

        $scope.startDate;
        $scope.endDate;

        function init() {
            if (subscription.startDate) {
                $scope.startDate = $filter('date')(subscription.startDate, 'dd-MMMM-yyyy');
            }
            if (subscription.endDate) {
                $scope.endDate = $filter('date')(subscription.endDate, 'dd-MMMM-yyyy');
            }
        }

        init();

        //var datediff = function (date1, date2, interval) {
        //    var second = 1000, minute = second * 60, hour = minute * 60, day = hour * 24, week = day * 7;
        //    date1 = $filter('date')(date1, 'yyyy-MM-ddTHH:mm:ss.sssZ');
        //    date2 = $filter('date')(date2, 'yyyy-MM-ddTHH:mm:ss.sssZ');
        //    console.log(date1);
        //    console.log(date2);
        //    var timediff = date2 - date1;
        //    if (isNaN(timediff)) return NaN;
        //    switch (interval) {
        //        case "years": return date2.getFullYear() - date1.getFullYear();
        //        case "months": return (
        //            (date2.getFullYear() * 12 + date2.getMonth())
        //            -
        //            (date1.getFullYear() * 12 + date1.getMonth())
        //        );
        //        case "weeks": return Math.floor(timediff / week);
        //        case "days": return Math.floor(timediff / day);
        //        case "hours": return Math.floor(timediff / hour);
        //        case "minutes": return Math.floor(timediff / minute);
        //        case "seconds": return Math.floor(timediff / second);
        //        default: return undefined;
        //    }
        //}

        $scope.ok = function () {
            if ($scope.startDate) {
                $scope.subscription.startDate = $filter('date')($scope.startDate, 'yyyy-MM-ddTHH:mm:ss.sssZ');
            }
            if ($scope.endDate) {
                $scope.subscription.endDate = $filter('date')($scope.endDate, 'yyyy-MM-ddTHH:mm:ss.sssZ');
            }
            if (subscription.id != 0) {
                SubscriptionService.editSubscription($scope.subscription).then(function (response) {
                    $modalInstance.close();
                })
            }
            else {
                SubscriptionService.addSubscription($scope.subscription).then(function (response) {
                    $modalInstance.close();
                })
            }
        };

        $scope.cancel = function () {
            $modalInstance.dismiss('cancel');
        };
    }

    AddOrEditSubscriptionController.$inject = ["$scope", "$modalInstance", "$filter", "SubscriptionService", "subscription"]


    app.controller("SubscriptionController", SubscriptionController);

    app.controller('AddOrEditSubscriptionController', AddOrEditSubscriptionController);

}(angular.module("DrNajeebAdmin")));