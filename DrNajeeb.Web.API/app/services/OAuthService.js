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
                            CurrentUserService.setProfile(username, response.data.access_token);
                            return username;
                        });

        }

        return OAuthService;

    }

    OAuthService.$inject = ["$http", "FormEncodeService", "CurrentUserService"];
    app.factory("OAuthService", OAuthService);

}(angular.module("DrNajeebAdmin")));