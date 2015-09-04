
(function (app) {

    var passwordFieldValidator = function () {
        return {
            restrict: 'A',
            require: 'ngModel',
            link: function (scope, elm, attrs, ctrl) {
                var validateFn = function (viewValue) {
                    var re = /^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).*$/;
                    if (ctrl.$isEmpty(viewValue) || re.test(viewValue) == false) {
                        ctrl.$setValidity('passwordFieldValidator', false);
                        return undefined;
                    } else {
                        ctrl.$setValidity('passwordFieldValidator', true);
                        return viewValue;
                    }
                };

                ctrl.$parsers.push(validateFn);
                ctrl.$formatters.push(validateFn);
            }
        }
    };


    app.directive("passwordFieldValidator", passwordFieldValidator);

}(angular.module("DrNajeebAdmin")));