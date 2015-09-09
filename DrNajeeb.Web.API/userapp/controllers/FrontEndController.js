(function (app) {

    var FrontEndController = function ($scope, LibraryService) {

        $scope.categories = [];
        $scope.videos = [];
        $scope.selectedCategory = null;

        $scope.categorySelected = function (cat) {
            loadVideos(cat);
        }

        var onCategoryVideos = function (data) {
            $scope.videos = null;
            $scope.videos = data;
        }

        var onCategories = function (data) {
            $scope.categories = data;
            loadVideos($scope.categories[0]);
        }

        var loadVideos = function (cat) {
            $scope.selectedCategory = cat;
            LibraryService.getFrontEndVideos(cat.id).then(onCategoryVideos, onError);
        }

        var onError = function (error) {
            console.log(error);
        }

        function init() {
            LibraryService.getCategories().then(onCategories, onError);
        }

        init();

    };

    FrontEndController.$inject = ["$scope", "LibraryService"];
    app.controller("FrontEndController", FrontEndController);

}(angular.module("DrNajeebUser")));