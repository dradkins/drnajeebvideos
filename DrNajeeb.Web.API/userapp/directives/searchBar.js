(function (app) {

    var searchBar = function () {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                element.bind('click', function () {
                    element.addClass("active");
                    $('#search1').focus();
                });
            }
        };
    };
    app.directive("searchBar", searchBar);

}(angular.module("DrNajeebUser")));