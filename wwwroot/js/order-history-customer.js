//Order history

var sortOrderId = localStorage.getItem('sortOrderId') || "";
var sortName = localStorage.getItem('sortName') || "";
var sortDate = localStorage.getItem('sortDate') || "zxy";
var sortTotalItems = localStorage.getItem('sortTotalItems') || "";
var sortPrice = localStorage.getItem('sortPrice') || "";
var searchName = "";
var searchDateOrder = "";
var searchOrderId = "";
var pageIndexOrder = 1;
var pageSizeOrder = 8;

var optionOrders = {
    UserId: "",
    PageIndex: pageIndexOrder,
    PageSize: pageSizeOrder,
    SortOrderId: sortOrderId,
    SortName: sortName,
    SortTotalItems: sortTotalItems,
    SortPrice: sortPrice,
    SortDate: sortDate,
    SearchName: searchName,
    SearchDate: searchDateOrder,
    SearchOrderId: searchOrderId
}

const sortingOrder = ["", "abc", "zxy"];

function LoadOrderHistory(optionOrders) {
    $.ajax({
        type: 'post',
        url: '/account/orderhistory',
        data: optionOrders,

        success: function (response) {
            $("#main-content").empty;
            $("#main-content").html(response);
        },
        error: function (xhr, status, error) {
            console.log(error);
        }
    });
}

function getOrdersHistory() {
    LoadOrderHistory(optionOrders);
}