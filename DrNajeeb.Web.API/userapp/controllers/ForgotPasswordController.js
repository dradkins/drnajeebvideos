(function (module) {

    var ForgotPasswordController = function ($scope, $location, toastr, OAuthService) {

        $scope.email = "";

        $scope.resetPassword = function (formId) {
            if (formId.$valid) {
                OAuthService.resetPassword({ email: $scope.email })
                     .then(onPasswordReset, onPasswordResetError);
            }
            else {
                toastr.error("invalid data entered. please enter valid data and try again.");
            }
        }

        var onPasswordReset = function (data) {
            toastr.success("If you are a registered user you will receive a password reset email soon.")
            $location.path("/login");
        }

        var onPasswordResetError = function (error) {
            console.log(error);
            toastr.error("unable to reset password at this time, please try later");
        }

    };

    ForgotPasswordController.$inject = ["$scope","$location", "toastr", "OAuthService"];
    module.controller("ForgotPasswordController", ForgotPasswordController);

}(angular.module("DrNajeebUser")));