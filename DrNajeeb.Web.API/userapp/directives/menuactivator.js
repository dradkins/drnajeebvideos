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
                //if (/\#!(\/[^\/]+)/.exec(link.attr("href"))[1] == value) {
                //    link.parent().addClass(className);
                //    setTimeout(window.scrollTo(0, 0), 100);
                //    //window.scrollTo(0, 0);
                //}
                if (link.attr("href") == '#!' + value.substring(1)) {
                    console.log(true);
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

}(angular.module("DrNajeebUser")));