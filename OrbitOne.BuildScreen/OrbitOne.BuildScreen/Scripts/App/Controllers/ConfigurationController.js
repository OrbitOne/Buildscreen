angular.module("BuildScreenApp")
    .controller("ConfigurationController", ["$scope", "ngDialog", "Configuration",
        function ($scope, ngDialog, Configuration) {

            $scope.configurations = "";
            $scope.configToRemove = "";
            $scope.hideButtons = false;
            var confirmModal;

            $scope.configurations = Configuration.query();

            $scope.editConfiguration = function (config) {
                config.$update(function() {
                    $scope.configurations = Configuration.query();
                    $scope.toggleButtons();
                }, function(error) {
                    $scope.editErrorMessage = error.data.Message;
                    $scope.hasEditServerError = true;
                });
            }

            var encryptToAes = function (string)
            {
                var key = CryptoJS.enc.Utf8.parse('7z9gg5sp606f9x2v');
                var iv = CryptoJS.enc.Utf8.parse('58e96aaz3bv5s6e8');
                var encrypted = CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(string), key,
                    {
                        keySize: 128 / 8,
                        iv: iv,
                        mode: CryptoJS.mode.CBC,
                        padding: CryptoJS.pad.Pkcs7
                    });

                return encrypted.ciphertext.toString(CryptoJS.enc.Base64);
            }
            
            $scope.createConfiguration = function (type, form) {

                var newConfig = new Configuration();
                if (type === "TFS") {
                    newConfig.Username = $scope.newTfsConfig.Username;
                    newConfig.Password = encryptToAes($scope.newTfsConfig.Password);
                    newConfig.Uri = $scope.newTfsConfig.Uri;
                    newConfig.Name = $scope.newTfsConfig.Name;
                }
                if (type === "VSO") {
                    newConfig.Username = $scope.newVsoConfig.Username;
                    newConfig.Password = encryptToAes($scope.newVsoConfig.Password);
                    newConfig.Uri = $scope.newVsoConfig.Uri;
                    newConfig.Name = $scope.newVsoConfig.Name;
                }
                newConfig.ServiceType = type;
                newConfig.$save(function () {
                    $scope.configurations = Configuration.query();
                    if (type === "TFS") {
                        $scope.newTfsConfig = {};
                        $scope.showNewTfs = false;
                    }
                    if (type === "VSO") {
                        $scope.newVsoConfig = {};
                        $scope.showNewVso = false;
                    }
                    form.$setPristine();
                    
                }, function (error) {
                    $scope.errorMessage = error.data.Message;
                    $scope.hasServerError = true;
                });
            }

            $scope.clearErrorMessage = function() {
                $scope.errorMessage = "";
                $scope.hasServerError = false;
            }

            $scope.clearEditErrorMessage = function () {
                $scope.editErrorMessage = "";
                $scope.hasEditServerError = false;
            }

            $scope.onBlur = function (event) {
                var input = event.target;
                if (!!input.value) {
                    event.target.classList.add("filled");
                } else {
                    input.classList.remove("filled");
                }
                //event.target.classList.toggle("filled", !!event.target.value); ie suckt
            };

            $scope.makeConfigCopy = function (config) {
                $scope.configCopy = angular.copy(config);
            }

            $scope.cancelEditConfig = function (config) {
                $scope.configurations[$scope.configurations.indexOf(config)] = $scope.configCopy;
            }

            $scope.toggleButtons = function() {
                $scope.hideButtons = ! $scope.hideButtons;
            }


            $scope.openConfirmModal = function (config) {
                ngDialog.openConfirm({
                    closeByEscape: false,
                    closeByDocument: false,
                    template: "Scripts/App/Templates/ConfirmModal.html",
                    scope: $scope
                }).then(function () {
                    console.log(config);
                    config.$remove(function () {
                        $scope.configurations = Configuration.query();
                        $scope.configToRemove = "";
                    });
                });
            }

        }
    ]);






