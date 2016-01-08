(function (app) {

    var VideoController = function ($scope, $routeParams, $sce, VideoService, toastr) {

        $scope.video = null;
        $scope.videoId = null;
        $scope.videoSource = null;
        $scope.url = window.location.href;

        $scope.addToFavorites = function (video) {
            VideoService.addToFavorites(video.id).then(onFavoritesAdd, onError);
        }

        $scope.removeFromFavorites = function (video) {
            VideoService.removeFromFavorites(video.id).then(onFavoritesRemove, onError);
        }

        $scope.backToLibrary = function () {
            window.history.back();
        }

        var onFavoritesAdd = function (data) {
            $scope.video.isFavoriteVideo = true;
        }

        var onFavoritesRemove = function (data) {
            $scope.video.isFavoriteVideo = false;
        }

        var onVideo = function (data) {
            $scope.video = data;
            //VideoService.getVimeoVideoDetails(data.vzaarVideoId).then(function (vd) {
            //    $scope.video = data;
            //    setUpPlayer(vd);
            //}, function (err) {
            //    console.log(err);
            //    toastr.error("Unable to load video at this time");
            //})
            $scope.videoId = "vzvd-" + $scope.video.vzaarVideoId;
            $scope.videoSource = $sce.trustAsResourceUrl("//player.vimeo.com/video/" + $scope.video.vzaarVideoId);
            //$scope.videoSource = $sce.trustAsResourceUrl("https://view.vzaar.com/" + $scope.video.vzaarVideoId + "/player?apiOn=true");
        }

        //var setUpPlayer = function (vd) {
        //    var bitrates = {
        //        mp4: [
        //          ['Start', vd.files[0].link_secure],
        //          ['360p', vd.files[0].link_secure],
        //          ['720p', vd.files[1].link_secure]
        //        ]
        //    };
        //    // Then we set our player settings
        //    var settings = {
        //        nav: true,
        //        bitrates: bitrates,
        //        delayToFade: 3000,
        //        width: 640,
        //        height: 360,
        //        skin: 's5',
        //        sharing: true,
        //        poster: vd.pictures.sizes[vd.pictures.sizes.length-1].link,
        //        displayStreams: true
        //    };
        //    // Reference to the wrapper div (unique id)
        //    var element = 'rmpPlayer';
        //    // Create an object based on RadiantMP constructor
        //    var rmp = new RadiantMP(element);
        //    // Initialization ... test your page and done!
        //    rmp.init(settings);
        //}

        var onError = function (error) {
            if (error.status == 400) {
                toastr.error("you are not allowed to view this video")
                window.history.back();
            }
            console.log(error);
        }

        $scope.downloadVideo = function (video) {
            console.log(video);
            VideoService.downloadVideo(video.vzaarVideoId)
                    .then(function (data) {

                        var link = document.createElement("a");
                        link.download = video.name + ".mp4";
                        link.href = data;
                        link.click();
                    })
        }

        function init() {
            var videoId = $routeParams.videoId;
            VideoService.getVideo(videoId).then(onVideo, onError);
            setTimeout(window.scrollTo(0, 0), 100);
            window.scrollTo(0, 0);
        }
        init();

    };

    VideoController.$inject = ["$scope", "$routeParams", "$sce", "VideoService", "toastr"];
    app.controller("VideoController", VideoController);

}(angular.module("DrNajeebUser")));