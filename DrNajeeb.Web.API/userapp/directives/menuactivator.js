(function (app) {

    var makeWatcher = function (location) {
        return function () {
            return location.url();
        };
    };

    var makeLinkUpdater = function (links, className) {
        return function (value) {
            angular.forEach(links, function (link) {
                link = angular.element(link);
                if (/\#(\/[^\/]+)/.exec(link.attr("href"))[1] == value) {
                    link.parent().addClass(className);
                    setTimeout(window.scrollTo(0, 0), 100);
                    //window.scrollTo(0, 0);
                } else {
                    link.parent().removeClass(className);
                }
            });
        };
    };

    var activeMenu = function ($location) {

        var link = function (scope, element, attrs) {
            var links = element.find("a");
            var className = attrs.activeMenu;
            scope.$watch(makeWatcher($location),
                         makeLinkUpdater(links, className));
        };

        return {
            link: link
        };
    };
    activeMenu.$injector = ["$location"];
    app.directive("activeMenu", activeMenu);

    //app.filter("nl2br", function ($filter, $sce) {
    //    return function (data) {
    //        if (!data) return data;
    //        var htmlCode = data.replace(/\n\r?/g, '<br />');
    //        return $sce.trustAsHtml(htmlCode);
    //    };
    //});

}(angular.module("DrNajeebUser")));

//return myApp.directive('mainMenu', function() {
//    return {
//        restrict: 'E',
//        templateUrl: 'partials/directives/main-menu.html',
//        replace: true,
//        controller: function($rootScope, $scope, $location, $routeParams) {
//            $scope.items = _.map(myApp.pages, function(page, i) {
//                return {
//                    name: page,
//                    path: '#/' + page,
//                    active: !i // Activate the first on default 
//                };
//            });
//            $rootScope.$on('$routeChangeSuccess', function() {
//                _($scope.items).each(function(item) {
//                    item.active = (item.path == '#' + $location.$$path);
//                });
//            });
//        }
//    }
//});