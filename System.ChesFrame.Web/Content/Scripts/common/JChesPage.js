




var jChes = window.jChes || {};

var jChesPage = jChes.jChesPage = function () { };

jChesPage.prototype.defaults = {

    //基本信息
    pageSize: [10, [10, 30, 50]],
    maxShowPageCount:10,//最多显示页数
    pagePlace: "bottom",//top,bottom,both

    //请求信息
    url: "",
    method: "GET",
    data: null,
    dataType: "json",

    queryKey: "queryKey",

    //事件
    //Ajax调用之前 执行 0
    onQuery: function () { },

    //Ajax调用之后返回结构时 执行
    onSuccess: function (data) { },

    //行绑定时执行
    onRowBind: function (row) { },

    onRowBinded: function (row, data) { },

    QueryData: function () { return null; }
};

jChesPage.prototype.options = {};
jChesPage.prototype.grid = {};
jChesPage.prototype.template = {};//模板
jChesPage.prototype.renderHtml = {};//每次数据绑定后零时存储
jChesPage.prototype.renderHtml.renderHead = {};//head
jChesPage.prototype.renderHtml.renderBody = [];//body  --所有body行


jChesPage.prototype.pageInfo = { index: 0, size: 0, count: 0, sortby: "", sortName: "", scend: "" };//分页信息  scend:asc desc


jChesPage.prototype.onSuccessCallBack = function (data) {

    //返回异常信息处理
    if (data.ErrorMsg != undefined) {
        alert(data.ErrorMsg);
        return;
    }


    var self = this;
    var pageInfo = data.PageInfo;

    self.pageInfo.count = data.PageInfo.Count;

    var mainData = data.Data;

    
    //ajax调用成功后响应
    self.options.onSuccess(mainData);

    //遍历行数据并处理赋值、绑定等信息
    $.each(mainData, function (i, n) {

        self.options.onRowBind(n);
        var headRowTemplateClone = self.template.bodyRowTemplate.clone(true);
        self._bindRow(headRowTemplateClone, n);
        self.renderHtml.renderBody.push(headRowTemplateClone);
        self.options.onRowBinded(headRowTemplateClone, n);
    });

    this._render();
};

jChesPage.prototype._bindRow = function (rowTemplet, row) {

    //属性方式
    var dataBindsInRow = rowTemplet.find("[data-bind]");//有data-bind属性的所有元素
    $.each(dataBindsInRow, function (i, n) {

        var databindInfo = $(n).attr("data-bind");
        var databindInfo2 = databindInfo.split(":");
        var targetAttr = databindInfo2[0];
        var targetValueKey = databindInfo2[1];

        $(n).attr(targetAttr, row[targetValueKey]);
    });
    //直接赋值方式（替换）
    var regExp = /{data-bind:\w+}/g;
    var databinds = regExp.exec($(rowTemplet).html())
    //if (databinds != null){
    //    $.each(databinds, function (j, m) {
    //        window.alert(databinds.length);
    //        var targetKey = m.substring(m.indexOf("{data-bind:") + "{data-bind:".length, m.lastIndexOf("}"));
    //        //$(rowTemplet).replace(m, row[targetKey]);
    //        $(rowTemplet).html($(rowTemplet).html().replace(m, row[targetKey]));
    //    });
    //}

    //var result;
    //var html =$(rowTemplet).html();
    //while ((result = regExp.exec(html)) != null) {
    //     //var targetKey = result.substring(result.indexOf("{data-bind:") + "{data-bind:".length, result.lastIndexOf("}"));
    //    // html.replace(m, row[targetKey]);
    //    var s = result;
    //}
    //$(rowTemplet).html(html);

    var html = $(rowTemplet).html();
    var databinds = html.match(regExp);

    if (databinds != null) {
        var html = $(rowTemplet).html();
        $.each(databinds, function (j, m) {
            var targetKey = m.substring(m.indexOf("{data-bind:") + "{data-bind:".length, m.lastIndexOf("}"));
            html = html.replace(m, row[targetKey]);
        });
        $(rowTemplet).html(html);
    }
};

