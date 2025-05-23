function loadData(pageIndex, pageSize) {
    $.ajax({
        url: url,
        type: 'POST',
        data: {
            pageIndex: pageIndex,
            pageSize: pageSize
        },
        success: function (response) {
            handleLoadSuccess(response);
        },
        error: function (xhr, status, error) {
            console.error("Error:", xhr.responseText);
            // Handle error scenario gracefully
        }
    });
}
function handleLoadSuccess(response) {
    var accounts = response.accounts;
    objects = response.accounts;

    if (accounts.length > 0) {
        displayAccounts(accounts, response.userType, response.totalAccount);
        paging(response.currentPage, response.numberPage, response.pageSize);
    } else {
        displayNoData();
        console.error("Unexpected response structure:", response);
    }
}

function paging(currentPage, numberPage, pageSize) {
    if (numberPage > 0) {
        var str = '<nav aria-label="Page navigation example"> <ul class="pagination">';
        if (currentPage > 1) {
            str += `<li class="page-item"><a class="page-link" onclick="nextPage(${currentPage - 1},${pageSize})" href="javascript:void(0);">Trang trước</a></li>`;
        } else {
            str += `<li class="page-item"> <a class="page-link" href="javascript:void(0);" style="pointer-events:none; cursor:default; color:#b7b7b7;">Trang trước</a></li> `;
        }
        var startPage = Math.max(1, currentPage - 2);
        var endPage = Math.min(numberPage, currentPage + 2);
        if (startPage > 1) {
            str += ` <li class="page-item"><a class="page-link" onclick="nextPage(${1},${pageSize})" href="javascript:void(0);">1</a></li>`;
            if (startPage > 2) {
                str += ` <li class="page-item"><a class="page-link" href="javascript:void(0);">...</a></li>`;
            }
        }
        for (let i = startPage; i <= endPage; i++) {
            if (currentPage === i) {
                str += ` <li class="page-item active"><a class="page-link" href="javascript:void(0);">${i}</a></li>`;
            }
            else {
                str += ` <li class="page-item"><a class="page-link" onclick="nextPage(${i},${pageSize})" href="javascript:void(0);">${i}</a></li>`;
            }
        }
        if (numberPage >= endPage + 1) {
            if (numberPage >= endPage + 2) {
                str += ` <li class="page-item"><a class="page-link" href="javascript:void(0);">...</a></li>`;
            }
            str += ` <li class="page-item"><a class="page-link" onclick="nextPage(${numberPage},${pageSize})" href="javascript:void(0);">${numberPage}</a></li>`;
        }
        if (currentPage != numberPage) {
            str += ` <li class="page-item"><a class="page-link" onclick="nextPage(${currentPage + 1},${pageSize})" href="javascript:void(0);">Trang sau</a></li>`;
        } else {
            str += `<li class="page-item" > <a class="page-link" href="javascript:void(0);" style="pointer-events:none; cursor:default; color:#b7b7b7;">Trang sau</a></li> `;
        }
        str += "</ul></nav>";
        $('#pagination').html(str);
    } else {
        $('#pagination').empty();
    }
}


function nextPage(page, pageSize) {
    console.log("Nextpage:");

    loadData(url, pageSize, page, selectedBrands, selectSort, priceInput[0].value, priceInput[1].value, selectedColors, selectedSizes, selectedStatus);
}