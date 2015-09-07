(function (app) {

    var menuHider = function () {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                element.bind('click', function () {
                    $('#page-wrapper').toggleClass('nav-small');
                });
            }
        };
    };
    app.directive("menuHider", menuHider);

}(angular.module("DrNajeebUser")));