jChesPage.prototype._render = function () {
   
    //body
    var self = this;
    self.grid.find("tbody").empty();
    //self.grid.find("thead").find("tr[class='pageInfo']").remove();

    var count = self.template.headRowTemplate.find("td").length;

    $.each(self.renderHtml.renderBody, function (i, n) {
        self.grid.find("tbody").append(n);
    });

    //if (self.renderHtml.renderBody.length < self.pageInfo.size) {
    //    for (var j = 0; j < self.pageInfo.size - self.renderHtml.renderBody.length; j++) {
    //        var tmpTr = $("<tr>").addClass("grid-body-row");
    //        for (var i = 0; i < count; i++) {
    //            $("<td>").text(" ").appendTo(tmpTr);
    //            //tmpTr.append("<td></td>");
    //        }
    //        self.grid.find("tbody").append(tmpTr);
    //    }
    //}

    var pageRowCount = self.pageInfo.count;
    var pageCurentIndex = parseInt(self.pageInfo.index);
    var pageSize = self.pageInfo.size;
    var pageShowNumber = [];    //需要显示的页Index
    var pageCount =Math.ceil( pageRowCount / pageSize);//总的页数
    //if (pageRowCount % pageSize > 0) { pageCount++; }
    //需要显示的页数计算（这里就把所有的都显示处理出来）
    var pageAllowCount = self.options.maxShowPageCount;//能够显示的页个数
    

    pageShowNumber.push(pageCurentIndex);//当前页

    var leftRef = rightRef = pageCurentIndex;
    var k = 0;

    while (k < pageAllowCount-1) {
        leftRef--;
        rightRef++;
        if (leftRef >= 0) {
            pageShowNumber.push(leftRef);
            k++;
        }
        if (k >= pageAllowCount - 1)
        { break;}
        if (rightRef <= pageCount-1) {
            pageShowNumber.push(rightRef);
            k++;
        }
        if (leftRef < 0 && rightRef > pageCount-1) {
            break;
        }
    }

    pageShowNumber = pageShowNumber.sort(function (m, n) { return m - n;  });//return m > n ? 1 : (m < n ? -1 : 0);

    var pageFirstNumber = 0;
    var pagePreNumber = parseInt(pageCurentIndex) - 1 > 0 ? parseInt(pageCurentIndex) - 1 : 0;
    var pageNextNumber = parseInt(pageCurentIndex) + 1 >= pageCount - 1 ? pageCount - 1 : parseInt(pageCurentIndex) + 1;
    var pageLastNumber = pageCount - 1;

    //pageInfo
    //thead td个数
    var count = self.template.headRowTemplate.find("td").length;

    self.grid.find(".grid-pagination").remove();
    var pageInfoTr = $("<tr class='grid-pagination'>");
    var pageInfoTd = $("<td>").attr("colspan", count).appendTo(pageInfoTr);

    $("<a>").attr("page-index", pageFirstNumber).attr("href", "#")
        .addClass("page-first")
        .text("首页").click(self, self._pageClickEvent)
        .appendTo(pageInfoTd);
    $("<a>").attr("page-index", pagePreNumber).attr("href", "#")
        .addClass("page-pre")
        .text("前一页").click(self, self._pageClickEvent)
        .appendTo(pageInfoTd);

    $.each(pageShowNumber, function (i, n) {
        var pageNumber = $("<a>").attr("page-index", n).attr("href", "javascript:void(0)").text(n + 1).appendTo(pageInfoTd);
        if (n != pageCurentIndex) {
            pageNumber.click(self, self._pageClickEvent)
        } else {
            pageNumber.addClass("page-current");
        }
    });

    $("<a>").attr("page-index", pageNextNumber).attr("href", "#")
        .addClass("page-next")
        .text("下一页").click(self, self._pageClickEvent)
        .appendTo(pageInfoTd);
    $("<a>").attr("page-index", pageLastNumber).attr("href", "#")
        .addClass("page-last")
        .text("末页").click(self, self._pageClickEvent)
        .appendTo(pageInfoTd);
    pageInfoTd.find("a").css("margin-left", "2px");

    if (self.options.pagePlace == "top" || self.options.pagePlace == "both") {
        self.grid.children(".grid-head").prepend(pageInfoTr);
    }
    
    if (self.options.pagePlace == "bottom" || self.options.pagePlace == "both") {
        var tfoot = self.grid.find("tfoot");
        if (tfoot.length <= 0) {
            $("<tfoot>").insertAfter(self.grid.find("tbody"));
        }
        tfoot = self.grid.find("tfoot");
        tfoot.prepend(pageInfoTr);
    }

    //head
    self.template.headRowTemplate.find("[data-sortby]").css("cursor", "pointer")
        .unbind().click(this, self._pageTitleSortEvent);

   
}

