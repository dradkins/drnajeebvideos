﻿(function (app) {

    var VideoController = function ($scope, $routeParams, $sce, $location, VideoService, toastr, localStorageService) {

        $scope.video = null;
        $scope.videoId = null;
        $scope.videoSource = null;
        $scope.url = window.location.href;
        $scope.videoLink = "";
        $scope.posterLink = "";

        var watchedVideoId = null;
        var lastSeekTime = 0;
        var urlResume = false;


        //for saving current state of video
        $scope.$on('$destroy', function () {

            var currentVideoTime = mediaPlayer.getCurrentTime();
            var videoDuration = mediaPlayer.getDuration();

            if (videoDuration && currentVideoTime != 0) {
                if (currentVideoTime != videoDuration) {
                    VideoService.saveVideoTime({ watchedVideoId: watchedVideoId, currentTime: currentVideoTime }).then(function (data) {
                        console.log('Saved Successfully');
                    }, function (err) {
                        console.log('Unable to save current time');
                    });
                }
            }

            console.log(mediaPlayer.getCurrentTime());
            console.log('page changed');
        });

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
                watchedVideoId = data.watchedVideoId;
                lastSeekTime = data.lastSeekTime;
                setUpPlayer(vd);
            }, function (err) {
                console.log(err);
                toastr.error("Unable to load video at this time");
            })
            //$scope.videoSource = $sce.trustAsResourceUrl("//player.vimeo.com/video/" + $scope.video.vzaarVideoId);
            //$scope.videoSource = $sce.trustAsResourceUrl("https://view.vzaar.com/" + $scope.video.vzaarVideoId + "/player?apiOn=true");
        }

        var mediaPlayer;

        var setUpPlayer = function (vd) {

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
                poster: vd.pictures.sizes[vd.pictures.sizes.length - 1].link,
                displayStreams: true
            };
            var element = 'rmPlayer';
            mediaPlayer = new RadiantMP(element);
            mediaPlayer.init(settings);

            var rmpContainer = document.getElementById(element);
            rmpContainer.addEventListener('ready', playerReady);
        }


        var playerReady = function () {
            console.log("Player is ready");
            if (lastSeekTime != 0) {
                if (!urlResume) {
                    bootbox.confirm({
                        title: 'Resume Video',
                        message: 'Start video from where you last left it..?',
                        backdrop: true,
                        buttons: {
                            'cancel': {
                                label: 'Start Over',
                                className: 'btn-default'
                            },
                            'confirm': {
                                label: 'Resume',
                                className: 'btn-info'
                            }
                        },
                        callback: function (result) {
                            if (result) {
                                mediaPlayer.play();
                                mediaPlayer.seekTo(lastSeekTime)
                            }
                        }
                    });
                }
                else {
                    mediaPlayer.play();
                    mediaPlayer.seekTo(lastSeekTime)
                }
            }
        }

        var onError = function (error) {
            if (error.status == 400) {
                toastr.error("you are not allowed to view this video or your account is expired. please purchase a package to view this video.")
                $location.path("/packages");
                //window.history.back();
            }
            console.log(error);
        }

        $scope.downloadVideo = function (video) {
            //VideoService.getVideoTotalDownloads(video.id).then(function (data) {
            //    if (data == 2) {
            //        toastr.info("The video reached the maximum download limit. Please contact customer support for assistance.")
            //    }
            //    else {
            VideoService.saveDownloadStats(video.id).then(function (data) {
                console.log(data);
            }, function (err) {
                console.log(err);
            });
            VideoService.downloadVideo(video.vzaarVideoId).then(function (data) {
                var link = document.createElement("a");
                link.download = video.name + ".mp4";
                link.href = data;
                document.body.appendChild(link);
                link.click();
            })
            //    }

            //}, function (err) {
            //    console.log(err);
            //})
        }

        function init() {
            var videoId = $routeParams.videoId;
            if ($routeParams.seekTime) {
                lastSeekTime = $routeParams.seekTime;
                urlResume = true;
            }
            VideoService.getVideo(videoId).then(onVideo, onError);
            setTimeout(window.scrollTo(0, 0), 100);
            window.scrollTo(0, 0);
        }
        init();

    };

    VideoController.$inject = ["$scope", "$routeParams", "$sce", "$location", "VideoService", "toastr", "localStorageService"];
    app.controller("VideoController", VideoController);

}(angular.module("DrNajeebUser")));