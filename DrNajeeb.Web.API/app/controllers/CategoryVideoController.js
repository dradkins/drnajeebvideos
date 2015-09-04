(function (module) {

    var CategoryVideoController = function ($scope, $location, $routeParams, VideoService, toastr) {


        $scope.videos = [];
        $scope.categoryName;

        $scope.updateOrder = function () {
            var items = [];
            for (var i = 0; i < $scope.videos.length; i++) {
                items.push({
                    videoId: $scope.videos[i].id,
                    locationNo: i,
                    categoryId: $routeParams.categoryId,
                });
            }
            VideoService.updateOrder(items).then(onUpdateOrder, onError);
        }

        var onUpdateOrder = function (data) {
            toastr.success("Update videos order successfully", "Update Successfull");
        }

        var onVideos = function (data) {
            $scope.videos = data;
        }

        var onError = function (error) {
            if (error.status === 404) {
                toastr.error("Category not found");
            }
            else if (error.status == 204) {
                toastr.error("No videos in cateogry");
            }
            else {
                toastr.error("Some error occured during operation")
            }
        }

        function init() {
            if ($routeParams.categoryId && $routeParams.categoryName) {
                $scope.categoryName = $routeParams.categoryName;
                VideoService.getByCategory($routeParams.categoryId).then(onVideos, onError);
            }
            else {
                $location.path("/categories")
            }
        }
        init();
    }

    CategoryVideoController.$inject = ["$scope", "$location", "$routeParams", "VideoService", "toastr"];
    module.controller("CategoryVideoController", CategoryVideoController);

}(angular.module("DrNajeebAdmin")))