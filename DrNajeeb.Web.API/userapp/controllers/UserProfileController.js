(function (app) {

    var UserProfileController = function ($scope, $location, $log, $rootScope, OAuthService, toastr, VideoService, SuportService, FileUploader, CurrentUserService, NotificationsService, UsersService) {

        /****** User history section ********/

        $scope.videos = [];

        $scope.pagingInfo = {
            page: 1,
            itemsPerPage: 10,
            search: '',
            totalItems: 0
        };

        $scope.isinstitutionalaccount = true;

        var init = function () {
            UsersService.isInstitutionalAccount().then(function (data) {
                $scope.isinstitutionalaccount = data;
            }, function (err) {
                console.log(err);
            })
        }
        init();

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

        /************* Change name section **********/

        $scope.fullName = "";
        $scope.showNamedDiv = false;

        $scope.changeName = function (form) {
            if (form.$valid) {
                $scope.nameForm = form;
                console.log($scope.fullName);
                if (!($scope.fullName) || $scope.fullName.length < 3) {
                    toastr.warning("Name must contains at least 3 characters");
                    return false;
                }
                OAuthService.setFullName({ fullName: $scope.fullName })
                    .then(onChangeName, onChangeNameError);
            }
        }

        var onChangeName = function (data) {
            toastr.success("Name Changed Successfully");
            $scope.fullName = "";
            $scope.showNamedDiv = false;
            $scope.nameForm.$setPristine(true);
        }

        var onChangeNameError = function (error) {
            toastr.error("Unable to change name");
        }

        /********** End change name section ***********/


        /****** Change password section *******/

        $scope.changePasswordModel = {
            oldPassword: "",
            newPassword: "",
            confirmPassword: ""
        };
        $scope.showPasswordDiv = false;
        $scope.showChangeImage = false;
        $scope.showProgress = false;

        $scope.changePassword = function (form) {
            if (form.$valid) {
                //var re = /^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).*$/;
                //if (re.test($scope.changePasswordModel.newPassword) == false) {
                //    toastr.warning("Password must contain at least one upper case letter, one lower case letter and one number and minimum 6 characters long.");
                //    return false;
                //}
                $scope.passwordForm = form;
                if (!($scope.changePasswordModel.newPassword) || $scope.changePasswordModel.newPassword.length < 6) {
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
            $scope.passwordForm.$setPristine(true);
        }

        var onChangePasswordError = function (error) {
            toastr.error("Unable to change password");
        }

        /****** End change password section *******/

        /******* Change Image Section ********/

        var uploader = $scope.uploader = new FileUploader({
            url: window.location.protocol + '//' + window.location.host +
                 window.location.pathname + 'api/User/UploadProfilePic',
            headers: {
                "Authorization": "Bearer " + CurrentUserService.profile.token,
            }
        });

        // FILTERS

        uploader.filters.push({
            name: 'extensionFilter',
            fn: function (item, options) {
                var filename = item.name;
                var extension = filename.substring(filename.lastIndexOf('.') + 1).toLowerCase();
                if (extension == "jpg" || extension == "png")
                    return true;
                else {
                    alert('Invalid file format. Please select a file with jpg/png');
                    return false;
                }
            }
        });

        uploader.filters.push({
            name: 'sizeFilter',
            fn: function (item, options) {
                var fileSize = item.size;
                fileSize = parseInt(fileSize) / (1024 * 1024);
                if (fileSize <= 1)
                    return true;
                else {
                    alert('Selected file exceeds the 1MB file size limit. Please choose a new file and try again.');
                    return false;
                }
            }
        });

        uploader.onAfterAddingFile = function (fileItem) {
            $scope.showProgress = true;
        };

        uploader.onErrorItem = function (fileItem, response, status, headers) {
            $scope.showProgress = false;
            toastr.error('We were unable to upload your file. Please try again.');
        };

        uploader.onSuccessItem = function (fileItem, response, status, headers) {
            $scope.uploader.queue = [];
            $scope.uploader.progress = 0;
            $scope.showProgress = false;
            $scope.showChangeImage = false;
            $rootScope.updateUserImage();
            toastr.success('profile picture changed successfully.');
        };

        /****** End Change Image Section ******/


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
                NotificationsService.sendMessage($scope.support.message);
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
            toastr.info("Your message has been sent successfully. Support team will contact you soon.");
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
            if (error.status == 401) {
                return;
            }
            console.log(error);
            toastr.error("unable to fetch old messages");
        }

        $scope.$on("messageReceived", function (event, data) {
            toastr.info("New message received");
            $scope.messages.push({
                messageDateTime: new Date(),
                messageText: data,
                isFromAdmin: true,
                isFromUser: false
            });
        })



        loadMessages();

        /***** End admin support section *****/

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
    };

    UserProfileController.$inject = ["$scope", "$location", "$log", "$rootScope", "OAuthService", "toastr", "VideoService", "SuportService", "FileUploader", "CurrentUserService", "NotificationsService", "UsersService"];
    app.controller("UserProfileController", UserProfileController);

}(angular.module("DrNajeebUser")));