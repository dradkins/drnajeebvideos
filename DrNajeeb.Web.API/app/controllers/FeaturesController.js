'use strict';

(function (app) {

    var FeaturesController = function ($scope, $modal, $log, FeaturesService, toastr) {

        $scope.addModal = function (category) {

            var modalInstance = $modal.open({
                animation: true,
                templateUrl: 'addFeature.html',
                controller: 'AddFeaturesController',
            });

            modalInstance.result.then(function () {
                loadFeatures();
            }, function () {
                $log.info('Modal dismissed at: ' + new Date());
            });
        };

        $scope.features = [];

        $scope.deleteFeature = function (ft) {
            if (confirm("Are you sure to delete this feature..?")) {
                FeaturesService.deleteFeature(ft.id).then(function (response) {
                    $scope.features.splice($scope.features.indexOf(ft), 1);
                }, onError);
            }
        }

        var onError = function (error) {
            console.info("Error in FeaturesController");
            console.error(error);
        }

        var onadd = function (data) {
            loadFeatures();
        }

        var onFeatures = function (data) {
            $scope.features = null;
            $scope.features = data;
        }

        var loadFeatures = function () {
            FeaturesService.getFeatures().then(onFeatures, onError);
        }

        loadFeatures();
    };

    FeaturesController.$inject = ["$scope", "$modal", "$log", "FeaturesService", "toastr"];



    var AddFeaturesController = function ($scope, $modalInstance, FeaturesService) {
        $scope.feature = {
            id: 0,
            title: ""
        };
        $scope.ok = function () {
            FeaturesService.addFeature($scope.feature).then(function (response) {
                $modalInstance.close();
            })
        };

        $scope.cancel = function () {
            $modalInstance.dismiss('cancel');
        };
    }

    AddFeaturesController.$inject = ["$scope", "$modalInstance", "FeaturesService"]

    app.controller("FeaturesController", FeaturesController);

    app.controller('AddFeaturesController', AddFeaturesController);

}(angular.module("DrNajeebAdmin")));