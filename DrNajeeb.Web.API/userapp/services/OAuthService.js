(function (app) {

    var OAuthService = function ($http, FormEncodeService, CurrentUserService) {

        var OAuthService = {};

        OAuthService.login = function (username, password) {

            var config = {
                headers: {
                    "Content-Type": "application/x-www-form-urlencoded"
                }
            }

            var data = FormEncodeService({
                username: username,
                password: password,
                grant_type: "password",
            });

            return $http.post("/token", data, config)
                        .then(function (response) {
                            CurrentUserService.setProfile(username, response.data.access_token, response.data.fullName, null, false);
                            return response.data.fullName;
                        });

        }

        OAuthService.changePassword = function (changePasswordModel) {
            return $http.post("/api/account/changePassword", changePasswordModel)
                   .then(function (response) {
                       return response.data;
                   });
        }

        OAuthService.register = function (registerModel) {
            return $http.post("/api/account/register", registerModel)
                   .then(function (response) {
                       return response.data;
                   });
        }

        OAuthService.registerExternal = function (registerModel) {
            return $http.post("/api/account/registerExternalToken", registerModel)
                        .then(function (response) {
                            return response.data;
                        });
        }

        OAuthService.loginExternal = function (signinModel) {
            return $http.post("/api/account/externalSignin", signinModel)
                        .then(function (response) {
                            return response.data;
                        });
        }

        OAuthService.getGoogleUserInfo = function (token) {
            var url = "https://www.googleapis.com/plus/v1/people/me";
            var config = {
                headers: {
                    "Authorization": "Bearer " + token,
                }
            }
            $http.get(url).then(function (response, config) {
                console.log(response);
            })
        }


        //OAuthService.setFullName = function (fullNameModel) {
        //    return $http.post("/api/account/setFullName", fullNameModel)
        //                .then(function (response) {
        //                    CurrentUserService.profile.fullName
        //                });
        //}

        return OAuthService;

    }

    OAuthService.$inject = ["$http", "FormEncodeService", "CurrentUserService"];
    app.factory("OAuthService", OAuthService);

}(angular.module("DrNajeebUser")));