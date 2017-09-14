﻿/// <reference path="E:\杨垒 含系统\Project\EicWebPlatform\EicWorkPlatfrom\Content/print/print.min.js" />
/// <reference path="../../common/angulee.js" />
/// <reference path="../../angular.min.js" />
/// <reference path="E:\杨垒 含系统\Project\EicWebPlatform\EicWorkPlatfrom\Content/pdfmaker/pdfmake.js" />

var productModule = angular.module('bpm.productApp');
productModule.factory('dReportDataOpService', function (ajaxService) {
    var urlPrefix = "/" + leeHelper.controllers.dailyReport + "/";
    var reportDataOp = {};
    //-------------------------标准工时设置-------------------------------------
    //获取产品工艺流程列表
    reportDataOp.getProductionFlowList = function (department, productName, orderId, searchMode) {
        var url = urlPrefix + 'GetProductionFlowList';
        return ajaxService.getData(url, {
            department: department,
            productName: productName,
            orderId: orderId,
            searchMode: searchMode,
        });
    };
    //保存产品工艺流程数据
    reportDataOp.storeProductFlowDatas = function (entities) {
        var url = urlPrefix + 'StoreProductFlowDatas';
        return ajaxService.postData(url, {
            entities: entities,
        });
    };
    ///得到标准生产工艺流程总览（New）
    reportDataOp.getProductionFlowOverview = function (department, productName, searchMode) {
        var url = urlPrefix + 'GetProductionFlowOverview';
        return ajaxService.getData(url, {
            department: department,
            productName: productName,
            searchMode: searchMode,
        });
    };


    ///////************************************************************************///

    ///载入ERP中   用于确认今天生产的订单

    reportDataOp.getInProductionOrderDatas = function (department) {
        var url = urlPrefix + 'GetInProductionOrderDatas';
        return ajaxService.getData(url, {
            department: department,
        });
    }
    ///载入ERP中   用于确认今天生产的订单
    reportDataOp.getOrderDispatchInfoDatas = function (department, nowDate) {
        var url = urlPrefix + 'GetOrderDispatchInfoDatas';
        return ajaxService.getData(url, {
            department: department,
            nowDate: nowDate,
        });
    }
    /// 保存分配数据
    reportDataOp.saveOrderDispatch = function (entity) {
        var url = urlPrefix + 'StoreOrderDispatchDatas';
        return ajaxService.postData(url, {
            entity: entity,
        });
    }
    ///**************************************
    ////日报录入 数据保存

    reportDataOp.saveDailyReportData = function (entity) {
        var url = urlPrefix + 'SaveDailyReportData';
        return ajaxService.postData(url, {
            entity: entity,
        });
    }
    ////******
    return reportDataOp;
});

