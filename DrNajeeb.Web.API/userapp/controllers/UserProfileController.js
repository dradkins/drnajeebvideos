(function (app) {

    var UserProfileController = function ($scope, $location, $log, OAuthService, toastr, VideoService, SuportService) {

        /****** User history section ********/

        $scope.videos = [];

        $scope.pagingInfo = {
            page: 1,
            itemsPerPage: 10,
            search: '',
            totalItems: 0
        };

        $scope.search = function () {
            $scope.pagingInfo.page = 1;
            loadVideos();
        };

        $scope.sort = function (sortBy) {
            if (sortBy === $scope.pagingInfo.sortBy) {
                $scope.pagingInfo.reverse = !$scope.pagingInfo.reverse;
            } else {
                $scope.pagingInfo.sortBy = sortBy;
                $scope.pagingInfo.reverse = false;
            }
            $scope.pagingInfo.page = 1;
            loadVideos();
        };

        $scope.selectPage = function (page) {
            $scope.pagingInfo.page = page;
            loadVideos();
        };

        $scope.showVideo = function (video) {
            $location.path('/video/' + video.id);
        }

        $scope.addToFavorites = function (video) {
            VideoService.addToFavorites(video.id).then(onFavoritesAdd, onError);
        }

        $scope.removeFromFavorites = function (video) {
            VideoService.removeFromFavorites(video.id).then(onFavoritesRemove, onError);
        }

        var onFavoritesAdd = function (data) {
            angular.forEach($scope.videos, function (vid) {
                if (vid.id == data) {
                    vid.isFavoriteVideo = true;
                    return false;
                }
            })
        }

        var onFavoritesRemove = function (data) {
            angular.forEach($scope.videos, function (vid) {
                if (vid.id == data) {
                    vid.isFavoriteVideo = false;
                    return false;
                }
            })
        }

        var onVideos = function (data) {
            $scope.videos = data.data;
            $scope.pagingInfo.totalItems = data.count;
        }

        var loadVideos = function (cat) {
            VideoService.getHistoryVideos($scope.pagingInfo).then(onVideos, onError);
        }

        var onError = function (error) {
            $log.info("Error in NewVideosController");
            $log.error(error);
        }

        loadVideos();

        /****** End user history section ********/


        /****** Change password section *******/

        $scope.changePasswordModel = {
            oldPassword: "",
            newPassword: "",
            confirmPassword: ""
        };
        $scope.showPasswordDiv = false;

        $scope.changePassword = function (form) {
            if (form.$valid) {
                //var re = /^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).*$/;
                //if (re.test($scope.changePasswordModel.newPassword) == false) {
                //    toastr.warning("Password must contain at least one upper case letter, one lower case letter and one number and minimum 6 characters long.");
                //    return false;
                //}
                if (!($scope.changePasswordModel.newPassword) || $scope.changePasswordModel.newPassword.length < 6 ) {
                    toastr.warning("Password must contains at least 6 characters");
                    return false;
                }
                if ($scope.changePasswordModel.newPassword !== $scope.changePasswordModel.confirmPassword) {
                    toastr.warning("New password and confirm password not matched.");
                    return false;
                }
                OAuthService.changePassword($scope.changePasswordModel)
                                .then(onChangePassword, onChangePasswordError);
            }
        }

        var onChangePassword = function (data) {
            toastr.success("Password Changed Successfully");
            $scope.changePasswordModel.oldPassword = "";
            $scope.changePasswordModel.newPassword = "";
            $scope.changePasswordModel.confirmPassword = "";
            $scope.showPasswordDiv = false;
        }

        var onChangePasswordError = function (error) {
            toastr.error("Unable to change password");
        }

        /****** End change password section *******/


        /***** Admin support section *******/

        //$scope.messages = [];

        $scope.support = {
            subject: "",
            message: "",
        };
        $scope.form = null;

        $scope.sendMessage = function (form) {
            $scope.form = form;
            if (form.$valid) {
                SuportService.sendMessage($scope.support)
                            .then(onMessageSend, onMessageError);
            }
            else {
                toastr.warning("The data you provided is not valid. Please verify data and send again.")
            }
        };

        var onMessageSend = function (data) {
            $scope.support.message = "";
            $scope.support.subject = "";
            $scope.form.$setPristine(true);
            $scope.messages.push(data);
            toastr.info("your message received successfully. Thanks for you valuable feedback, we will contact you as soon as possible");
        }

        var onMessageError = function (error) {
            console.log(error);
            toastr.error("unable to send message at this time");
        }

        var loadMessages = function () {
            SuportService.getMessages().then(onMessagesArrive, onMessagesArriveError);
        }

        var onMessagesArrive = function (data) {
            $scope.messages = data;
        }

        var onMessagesArriveError = function (error) {
            console.log(error);
            toastr.error("unable to fetch old messages");
        }

        loadMessages();

        /***** End admin support section *****/
    };

    UserProfileController.$inject = ["$scope", "$location", "$log", "OAuthService", "toastr", "VideoService", "SuportService"];
    app.controller("UserProfileController", UserProfileController);

}(angular.module("DrNajeebUser")));