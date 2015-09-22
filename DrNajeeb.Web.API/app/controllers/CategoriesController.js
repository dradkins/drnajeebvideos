'use strict';

(function (app) {

    var CategoriesController = function ($scope, $modal, $log, CategoriesService, toastr) {

        $scope.category = {
            id: 0,
            isShowOnFrontPage: true,
            name: "",
            imageURL: null,
            seoName: null,
            categoryURL: null,
            displayOrder: 0,
        };

        $scope.pagingInfo = {
            page: 1,
            itemsPerPage: 50,
            sortBy: 'displayOrder',
            reverse: false,
            search: '',
            totalItems: 0
        };

        $scope.addOrEditModal = function (category) {

            var modalInstance = $modal.open({
                animation: true,
                templateUrl: 'addoreditcategory.html',
                controller: 'AddOrEditCategoryController',
                //size: 'lg',
                resolve: {
                    category: function () {
                        if (category) {
                            return angular.copy(category);
                        }
                        else {
                            return angular.copy($scope.category);
                        }
                    }
                }
            });

            modalInstance.result.then(function () {
                loadCategories();
            }, function () {
                $log.info('Modal dismissed at: ' + new Date());
            });
        };

        $scope.categories = [];

        $scope.search = function () {
            $scope.pagingInfo.page = 1;
            loadCategories();
        };

        $scope.sort = function (sortBy) {
            if (sortBy === $scope.pagingInfo.sortBy) {
                $scope.pagingInfo.reverse = !$scope.pagingInfo.reverse;
            } else {
                $scope.pagingInfo.sortBy = sortBy;
                $scope.pagingInfo.reverse = false;
            }
            $scope.pagingInfo.page = 1;
            loadCategories();
        };

        $scope.selectPage = function (page) {
            $scope.pagingInfo.page = page;
            loadCategories();
        };

        $scope.addCategory = function () {
            CategoriesService.addCategory($scope.category).then(onadd, onError);
        }

        $scope.deleteCategory = function (cat) {
            if (confirm("Are you sure to delete this category..?")) {
                CategoriesService.deleteCategory(cat.id).then(function (response) {
                    $scope.categories.splice($scope.categories.indexOf(cat), 1);
                }, onError);
            }
        }

        $scope.updateOrder = function () {
            var items = [];
            for (var i = 0; i < $scope.categories.length; i++) {
                items.push({
                    categoryId: $scope.categories[i].id,
                    locationNo: i
                });
            }
            CategoriesService.updateOrder(items).then(onUpdateOrder, onError);
        }

        var onUpdateOrder = function (data) {
            toastr.success("Update categories order successfully", "Update Successfull");
            console.log("Updated Successfully");
        }

        var onError = function (error) {
            console.info("Error in CategoriesController");
            console.error(error);
        }

        var onadd = function (data) {
            loadCategories();
        }

        var onCategories = function (data) {
            $scope.categories = null;
            $scope.categories = data.data;
            $scope.pagingInfo.totalItems = data.count;
        }

        var loadCategories = function () {
            CategoriesService.getCategories($scope.pagingInfo).then(onCategories, onError);
        }

        loadCategories();
    };

    CategoriesController.$inject = ["$scope", "$modal", "$log", "CategoriesService", "toastr"];



    var AddOrEditCategoryController = function ($scope, $modalInstance, CategoriesService, category) {
        $scope.category = category;

        $scope.ok = function () {
            if (category.id != 0) {
                CategoriesService.editCategory($scope.category).then(function (response) {
                    $modalInstance.close();
                })
            }
            else {
                CategoriesService.addCategory($scope.category).then(function (response) {
                    $modalInstance.close();
                })
            }
        };

        $scope.cancel = function () {
            $modalInstance.dismiss('cancel');
        };
    }

    AddOrEditCategoryController.$inject = ["$scope", "$modalInstance", "CategoriesService", "category"]


    app.controller("CategoriesController", CategoriesController);

    app.controller('AddOrEditCategoryController', AddOrEditCategoryController);

}(angular.module("DrNajeebAdmin")));