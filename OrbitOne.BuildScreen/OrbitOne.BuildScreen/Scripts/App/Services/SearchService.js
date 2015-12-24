angular.module("BuildscreenApp.services").
    service("Search", function () {
        this.model = {}
        this.dateFilter = {};
        this.searchKey = "";
        this.builds = [];
        this.TeamProjectTags = [];
        this.BuildNameTags = [];

        var observers = [];

        this.getArrayOfTeamProjectNames = function (query) {
            var returnArray = [];
            for (var i = 0; i < this.builds.length; i++) {
                if (this.builds[i].TeamProject.toLowerCase().indexOf(query.toLowerCase()) !== -1) {
                    if (returnArray.indexOf(this.builds[i].TeamProject) === -1) {
                        returnArray.push(this.builds[i].TeamProject);
                    }
                }
            }
            return returnArray;
        };
        this.getArrayOfBuildNames = function (query) {
            var returnArray = [];
            for (var i = 0; i < this.builds.length; i++) {
                if (this.builds[i].Builddefinition.toLowerCase().indexOf(query.toLowerCase()) !== -1) {
                    if (returnArray.indexOf(this.builds[i].Builddefinition) === -1) {
                        returnArray.push(this.builds[i].Builddefinition);
                    }
                }
            }
            return returnArray;
        };
        //_____________________________________________________________________________________________________________
        //---------------initial notification-------------//
        //_____________________________________________________________________________________________________________
        this.initialize = function () {
            for (var i = 0; i < observers.length; i++) {
                observers[i].update(false);
            }
        }
        //_____________________________________________________________________________________________________________
        //---------------initialize methods-------------//
        //---------------Observer-------------//
        //_____________________________________________________________________________________________________________
        this.addObserver = function (observer) {
            observers.push(observer);
        };
        this.removeObserver = function (observer) {
            observers.remove(observer);
        };
        //_____________________________________________________________________________________________________________
        //---------------Set localstorage-------------//
        //_____________________________________________________________________________________________________________
        this.updateSimpleFilter = function () {
            localStorage.setItem("searchValue", this.searchKey);
            this.initialize();
        };
        this.updateTeamProjectTags = function (array) {
            this.TeamProjectTags = array;
            localStorage.setItem("TeamProjectTags", JSON.stringify(array));
            this.initialize();
        }
        this.updateBuildNameTags = function (array) {
            this.BuildNameTags = array;
            localStorage.setItem("BuildNameTags", JSON.stringify(array));
            this.initialize();
        }
        this.updateAdvancedFilterBool = function () {
            localStorage.setItem("checkValue", this.model.checkValue);
            if (this.builds.length !== 0) {
                this.initialize();
            }
        }
        this.updateDateFilter = function () {
            localStorage.setItem("dateFilter", JSON.stringify(this.dateFilter));
            this.initialize();
        }
        //_____________________________________________________________________________________________________________
        //---------------retrieve localstorage-------------//
        //_____________________________________________________________________________________________________________
        if (localStorage.getItem("BuildNameTags")) {
            this.updateBuildNameTags(this.TeamProjectTags = JSON.parse(localStorage.getItem("BuildNameTags")));
        };
        if (localStorage.getItem("TeamProjectTags")) {
            this.updateTeamProjectTags(this.TeamProjectTags = JSON.parse(localStorage.getItem("TeamProjectTags")));
        };
        if (localStorage.getItem("searchValue")) {
            this.searchKey = localStorage.getItem("searchValue");
        };
        if (localStorage.getItem("checkValue")) {
            this.model.checkValue = localStorage.getItem("checkValue") === "true" ? true : false;
        };
        if (localStorage.getItem("dateFilter")) {
            this.dateFilter = JSON.parse(localStorage.getItem("dateFilter"));
            //this.initialize();
        } else {
            this.dateFilter = {
                week: 0,
                month: 0
            }
        }

    });