//生产标准艺流程设定
productModule.controller("standardProductionFlowSetCtrl", function ($scope, dReportDataOpService, dataDicConfigTreeSet, connDataOpService, $modal) {
    ///工艺标准工时视图模型
    var uiVM = {
        Department: null,
        ProductId: null,
        ProductName: null,
        ProcessesIndex: 0,
        ProcessesSign: null,
        ProcessesName: null,
        ProcessesType: '人工',
        StandardProductionTimeType: 'UPS',
        StandardProductionTime: null,
        UPH: null,
        UPS: null,
        MachinePersonRatio: null,
        MouldId: null,
        MouldName: null,
        MouldHoleCount: 0,
        Remark: null,
        OpPerson: null,
        OpSign: leeDataHandler.dataOpMode.add,
        OpDate: null,
        OpTime: null,
        Id_Key: null,
        Id: null,
        IsServer: false,//是否是从服务器端获取的数据标志
    };
    $scope.vm = uiVM;
    //初始化视图
    var initVM = _.clone(uiVM);
    var vmManager = {
        init: function () {
            uiVM = _.clone(initVM);
            $scope.vm = uiVM;
        },
        isShowMachine: false,
        opSign: null,
        editWindowDisplay: false,
        editWindowWidth: '100%',
        copyWindowDisplay: false,
        department: leeLoginUser.department,
        productName: null,
        //编辑显示的数据集合
        editDatas: [],
        //编辑数据复制副本
        copyEditDatas: [],
        productNameFrom: null,
        productNameTo: null,
        delItem: null,
        ///部门
        departments: [
           { value: "MS1", label: "制一课" },
           { value: "MS2", label: "制二课" },
            { value: "MS3", label: "制三课" },
           { value: "MS5", label: "制五课" },
           { value: "MS6", label: "制六课" },
           { value: "MS7", label: "制七课" },
           { value: "MS10", label: "制十课" },
           { value: "PT1", label: "成型课" }],
        //工时类别
        standardProductionTimeTypes: [{ id: "UPH", text: "UPH" }, { id: "UPS", text: "UPS" }],
        //工艺类别
        processesTypes: [{ id: "人工", text: "人工" }, { id: "机台", text: "机台" }],
        editDatasSummyset: [],
        editDatasSummysetsource: [],
        //查看工艺流程明细 OK
        queryProductionFlowDetails: function (item) {
            vmManager.editDatas = [];
            vmManager.productName = item.ProductName;
            $scope.searchPromise = dReportDataOpService.getProductionFlowList(vmManager.department, vmManager.productName, "", 2).then(function (datas) {
                angular.forEach(datas, function (e) {
                    var dataItem = _.clone(e);
                    leeHelper.copyVm(e, dataItem);
                    dataItem.Id = leeHelper.newGuid();
                    dataItem.IsServer = true;
                    vmManager.editDatas.push(dataItem);
                });

                _.sortBy(vmManager.editDatas, 'ProcessesIndex');
            });
        },
        //获取项目最大配置工序ID
        getInspectionIndex: function () {
            if (vmManager.editDatas.length >= 0) {
                uiVM.ProcessesIndex = $scope.vm.ProcessesIndex = vmManager.editDatas.length + 1;
            }
            else {
                $scope.vm.ProcessesIndex = 0;
            }
        },

        ///工艺名称不能有重复 ---待写
        verifyProcessesName: function () {
            console.log(78975464);
        },
        //项次添加 
        addProductionFlow: function (item) {
            vmManager.queryProductionFlowDetails(item);
            vmManager.init();
            uiVM.ProcessesIndex = $scope.vm.ProcessesIndex = item.ProductFlowCount + 1;
            uiVM.ProductName = vmManager.productName;
            leeHelper.setObjectGuid(uiVM);
            vmManager.editWindowDisplay = true;
        },
        // 模糊查找
        vagueQueryProductionSummyDatas: function () {
            $scope.searchPromise = dReportDataOpService.getProductionFlowOverview(vmManager.department, vmManager.productName, 1).then(function (datas) {
                vmManager.editDatasSummyset = [];
                vmManager.editDatasSummyset = datas;
                vmManager.editDatasSummysetsource = datas;
            });
        },
        ///是否显示输入机台信息
        changeIsShowMachine: function () {
            if ($scope.vm.ProcessesType === "机台") {
                vmManager.isShowMachine = true;
            }
            else { vmManager.isShowMachine = false; }
        },
        ///changeDepartment
        changeDepartment: function () {
            $scope.promise = dReportDataOpService.getProductionFlowOverview(vmManager.department, vmManager.productName, 0).then(function (datas) {
                vmManager.editDatasSummyset = [];
                vmManager.editDatasSummyset = datas;
                vmManager.editDatasSummysetsource = datas;
            });
        },
    };
    $scope.vmManager = vmManager;
    var operate = Object.create(leeDataHandler.operateStatus);
    $scope.operate = operate;

    ////
    operate.productNameFrom = function () {
        vmManager.productNameFrom = vmManager.productName;
        vmManager.copyWindowDisplay = true;
    };
    //确认复制
    operate.copyConfirm = function () {
        vmManager.copyEditDatas = [];
        console.log(vmManager.editDatas);
        if (vmManager.editDatas.length <= 0) {
            leePopups.alert("复制数据不能为空");
            return;
        }
        angular.forEach(vmManager.editDatas, function (item) {
            var confirmData = _.clone(item);
            confirmData.Id = leeHelper.newGuid();
            confirmData.IsServer = false;//由客户端创建的数据
            confirmData.ProductName = vmManager.productNameTo;
            confirmData.opSign = leeDataHandler.dataOpMode.add;
            vmManager.copyEditDatas.push(confirmData);
        });
    };
    ///批量复制
    operate.copyok = function () {
        vmManager.productNameFrom = vmManager.productName;
        vmManager.copyWindowDisplay = true;
        vmManager.editWindowDisplay = false;
    };

    ///添加新一项
    operate.add = function () {
        vmManager.init();
        uiVM.ProductName = vmManager.productName;
        vmManager.getInspectionIndex();
        leeHelper.setObjectGuid(uiVM);
        vmManager.editWindowDisplay = true;
    };
    ///编辑
    operate.editItem = function (item) {
        vmManager.opSign = leeDataHandler.dataOpMode.edit;
        uiVM = item;
        uiVM.opSign = leeDataHandler.dataOpMode.edit;
        $scope.vm = uiVM;
        vmManager.editWindowDisplay = true;
    };
    ///删除 
    operate.deleteItem = function (item) {
        console.log(item);
        leePopups.confirm("删除提示", "您确定要删除该项数据吗？", function () {
            $scope.$apply(function () {
                if (item.IsServer) {
                    item.OpSign = leeDataHandler.dataOpMode.delete;
                }
                else {
                    leeHelper.delWithId(vmManager.editDatas, item);//从数据操作库中移除
                }
                leeHelper.delWithId(vmManager.editDatas, item);//移除临时数据
            });
        });
    },
    operate.copyDeleteItem = function (item) {
        console.log(item);
        leePopups.confirm("删除提示", "您确定要删除复制的数据吗？", function () {
            $scope.$apply(function () {
                if (item.IsServer) {
                    item.OpSign = leeDataHandler.dataOpMode.delete;
                }
                leeHelper.delWithId(vmManager.copyEditDatas, item);//移除临时数据
            });
        });
        vmManager.copyWindowDisplay = true;
        vmManager.editWindowDisplay = false;
    },
    //确认数据(显示保存)
    operate.confirmSave = function (isValid) {
        leeHelper.setUserData(uiVM);
        //部门信息不用变化
        $scope.vm.Department = vmManager.department;
        ///查询数据是否有相同的工艺名称
        if (!issave) return;
        if (uiVM.OpSign === leeDataHandler.dataOpMode.add) {
            var issave = true;
            angular.forEach(vmManager.editDatas, function (i) {
                if (i.ProcessesName == uiVM.ProcessesName) {
                    console.log(i.ProcessesName);
                    leePopups.alert("已经添加过了！【" + i.ProcessesName + "】");
                    issave = false;
                    return;
                };
            });
            var confirmData = _.clone(uiVM);
            confirmData.Id = leeHelper.newGuid();
            confirmData.IsServer = false;//由客户端创建的数据
            vmManager.editDatas.push(confirmData);
            vmManager.editWindowDisplay = false;
            vmManager.init();
        }
    };

    //批量保存数据
    operate.saveAll = function () {
        $scope.searchPromise = dReportDataOpService.storeProductFlowDatas(vmManager.editDatas).then(function (opresult) {
            if (opresult.Result) {
                leeDataHandler.dataOperate.handleSuccessResult(operate, opresult);
                vmManager.editDatas = [];
            }
        });
    };
    //批量复制保存数据
    operate.copySaveAll = function () {
        $scope.searchPromise = dReportDataOpService.storeProductFlowDatas(vmManager.copyEditDatas).then(function (opresult) {
            if (opresult.Result) {
                leeDataHandler.dataOperate.handleSuccessResult(operate, opresult);
                vmManager.copyEditDatas = [];
            }
        });
    };
    //复制取消
    operate.copyCancel = function () {
        vmManager.copyEditDatas = [];
        vmManager.copyWindowDisplay = false;
        vmManager.editWindowDisplay = true;
        vmManager.productNameTo = null;
    };
    //取消编辑
    operate.refresh = function () {
        leeDataHandler.dataOperate.refresh(operate, function () {
            vmManager.init();
            vmManager.editWindowDisplay = false;
        });
    };

    ///选择文件并导入数据
    $scope.selectFile = function (el) {
        var files = el.files;
        if (files.length > 0) {
            var file = files[0];
            var fd = new FormData();
            fd.append('file', file);
            dReportDataOpService.importProductFlowTemplateFile(fd).then(function (datas) {
                vmManager.editDatas = datas;
            });
        }
    };


    ////组织架构
    //$scope.promise = connDataOpService.getConfigDicData('Organization').then(function (tdatas) {
    //    departmentTreeSet.setTreeDataset(tdatas);
    //    var user = leeLoginUser;
    //});
    $scope.promise = dReportDataOpService.getProductionFlowOverview(vmManager.department, vmManager.productName, 0).then(function (datas) {
        vmManager.editDatasSummyset = [];
        vmManager.editDatasSummyset = datas;
        ///根据登录用户 载入信息 ，如果没有侧 选择载入
        if (datas.length > 0)
            vmManager.departments = [{ value: leeLoginUser.department, label: leeLoginUser.departmentText }];
    });
    $(function () {
        $("[data-toggle='popover']").popover();
    });
    //$scope.ztree = departmentTreeSet;
});
//日报录入
productModule.controller("DailyProductionReportCtrl", function ($scope, dataDicConfigTreeSet, connDataOpService, dReportDataOpService, $modal) {
    ///日报录入视图模型
    var uiVM = {
        Department: leeLoginUser.department,
        ClassType: '白班',
        InPutDate: new Date(),
        OrderId: null,
        ProductId: null,
        ProductName: null,
        ProductSpec: null,
        OrderQuantity: null,
        ProcessesIndex: 0,
        ProcessesName: null,
        ProcessesType: null,
        StandardProductionTime: null,
        MachinePersonRatio: null,
        MachineId: null,
        MouldId: null,
        MouldHoleCount: 0,
        MachineProductionTime: null,
        MachineUnproductiveTime: null,
        MachineUnproductiveReason: null,
        MasterWorkerId: null,
        MasterName: null,
        WorkerId: null,
        WorkerName: null,
        TodayProductionCount: null,
        TodayBadProductCount: null,
        WorkerProductionTime: null,
        WorkerNoProductionTime: null,
        WorkerNoProductionReason: null,
        Field1: null,
        Field2: null,
        Field3: null,
        Field4: null,
        Field5: null,
        OpPerson: null,
        OpSign: leeDataHandler.dataOpMode.add,
        OpDate: null,
        OpTime: null,
        Id_Key: null,
    }
    $scope.vm = uiVM;
    //初始化视图

    var initVM = _.clone(uiVM);
    var vmManager = {
        ///部门 
        department: leeLoginUser.department,
        putInDisplay: false,
        productionFlowShow: true,
        init: function () {
            uiVM = _.clone(initVM);
            $scope.vm = uiVM;
        },
        ///选择部门
        departments: [
           { value: "MS1", label: "制一课" },
           { value: "MS2", label: "制二课" },
            { value: "MS3", label: "制三课" },
           { value: "MS5", label: "制五课" },
           { value: "MS6", label: "制六课" },
           { value: "MS7", label: "制七课" },
           { value: "MS10", label: "制十课" },
           { value: "PT1", label: "成型课" }],
        searchedWorkers: [],
        processesInfos: [],
        isSingle: true,//是否搜寻到的是单个人
        getWorkerInfo: function () {
            if (uiVM.WorkerId === undefined) return;
            var strLen = leeHelper.checkIsChineseValue(uiVM.WorkerId) ? 2 : 6;
            if (uiVM.WorkerId.length >= strLen) {
                vmManager.searchedWorkers = [];
                $scope.searchedWorkersPrommise = connDataOpService.getWorkersBy(uiVM.WorkerId).then(function (datas) {
                    if (datas.length > 0) {
                        vmManager.searchedWorkers = datas;
                        if (vmManager.searchedWorkers.length === 1) {
                            vmManager.isSingle = true;
                            vmManager.selectWorker(vmManager.searchedWorkers[0]);
                            focusSetter.processesIndexFocus = true;
                        }
                        else {
                            vmManager.isSingle = false;
                        }
                    }
                    else {
                        vmManager.selectWorker(null);
                    }
                });
            }
        },
        selectWorker: function (worker) {
            if (worker !== null) {
                uiVM.WorkerName = worker.Name;
                uiVM.WorkerId = worker.WorkerId;
                uiVM.Department = worker.Department;
            }
            else {
                uiVM.Department = null;
            }
        },
        havePutInData: [],
        isShowhavePutInData: false,
        erpOrderInfoDatas: [],//载入已分配的订单信息
        erpOrderInfoDatasSource: [],
        productionFlowDatas: [],//工序信息
        changeDepartment: function () {
            $scope.promise = dReportDataOpService.getInProductionOrderDatas(vmManager.department).then(function (erpDatas) {
                vmManager.erpOrderInfoDatas = [];
                erpOrderInfoDatasSource = vmManager.erpOrderInfoDatas = erpDatas;
                console.log(erpDatas);
                ///根据登录用户 载入信息 ，如果没有侧 选择载入
                if (erpDatas.length > 0)
                    vmManager.departments = [{ value: leeLoginUser.department, label: leeLoginUser.departmentText }];
            });
            //if (vmManager.putInDisplay === false) {
            //    vmManager.putInDisplay = true;
            //}
            //else vmManager.putInDisplay = false;
        },//部门变化载入分配的订单信息
        putInDatas: function (item) {
            uiVM.OrderId = item.OrderId;
            uiVM.ProductId = item.ProductId;
            uiVM.ProductName = item.ProductName;
            uiVM.ProductSpec = item.ProductSpec;
            uiVM.OrderQuantity = item.ProduceNumber - item.PutInStoreNumber;
            vmManager.putInDisplay = true;
            vmManager.getProductionFlowDatas(uiVM.ProductName);
        }, //选择录入的项次
        getProductionFlowDatas: function (productName) {
            $scope.searchPromise = dReportDataOpService.getProductionFlowList(vmManager.department, productName, null, 2).then(function (datas) {
                vmManager.productionFlowDatas = datas;
            });
        },// 得到工序信息
        findProcessesInfo: function ($event) {
            if ($event.keyCode === 13 || $event.keyCode === 40 || $event.keyCode === 9) {
                if (uiVM.ProcessesIndex === null || vmManager.productionFlowDatas.length == 0) return;
                var processesInfo = _.find(vmManager.productionFlowDatas, function (u) { return u.ProcessesIndex == uiVM.ProcessesIndex })
                if (!_.isUndefined(processesInfo)) {
                    //leePopups.alert("没有此工序号");
                    focusSetter.processesIndexFocus = true;
                    vmManager.selectProcesses(processesInfo);
                    focusSetter.workerProductionTimeFocus = true;
                    vmManager.isProcessesNameShow = true;
                }
            };
        },//在数据查找相应的信息
        selectProcesses: function (item) {
            if (item !== null) {
                console.log(item);
                uiVM.ProcessesIndex = item.ProcessesIndex;
                uiVM.ProcessesName = item.ProcessesName;
                uiVM.ProcessesType = item.ProcessesType;
                uiVM.StandardProductionTime = item.StandardProductionTime;
            }
            else {
                uiVM.ProcessesIndex = 0;
            }
        }, //选择工序
    };
    $scope.vmManager = vmManager;
    $scope.promise = vmManager.changeDepartment();


    var operate = Object.create(leeDataHandler.operateStatus);
    $scope.operate = operate;
    operate.saveAlldata = function (isValid) {
        leeHelper.setUserData(uiVM);
        console.log(uiVM);
        leeDataHandler.dataOperate.add(operate, isValid, function () {
            $scope.searchPromise = dReportDataOpService.saveDailyReportData(uiVM).then(function (opresult) {
                if (opresult.Result) {
                    leeDataHandler.dataOperate.handleSuccessResult(operate, opresult);
                    vmManager.havePutInData.push(opresult.entity);
                    vmManager.productionFlowShow = false;
                    vmManager.isShowhavePutInData = true;
                    vmManager.init();
                }
            });
        });

    };
    //焦点设置器
    var focusSetter = {
        workerIdFocus: false,

        orderIdFocus: false,


        standardProductionTimeFocus: false,
        machineUnproductiveTimeFocus: false,

        processesIndexFocus: false,
        workerProductionTimeFocus: false,
        workerNoProductionTimeFocus: false,
        todayProductionCountFocus: false,
        saveAlldataFocus: false,
        //移动焦点到指定对象
        moveFocusTo: function ($event, elPreName, elNextName) {
            if ($event.keyCode === 13 || $event.keyCode === 39 || $event.keyCode === 9) {
                focusSetter[elNextName] = true;
            }
            else if ($event.keyCode === 37) {
                focusSetter[elPreName] = true;
            };
        },
        doWhenKeyDown: function ($event, fn) {
            if ($event.keyCode === 13 || $event.keyCode === 39 || $event.keyCode === 9) { fn(); }
        },
        //回车事件
        changeEnter: function ($event, elPreName, elNextName) {
            focusSetter.moveFocusTo($event, elPreName, elNextName)
        }

    };
    $scope.focus = focusSetter;

});
/// 生产订单分派  
productModule.controller("DailyProductOrderDispatchCtrl", function ($scope, dataDicConfigTreeSet, connDataOpService, dReportDataOpService, $modal) {
    ///日报分派录入视图模型
    var uiVm = {
        OrderId: null,
        ProductionDepartment: null,
        ProductId: null,
        ProductName: null,
        ProductSpec: null,
        PutInStoreNumber: 0,
        ProduceNumber: 0,
        ProductStatus: null,
        ProductionDate: null,
        IsValid: true,
        ValidDate: null,
        OpPerson: null,
        OpSign: leeDataHandler.dataOpMode.add,
        OpDate: null,
        OpTime: null,
        Id_Key: null,
        Id: null,
    }
    $scope.vm = uiVm;
    var initVM = _.clone(uiVm);
    var dialog = $scope.dialog = leePopups.dialog();
    var vmManager = {
        ///部门 
        department: leeLoginUser.department,
        dispatchActiveTab: 'qryERPFormTab',
        departments: [
           { value: "MS1", label: "制一课" },
           { value: "MS2", label: "制二课" },
           { value: "MS3", label: "制三课" },
           { value: "MS5", label: "制五课" },
           { value: "MS6", label: "制六课" },
           { value: "MS7", label: "制七课" },
           { value: "MS10", label: "制十课" },
           { value: "PT1", label: "成型课" }],
        IsValids: [{ id: "true", text: "启用" }, { id: "false", text: "不启用" }],
        erpOrderInfoDatas: [],
        nowDate: new Date(),
        erpOrderInfoDatasSource: [],
        todayHaveDispatchDatas: [],
        changeDepartment: function () {
            $scope.promise = dReportDataOpService.getOrderDispatchInfoDatas(vmManager.department, vmManager.nowDate).then(function (datas) {
                vmManager.erpOrderInfoDatas = [];
                vmManager.erpOrderInfoDatasSource = [];
                vmManager.todayHaveDispatchDatas = [];
                vmManager.todayHaveDispatchDatas = datas.todayHaveDispatchProductionOrderDatas;
                angular.forEach(datas.erpInProductiondatas, function (item) {
                    var isHaveDispatch = false;
                    if (datas.todayHaveDispatchProductionOrderDatas.length > 0) {
                        _.find(datas.todayHaveDispatchProductionOrderDatas, function (i) {
                            if (i.OrderId === item.OrderId) {
                                isHaveDispatch = true;
                                item.ProductStatus = '生产中';
                            }
                        });
                    }
                    var dataItem = item;
                    dataItem.isDispatch = isHaveDispatch;
                    dataItem.Id = leeHelper.newGuid();
                    vmManager.erpOrderInfoDatas.push(dataItem);
                    vmManager.erpOrderInfoDatasSource.push(dataItem);
                });
                console.log(vmManager.erpOrderInfoDatas);
                ///根据登录用户 载入信息 ，如果没有侧 选择载入
                //if (datas.erpInProductiondatas > 0)
                //    vmManager.departments = [{ value: leeLoginUser.department, label: leeLoginUser.departmentText }];
            });
        },
        ///分配订单到数据表中(取消分配)
        dispatchOrder: function (item) {
            var findItem = _.findWhere(vmManager.todayHaveDispatchDatas, { OrderId: item.OrderId });
            if (_.isUndefined(findItem)) {
                leeHelper.copyVm(item, uiVm)
                item.isDispatch = uiVm.IsValid = true;
                uiVm.OpSign = leeDataHandler.dataOpMode.add;

            }
            else {
                leeHelper.copyVm(findItem, uiVm)
                item.isDispatch = uiVm.IsValid = false;
                uiVm.OpSign = leeDataHandler.dataOpMode.delete;
                console.log(findItem);
            }
            item.ProductStatus = uiVm.ProductStatus;
            dialog.show();
        },
        editHaveDispatchOrder: function (item) {
            leeHelper.copyVm(item, uiVm)
            uiVm.OpSign = leeDataHandler.dataOpMode.edit;
            dialog.show();
        },
        deleteHaveDispatchOrder: function (item) {
            leeHelper.copyVm(item, uiVm)
            item.isDispatch = uiVm.IsValid = false;
            uiVm.OpSign = leeDataHandler.dataOpMode.delete;
            dialog.show();
        },
    };
    $scope.vmManager = vmManager;
    $scope.promise = vmManager.changeDepartment();
    var operate = Object.create(leeDataHandler.operateStatus);
    $scope.operate = operate;
    operate.saveDispatchData = function (isValid) {
        leeHelper.setUserData(uiVm);
        if (uiVm.IsValid == true) uiVm.ProductStatus = "生产中";
        leeDataHandler.dataOperate.add(operate, isValid, function () {
            $scope.promise = dReportDataOpService.saveOrderDispatch(uiVm).then(function (opresult) {
                leeDataHandler.dataOperate.handleSuccessResult(operate, opresult, function () {
                    if (opresult.Result) {
                        var dataItem = _.clone(opresult.Entity);
                        var findItem = _.findWhere(vmManager.todayHaveDispatchDatas, { OrderId: dataItem.OrderId });
                        if (_.isUndefined(findItem)) {
                            vmManager.todayHaveDispatchDatas.push(dataItem);
                        }
                        else {
                            if (dataItem.opSign == leeDataHandler.dataOpMode.delete)
                                leeHelper.delWithId(vmManager.todayHaveDispatchDatas, uiVm)
                        };
                        //vmManager.dispatchActiveTab = "qryTodayHaveDispatchTab";
                        dialog.close();
                    }
                });
            });
        });
    }
    operate.Cancel = function () {
        dialog.close();
    }
});