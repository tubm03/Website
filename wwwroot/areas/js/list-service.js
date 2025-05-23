let pageIndex = 1;
let pageSize = 7;

let serviceFilter = {
    name: null,
    serviceType: null,
    status: null,
    sortServiceName: null,
    sortServiceId: null,
    sortPrice: null,
    sortUsedQuantity: null
}

function resetSortOption() {
    serviceFilter.sortServiceName = null;
    serviceFilter.sortServiceId = null;
    serviceFilter.sortPrice = null;
    serviceFilter.sortUsedQuantity = null;
}

function refreshServiceFilter() {
    serviceFilter.name = $("#service-name-search").val();
    serviceFilter.serviceType = $("#service-type-search").find(".choose").attr("data-service-type");
    serviceFilter.status = $("#service-status-search").find(".choose").attr("data-status");

    resetSortOption();
    serviceFilter.sortServiceId = 'Ascending';

    pageIndex = 1;
}

function searchServiceName(event) {
    event.preventDefault();
    refreshServiceFilter();
    fetchNewListService();
}

function searchServiceType(element) {
    $("#service-type-search").find("li a").removeClass("choose");
    $(element).addClass("choose");
    refreshServiceFilter();
    fetchNewListService();
}

function searchServiceStatus(element) {
    $("#service-status-search").find("li a").removeClass("choose");
    $(element).addClass("choose");
    refreshServiceFilter();
    fetchNewListService();
}

function selectServiceName() {
    let currentStateSort = serviceFilter.sortServiceName;
    resetSortOption();
    if (currentStateSort === null || currentStateSort === 'Descending') {
        serviceFilter.sortServiceName = 'Ascending';
    } else {
        serviceFilter.sortServiceName = 'Descending';
    }
    fetchNewListService();
}

function selectServiceId() {
    let currentStateSort = serviceFilter.sortServiceId;
    resetSortOption();
    if (currentStateSort === null || currentStateSort === 'Descending') {
        serviceFilter.sortServiceId = 'Ascending';
    } else {
        serviceFilter.sortServiceId = 'Descending';
    }
    fetchNewListService();
}

function selectPrice() {
    let currentStateSort = serviceFilter.sortPrice;
    resetSortOption();
    if (currentStateSort === null || currentStateSort === 'Descending') {
        serviceFilter.sortPrice = 'Ascending';
    } else {
        serviceFilter.sortPrice = 'Descending';
    }
    fetchNewListService();
}

function selectUsedQuantity() {
    let currentStateSort = serviceFilter.sortUsedQuantity;
    resetSortOption();
    if (currentStateSort === null || currentStateSort === 'Descending') {
        serviceFilter.sortUsedQuantity = 'Ascending';
    } else {
        serviceFilter.sortUsedQuantity = 'Descending';
    }
    fetchNewListService();
}

function selectPage(currentPage) {
    $("#paging-navigation").find("li").removeClass("active");
    $("#paging-navigation").find("#page-" + currentPage).addClass("active");

    pageIndex = currentPage;
    fetchNewListService();
}

function generatePagingNavigation(currentPage, numberPage) {
    if (numberPage > 0) {
        let content = '';
        if (currentPage > 1) {
            content += `<li id="page-${currentPage - 1}" class="page-item" > <a class="page-link" onclick="selectPage(${currentPage - 1})" href="javascript:void(0);">Trang trước</a></li> `;
        } else {
            content += `<li class="page-item"> <a class="page-link" href="javascript:void(0);" style="pointer-events:none; cursor:default; color:#b7b7b7;">Trang trước</a></li> `;
        }
        var startPage = Math.max(1, currentPage - 2);
        var endPage = Math.min(numberPage, currentPage + 2);
        if (startPage > 1) {
            content += ` <li id="page-1" class="page-item" > <a class="page-link" onclick="selectPage(1)" href="javascript:void(0);">1</a></li> `;
            if (startPage > 2) {
                content += ` <li class="page-item" > <a class="page-link" href="javascript:void(0);">...</a></li> `;
            }
        }
        for (let i = startPage; i <= endPage; i++) {
            if (currentPage === i) {
                content += ` <li id="page-${i}" class="page-item active" > <a class="page-link" href="javascript:void(0);">${i}</a></li> `;
            }
            else {
                content += ` <li id="page-${i}" class="page-item" > <a class="page-link" onclick="selectPage(${i})" href="javascript:void(0);">${i}</a></li> `;
            }
        }
        if (numberPage >= endPage + 1) {
            if (numberPage >= endPage + 2) {
                content += ` <li class="page-item" > <a class="page-link" href="javascript:void(0);">...</a></li> `;
            }
            content += ` <li id="page-${numberPage}" class="page-item" > <a class="page-link" onclick="selectPage(${numberPage})" href="javascript:void(0);">${numberPage}</a></li> `;
        }
        if (currentPage != numberPage) {
            content += ` <li id="page-${currentPage + 1}" class="page-item" > <a class="page-link" onclick="selectPage(${currentPage + 1})" href="javascript:void(0);">Trang sau</a></li> `;
        } else {
            content += `<li class="page-item" > <a class="page-link" href="javascript:void(0);" style="pointer-events:none; cursor:default; color:#b7b7b7;">Trang sau</a></li> `;
        }
        $('#paging-navigation').html(content);
    } else {
        $('#paging-navigation').empty();
    }
}
