﻿/// <reference path="../../common/angulee.js" />
/// <reference path="../../angular.min.js" />
angular.module('bpm.toolsOnlineApp', ['eicomm.directive', 'mp.configApp', 'ngAnimate', 'ui.router', 'ngMessages', 'cgBusy', 'ngSanitize', 'mgcrea.ngStrap', "pageslide-directive", 'angular-popups'])

.config(function ($stateProvider, $urlRouterProvider, $compileProvider) {
    $compileProvider.imgSrcSanitizationWhitelist(/^\s*(https?|local|data):/);

    var oAssistantUrlPrefix = leeHelper.controllers.TolOfficeAssistant + "/";

    var wfUrlPrefix = leeHelper.controllers.TolWorkFlow + "/";

    var hwUrlPrefix = leeHelper.controllers.TolCooperateWithHw + "/";

    var itunnelPrefix = leeHelper.controllers.TolManPowerTunnel + "/";

    //--------------名片夹管理--------------------------
    $stateProvider.state('collaborateContactLib', {
        templateUrl: oAssistantUrlPrefix + 'CollaborateContactLib'
    })
    ///工作任务管理
    $stateProvider.state('workTaskManage', {
        templateUrl: oAssistantUrlPrefix + 'WorkTaskManage'
    });
    ///上报改善问题
    $stateProvider.state('reportImproveProblem', {
        templateUrl: oAssistantUrlPrefix + 'ReportImproveProblem'
    });
    //--------------电子签核--------------------------
    ///内部联络单
    $stateProvider.state('wFInternalContactForm', {
        templateUrl: wfUrlPrefix + 'WFInternalContactForm'
    });
    //--------------华为协同--------------------
    $stateProvider.state('hwMaterialBaseConfig', {//物料配置与BOM信息
        templateUrl: hwUrlPrefix + 'HwMaterialBaseConfig'
    });
    $stateProvider.state('hwManpowerInput', {//人力管理
        templateUrl: hwUrlPrefix + 'HwManpowerInput'
    });
    $stateProvider.state('hwLogisticDeliveryInput', {//物流信息登记
        templateUrl: hwUrlPrefix + 'HwLogisticDeliveryInput'
    });
    $stateProvider.state('hwInventoryDetail', {//库存明细
        templateUrl: hwUrlPrefix + 'HwInventoryDetail'
    });
    //--------------智能隧道---------------------
    $stateProvider.state('tolManpowerDisk', {//人力磁盘
        templateUrl: itunnelPrefix + 'TolManpowerDisk'
    });
})


