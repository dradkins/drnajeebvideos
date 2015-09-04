(function (app) {

    var LibraryService = function ($http) {

        var LibraryService = {};

        LibraryService.getCategories = function () {

            return $http.get("/api/category/getUserCategories")
                        .then(function (response) {
                            return response.data;
                        });

        };

        LibraryService.getVideos = function (id) {

            return $http.get("/api/videos/getByCategory/" + id)
                        .then(function (response) {
                            return response.data;
                        });

        };

        return LibraryService;

    }

    LibraryService.$inject = ["$http"];
    app.factory("LibraryService", LibraryService);

}(angular.module("DrNajeebUser")));