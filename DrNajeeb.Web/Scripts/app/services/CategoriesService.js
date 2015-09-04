'use strict';

(function (app) {

    var CategoriesService = function ($http) {

        var CategoriesService = {};

        CategoriesService.getCategories = function (pagingInfo) {
            return $http.get("/api/category/getall", { params: pagingInfo })
                        .then(function (response) {
                            return response.data;
                        });
        };

        CategoriesService.addCategory = function (category) {
            return $http.post("/api/category/addCategory", category)
                        .then(function (response) {
                            return response.data;
                        });
        };

        CategoriesService.editCategory = function (category) {
            return $http.post("/api/category/updateCategory", category)
                        .then(function (response) {
                            return response.data;
                        });
        };

        CategoriesService.deleteCategory = function (id) {
            return $http.delete("/api/category/deleteCategory/" + id)
                        .then(function (response) {
                            return response.data;
                        });
        };

        CategoriesService.showCategory = function (categoryId) {
            return $http.get("/api/category/getSingle", { params: categoryId })
                        .then(function (response) {
                            return response.data;
                        });
        };

        return CategoriesService;
    }

    CategoriesService.$inject = ["$http"];
    app.factory("CategoriesService", CategoriesService);

}(angular.module("DrNajeebAdmin")));