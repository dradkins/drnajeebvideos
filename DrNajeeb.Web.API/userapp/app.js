(function () {

    var app = angular.module("DrNajeebUser", [
        "ngRoute",
        "angular-loading-bar",
        "ui.bootstrap",
        "LocalStorageModule",
        "toastr",
        //"jcs-autoValidate",
        "facebook",
        "directive.g+signin",
        "angularFileUpload",
        "LiveSearch",
        "ngIOS9UIWebViewPatch",
        //"vcRecaptcha"
    ]);

    app.config(function ($routeProvider, FacebookProvider) {

        var myAppId = '168728473460376';

        // You can set appId with setApp method
        // FacebookProvider.setAppId('myAppId');

        /**
         * After setting appId you need to initialize the module.
         * You can pass the appId on the init method as a shortcut too.
         */
        FacebookProvider.init(myAppId);

        $routeProvider
            .when("/dashboard", {
                templateUrl: "/userapp/views/userdashboard.html",
                controller: "DashboardController"
            })
            .when("/video-library", {
                templateUrl: "/userapp/views/front-end.html",
                controller: "FrontEndController"
            })
            .when("/newvideos", {
                templateUrl: "/userapp/views/newvideos.html",
                controller: "NewVideosController"
            })
            .when("/newvideos/:searchTerm", {
                templateUrl: "/userapp/views/newvideos.html",
                controller: "NewVideosController"
            })
            .when("/library", {
                templateUrl: "/userapp/views/library.html",
                controller: "LibraryController"
            })
            .when("/library/:categoryId", {
                templateUrl: "/userapp/views/library.html",
                controller: "LibraryController"
            })
            .when("/video/:videoId", {
                templateUrl: "/userapp/views/video.html",
                controller: "VideoController"
            })
            .when("/profile", {
                templateUrl: "/userapp/views/userprofile.html",
                controller: "UserProfileController"
            })
            .when("/favorites", {
                templateUrl: "/userapp/views/userfavorites.html",
                controller: "UserFavoritesController"
            })
            .when("/clinicalvideos", {
                templateUrl: "/userapp/views/userclinicalvideos.html",
                controller: "ClinicalVideosController"
            })
            .when("/login", {
                templateUrl: "/userapp/views/login.html",
                controller: "LoginController"
            })
            .when("/history", {
                templateUrl: "/userapp/views/history.html",
                controller: "UserProfileController"
            })
            .when("/support", {
                templateUrl: "/userapp/views/support.html",
                controller: "UserProfileController"
            })
            .when("/register", {
                templateUrl: "/userapp/views/register.html",
                controller: "RegisterController"
            })
            .when("/free-register", {
                templateUrl: "/userapp/views/free-register.html",
                controller: "RegisterController"
            })
            .when("/checkout", {
                templateUrl: "/userapp/views/checkout.html",
            })
            .when("/register-external", {
                templateUrl: "/userapp/views/register-external.html",
                controller: "ExternalRegisterController"
            })
            .when("/terms-and-conditions", {
                templateUrl: "/userapp/views/terms-and-conditions.html"
            })
            .when("/privacy-policy", {
                templateUrl: "/userapp/views/privacy-policy.html"
            })
            .when("/free-videos", {
                templateUrl: "/userapp/views/free-videos.html",
                controller: "FreeVideosController"
            })
            .when("/packages", {
                templateUrl: "/userapp/views/packages.html",
                controller: "PackagesController"
            })
            .when("/forgot-password", {
                templateUrl: "/userapp/views/forgot-password.html",
                controller: "ForgotPasswordController"
            })
        .otherwise({ redirectTo: "/dashboard" })
    });

    app.run(function ($rootScope, $location, $http, $q, CurrentUserService, PoolingService) {

        //PoolingService.StartPooling();

        var checkUserLogin = function () {
            $http.get("/api/user/isUserLoggedIn?id=" + CurrentUserService.profile.guid).then(function (response) {
                if (!response.data) {
                    CurrentUserService.logout();
                    window.location.reload(true);
                };
            }, function (err) {
                console.log(err);
            })
        }
        checkUserLogin();

        $rootScope.location = $location;

        $rootScope.USER_NAME = CurrentUserService.profile.username;

        $rootScope.FULL_NAME = CurrentUserService.profile.fullName;

        $rootScope.isFreeUser = CurrentUserService.profile.isFreeUser;
        $rootScope.showDownloadOption = CurrentUserService.profile.showDownloadOption;;

        $rootScope.VIDEOS = [];

        $rootScope.UPDATE_VIDEOS = function (typed) {
            console.log(typed);
            var defer = $q.defer();

            $http.get("/api/videos/searchVideos/" + typed.query)
            .success(function (response) {
                defer.resolve(response);
            });

            return defer.promise;
        }

        var isImageGet = false;

        $rootScope.LOG_OUT = function () {
            CurrentUserService.logout();
            window.location.reload();
        }
        $rootScope.SEARCH_TERM = "";
        $rootScope.facebookProfilePic = CurrentUserService.profile.profilePic;
        $rootScope.USER_IMAGE;

        $rootScope.updateUserImage = function () {
            console.log("Getting user image")
            $http.get("/api/user/getUserProfilePicture")
            .success(function (response) {
                $rootScope.USER_IMAGE = response;
            });
        }

        $rootScope.updateUserImage();

        $rootScope.SEARC_VIDEO = function () {
            console.log($rootScope.SEARCH_TERM);
            if ($rootScope.SEARCH_TERM && $rootScope.SEARCH_TERM.length > 2) {
                $location.path("/newvideos/" + $rootScope.SEARCH_TERM);
            }
            else {
                alert("Input no valid, Min. length is 3 characters");
            }
        }
    });

}());