jChesPage.prototype.Init = function (gridId, options) {
    var grid = this.grid = $("#" + gridId);
    var headRowTemplate = grid.children(".grid-head").children(".grid-title");
    var bodyRowTemplate = grid.children(".grid-body").children(".grid-body-row");
    //var paginationTemplate = grid.find(".grid-pagination");

    this.template.headRowTemplate = headRowTemplate;
    this.template.bodyRowTemplate = bodyRowTemplate;
    //this.template.paginationTemplate = paginationTemplate.clone(true);

    grid.children(".grid-body").empty();
    //paginationTemplate.hide();

    //默认值
    this.options = $.extend({}, this.defaults, options);

    this.pageInfo.size = this.options.pageSize[0];

    this._render();
}

jChesPage.prototype.Query = function () {
    //pageinfo index 归0
    this.pageInfo.index = 0;
    this._Query();
}

jChesPage.prototype._Query = function () {
    //清除数据
    this.renderHtml.renderBody = [];
    this._AjaxQuery();
}

jChesPage.prototype._makeQueryData = function () {
    //构造data:
    var data = this.options.QueryData();
    data = data || {};
    data.pageInfo = {
        Index: this.pageInfo.index,
        Size: this.pageInfo.size,
        SortBy: this.pageInfo.sortName + " " + this.pageInfo.scend
    };

    var key = this.options.queryKey;
    var tmp = { key: JSON.stringify(data) };

    var key = this.options.queryKey;
    var tmp = "{\"" +key+"\":null }";
    tmp = eval("(" + tmp + ")");
    tmp[key] = JSON.stringify(data);
    return tmp;
}

jChesPage.prototype._AjaxQuery = function () {
    this.options.onQuery();

    var param = {
        url: this.options.url,
        data: this._makeQueryData(),
        success: this.onSuccessCallBack,
        type: this.options.method,
        dataType: this.options.dataType,
        context: this                     //这个对象用于设置Ajax相关回调函数的上下文，如果不设定这个参数，那么this就指向调用本次AJAX请求时传递的options参数
    };
    $.ajax(param);
}

jChesPage.prototype._pageClickEvent = function (event) {
    //$(this).hide();
    var jpage = event.data;
    var pageindex = $(this).attr("page-index");
    jpage.pageInfo.index = pageindex;

    ////触发请求
    jpage._Query();
}

jChesPage.prototype._pageTitleSortEvent = function (event) {
    var jpage = event.data;
    var sortName = $(this).attr("data-sortby");

    var ascKey = "asc";
    var descKey = "desc";

    if (jpage.pageInfo.sortName == sortName) {
        if (jpage.pageInfo.scend == ascKey) {
            jpage.pageInfo.scend = descKey;
        }
        else {
            jpage.pageInfo.scend = ascKey;
        }
    } else {
        jpage.pageInfo.sortName = sortName;
        jpage.pageInfo.scend = ascKey;
    }
    //触发请求
    jpage._Query();
}

jChesPage.prototype._pageSizeSettingChange = function (event) {
    var jpage = event.data;
    var pageSize = $(this).val();
    
    jpage.pageInfo.size = pageSize;
    //修改后，需要重新计算当前页，所以需要重置pageinfo.index
    jpage.pageInfo.index = 0;

    ////触发请求
    jpage._Query();
}

