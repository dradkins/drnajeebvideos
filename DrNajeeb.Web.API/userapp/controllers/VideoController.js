(function (app) {

    var VideoController = function ($scope, $routeParams, $sce, VideoService, toastr, localStorageService) {

        $scope.video = null;
        $scope.videoId = null;
        $scope.videoSource = null;
        $scope.url = window.location.href;
        $scope.videoLink = "";
        $scope.posterLink = "";

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
            //$scope.video = data;
            VideoService.getVimeoVideoDetails(data.vzaarVideoId).then(function (vd) {
                $scope.video = data;
                //$scope.videoId = "vzvd" + $scope.video.vzaarVideoId;
                setUpPlayer(vd);
            }, function (err) {
                console.log(err);
                toastr.error("Unable to load video at this time");
            })
            //$scope.videoSource = $sce.trustAsResourceUrl("//player.vimeo.com/video/" + $scope.video.vzaarVideoId);
            //$scope.videoSource = $sce.trustAsResourceUrl("https://view.vzaar.com/" + $scope.video.vzaarVideoId + "/player?apiOn=true");
        }

        var setUpPlayer = function (vd) {
            //$scope.videoLink = $sce.trustAsResourceUrl(vd.files[0].link_secure);
            //$scope.posterLink = vd.pictures.sizes[vd.pictures.sizes.length - 1].link;
            //console.log($scope.videoId);
            //var vID = $scope.videoId;
            //console.log(angular.element(document.querySelector("#" + vID + "")));
            //videojs("drVideo", {
            //    "aspectRatio": "840:475",
            //    "playbackRates": [1, 1.5, 2]
            //}, function () {
            //    myPlayer = this;
            //});


            var bitrates = {
                mp4: [
                  ['Start', vd.files[0].link_secure],
                  ['360p', vd.files[0].link_secure],
                  ['720p', vd.files[1].link_secure]
                ]
            };
            var settings = {
                nav: true,
                bitrates: bitrates,
                delayToFade: 3000,
                width: 840,
                height: 475,
                skin: 's5',
                sharing: false,
                poster: vd.pictures.sizes[vd.pictures.sizes.length-1].link,
                displayStreams: true
            };
            var element = 'rmPlayer';
            var rmp = new RadiantMP(element);
            rmp.init(settings);
        }

        //var myPlayer;
        //$scope.$on('$destroy', function () {
        //    // Destroy the object if it exists
        //    if ((myPlayer !== undefined) && (myPlayer !== null)) {
        //        myPlayer.dispose();
        //    }
        //});

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
                        document.body.appendChild(link);
                        link.click();
                    })
        }

        function init() {
            //if (window.localStorage) {
            //    if (!localStorage.getItem('firstLoad')) {
            //        localStorage['firstLoad'] = true;
            //        window.location.reload();
            //    }
            //    else
            //        localStorage.removeItem('firstLoad');
            //}
            var videoId = $routeParams.videoId;
            VideoService.getVideo(videoId).then(onVideo, onError);
            setTimeout(window.scrollTo(0, 0), 100);
            window.scrollTo(0, 0);
        }
        init();

    };

    VideoController.$inject = ["$scope", "$routeParams", "$sce", "VideoService", "toastr", "localStorageService"];
    app.controller("VideoController", VideoController);

}(angular.module("DrNajeebUser")));