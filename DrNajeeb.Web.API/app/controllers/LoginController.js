(function (app) {

    var LoginController = function ($scope, OAuthService, CurrentUserService, loginRedirect, toastr) {

        $scope.username = "";
        $scope.password = "";
        $scope.user = CurrentUserService.profile;
        $scope.form = null;

        $scope.login = function (form) {
            if (form.$valid) {
                OAuthService.login($scope.username, $scope.password)
                    .then(onLogin, onError);
                $scope.form = form;
            };
        }


        var onLogin = function (data) {
            toastr.success("Welcome To DrNajeebLectures");
            loginRedirect.redirectPostLogin();
            $scope.password = "";
            $scope.form.$setPristine(true)
        }

        var onError = function (response) {
            toastr.error(response.data.error_description);
        }
    }


    LoginController.$inject = ["$scope", "OAuthService", "CurrentUserService","loginRedirect", "toastr"];
    app.controller("LoginController", LoginController);

}(angular.module("DrNajeebAdmin")));