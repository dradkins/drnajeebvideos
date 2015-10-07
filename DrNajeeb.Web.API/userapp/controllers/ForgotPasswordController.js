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
            toastr.success("New password emailed to you successfully, Please login with new password")
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