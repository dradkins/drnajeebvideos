(function (app) {

    var searchBar = function () {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                element.bind('click', function () {
                    element.addClass("active");
                });
            }
        };
    };
    app.directive("searchBar", searchBar);

}(angular.module("DrNajeebUser")));