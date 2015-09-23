(function (app) {

    //for adding token to each request
    var addToken = function ($q, CurrentUserService) {

        var request = function (config) {
            if (CurrentUserService.profile.isLoggedIn) {
                config.headers.Authorization = "Bearer " + CurrentUserService.profile.token;
            }
            return $q.when(config);
        }

        return {
            request: request,
        }

    };

    addToken.$inject = ["$q", "CurrentUserService"];
    app.factory("addToken", addToken);

    //for redirecting user to login page
    var loginRedirect = function ($q, $location) {

        var lastPath="/dashboard"

        var responseError = function (response) {
            if (response.status == 401) {
                console.log($location.path());
                if ($location.path() == '/register' || $location.path() == '/free-register' || $location.path() == '/video-library') {
                    return $q.reject(response);
                }
                //console.log($location.path());
                //lastPath = $location.path();
                $location.path("/login");
            }
            return $q.reject(response);
        };

        var redirectPostLogin = function () {
            $location.path(lastPath);
        }

        return {
            responseError: responseError,
            redirectPostLogin: redirectPostLogin,
        };

    }

    loginRedirect.$inject = ["$q", "$location"];
    app.factory("loginRedirect", loginRedirect);

    //registering interceptors
    app.config(function ($httpProvider) {
        $httpProvider.interceptors.push("addToken");
        $httpProvider.interceptors.push("loginRedirect");
    });

}(angular.module("DrNajeebUser")));