
var selectedBrands = [];
var selectedColors = [];
var selectedSizes = [];
var selectedStatus = [];
var selectedPromotion = [];
var pageSize = 21;
var pageIndex = 1;
var selectSort = "";
var url = window.location.pathname;
var rangeInput = document.querySelectorAll(".range-input input"),
    priceInput = document.querySelectorAll(".price-input input"),
    range = document.querySelector(".slider .progress");
var priceGap = 1000;

const priceMinInit = priceInput[0].value;
const priceMaxInit = priceInput[1].value;
var rangeInputMin = rangeInput[0].value;
var rangeInputMax = rangeInput[1].value;
const rangeInputMaxInit = rangeInput[1].value;
const rangeInputMinInit = rangeInput[0].value;

window.addEventListener('load', function() {
    // Your code here
    rangeInput[0].value = rangeInputMin;
    rangeInput[1].value = rangeInputMax;
});
rangeInput.forEach(input => {
    input.addEventListener("input", e => {
        let minVal = parseInt(rangeInput[0].value),
            maxVal = parseInt(rangeInput[1].value);

        if ((maxVal - minVal) < priceGap) {
            if (e.target.className === "range-min") {
                rangeInput[0].value = maxVal - priceGap;
                rangeInputMin = rangeInput[0].value;
            } else {
                rangeInput[1].value = minVal + priceGap;
                rangeInputMax = rangeInput[1].value;
            }
        } else {
            priceInput[0].value = formatVND(minVal);
            priceInput[1].value = formatVND(maxVal);
            rangeInputMin = rangeInput[0].value;
            rangeInputMax = rangeInput[1].value;
            range.style.left = ((minVal / rangeInput[0].max) * 100) + "%";
            range.style.right = 100 - (maxVal / rangeInput[1].max) * 100 + "%";
        }
    });
});

function format(amount) {
    return amount.toLocaleString('en-US', { minimumFractionDigits: 0 });
}

document.addEventListener('DOMContentLoaded', function () {
    document.getElementById("filter_button").addEventListener('click', function () {
        selectedBrands = [];
        selectedColors = [];
        selectedSizes = [];
        selectedStatus = [];
        selectedPromotion = [];
        //---brands
        var checkboxesBrands = document.querySelectorAll(".brand-checkbox");
        checkboxesBrands.forEach(function (checkbox) {
            if (checkbox.checked) {
                selectedBrands.push(checkbox.value);
            }
        });
        console.log("filter: " + selectedBrands);

        //----price
        console.log("min: " + priceInput[0].value);
        console.log("max: " + priceInput[1].value);
        console.log("pmin: " + priceMinInit);
        console.log("pmax: " + priceMaxInit);

        //---size
        var checkboxesSize = document.querySelectorAll(".size-checkboxes");
        checkboxesSize.forEach(function (checkbox) {
            if (checkbox.checked) {
                selectedSizes.push(checkbox.value);
            }
        });
        console.log("Size: " + selectedSizes);

        //---color
        var checkboxesColor = document.querySelectorAll(".color-checkboxes");
        checkboxesColor.forEach(function (checkbox) {
            if (checkbox.checked) {
                selectedColors.push(checkbox.value);
            }
        });
        console.log("Color: " + selectedColors);

        //---status
        var checkboxesStatus = document.querySelectorAll(".status-checkboxes");
        checkboxesStatus.forEach(function (checkbox) {
            if (checkbox.checked) {
                selectedStatus.push(checkbox.value);
            }
        });
        console.log("status: " + selectedColors);

        //-- Promotion
        let checkboxesPromotion = document.querySelectorAll(".promotion-checkboxes");
        checkboxesPromotion.forEach(function (checkbox) {
            if (checkbox.checked) {
                selectedPromotion.push(checkbox.value);
            }
        });


        loadData(url, pageSize, 1, selectedBrands, selectSort, rangeInput[0].value, rangeInput[1].value, selectedColors, selectedSizes, selectedStatus, selectedPromotion);
    });

    document.querySelector("#clear_button").addEventListener('click', function () {
        var checkboxes = document.querySelectorAll(".brand-checkbox");
        checkboxes.forEach(function (checkbox) {
            checkbox.checked = false;
        });
        var checkboxes = document.querySelectorAll(".size-checkboxes");
        checkboxes.forEach(function (checkbox) {
            checkbox.checked = false;
        });
        var checkboxes = document.querySelectorAll(".color-checkboxes");
        checkboxes.forEach(function (checkbox) {
            checkbox.checked = false;
        });

        var checkboxes = document.querySelectorAll(".status-checkboxes");
        checkboxes.forEach(function (checkbox) {
            checkbox.checked = false;
        });
        selectedStatus = [];
        selectedBrands = [];
        // Reset price inputs
        priceInput[0].value = formatVND(priceMinInit);
        priceInput[1].value = formatVND(priceMaxInit);

        console.log(rangeInputMax);

        // Update range inputs
        rangeInput[0].value = rangeInputMinInit;
        rangeInput[1].value = rangeInputMaxInit;

        // Update slider progress
        range.style.left = "0%";
        range.style.right = "0%";

        var checkboxes = document.querySelectorAll('.promotion-checkboxes');
        checkboxes.forEach(function (checkbox) {
            checkbox.checked = false;
        });

        console.log("Filters cleared");
        loadData(url, pageSize, 1, [], selectSort, rangeInputMinInit, rangeInputMaxInit, [], [], [], []);
    });

});

function loadData(url, pageSize, page, selectedBrands, selectSort, rangeInputMin, rangeInputMax, selectedColors, selectedSizes, selectedStatus, selectedPromotion) {
    $('#data-grid-view').empty();
    $('#list-view').empty();
    $.ajax({
        url: '/Product/ListProduct',
        type: 'post',
        data: {
            url: url,
            pageSize: pageSize, page: page, selectedBrands: selectedBrands, selectedSort: selectSort,
            priceMin: rangeInputMin, priceMax: rangeInputMax, selectedColors: selectedColors, selectedSizes: selectedSizes,
            selectedStatus: selectedStatus, selectedPromotion: selectedPromotion
        },
        success: function (response) {
            if (response.data.length > 0) {
                let html1 = "";
                let htmlGridView = "";
                var html = "";
                var items = response.data;
                for (var index = 0; index < items.length; index++) {


                    var isFavorite = false;
                    for (var i = 0; i < response.wishlist.length; i++) {
                        if (response.wishlist[i] == items[index].productId) {
                            isFavorite = true;
                        }
                    }

                    const favoriteClass = isFavorite ? "favorite" : "not-favorite";
                    const favoriteColor = isFavorite ? "red" : "black";

                    htmlGridView += "<div id='data-grid-view' class='row border-hover-effect'>";
                    htmlGridView += "</div>";
                    html += "<!-- Single Product Start -->";
                    html += "<div class='col-xl-4 col-lg-6 col-md-6'>";
                    html += "<div class='single-template-product'>";
                    html += "<!-- Product Image Start -->";
                    html += "<div class='pro-img'>";
                    if (items[index].promotion != null && items[index].promotion.promotionId != 0) {
                        html += "<span class='sticker-sale'>-" + items[index].promotion.value + "%</span>";
                    }
                    html += "<a href='/product/detail/" + items[index].productId + "'>";

                    if (items[index].productOption && items[index].productOption.length > 0) {
                        var isSoldOutAll = true;
                        for (var i = 0; i < items[index].productOption.length; i++) {
                            if (items[index].productOption[i].isSoldOut == false || items[index].productOption[i].quantity != 0) {
                                isSoldOutAll = false;
                                break;
                            }
                        }
                        if (isSoldOutAll == false) {
                            html += "<img style='max-height: 300px' class='primary-img' src='" + items[index].productOption[0].img_url + "' alt='single-product'>";
                        }
                        else {
                            //html += "<div class ='product'>";
                            html += "<img style='max-height: 300px' class='primary-img img_sold_out ' src='" + items[index].productOption[0].img_url + "' alt='single-product'>";
                            html += "<div class='overlay'>Hết hàng</div>";
                            //html += "</div>";
                        }
                    }

                    html += "</a>";
                    html += "<div class='item_quick_link'>";
                    html += "<a href='#' title='Xem chi tiết' onclick='quickView(" + items[index].productId + ")' ><i class='icofont-search'></i></a>";
                    html += "</div>";
                    html += "<div class='product-count-wrap'>";
                    html += "</div>";
                    html += "</div>";
                    html += "<!-- Product Image End -->";
                    html += "<!-- Product Content Start -->";
                    html += "<div class='product_content_wrap'>";
                    html += "<div class='product_content'>";
                    html += "<h4><a style='height: 40px;'  href='/product/detail/" + items[index].productId + "'>" + items[index].name + "</a></h4>";
                    html += "<div class='grid_price'>";

                    if (items[index].productOption && items[index].productOption.length > 0) {
                        const amount = items[index].productOption[0].price;
                        const formattedAmount = formatVND(amount);
                        if (items[index].promotion != null && items[index].promotion.promotionId != 0) {
                            const amountAfterDiscount = amount * (1 - items[index].promotion.value / 100);
                            html += "<span class='regular-price'>" + formatVND(amountAfterDiscount) + " VND</span>";
                            html += "<del class='discount_price'>" + formatVND(amount) + "VND</del>";
                        } else {
                            html += "<span class='regular-price'>" + formatVND(amount) + " VND</span>";
                        }

                    }

                    html += "</div>";
                    html += "</div>";
                    html += "<div class='item_add_cart'>";
                    //html += "<a class='grid_compare' href='compare.html' title='Compare'><i class='icofont-random'></i></a>";
                    html += '<a class="grid_cart details_cart link " href="#" data-bs-toggle="modal" data-bs-target="#myModal" title="Add to Cart" onclick="quickView(' + items[index].productId + ')">Thêm Vào Giỏ Hàng</a>';
                    html += "<a class='grid_wishlist' title='Wishlist'><i class='icofont-heart-alt " + favoriteClass + "' data-id='" + items[index].productId + "' style='color: " + favoriteColor + "; cursor: pointer;' onclick='ToggleFavorite(" + items[index].productId + ", this)'></i></a>";

                    html += "</div>";
                    html += "</div>";
                    html += "<!-- Product Content End -->";
                    html += "</div>";
                    html += "</div>";
                    html += "<!-- Single Product End -->";

                    html1 += "<div class='single-template-product'>";
                    html1 += "<!-- Product Image Start -->";
                    html1 += "<div class='pro-img'>";
                    if (items[index].promotion != null && items[index].promotion.promotionId != 0) {
                        html1 += "<span class='sticker-sale'>-" + items[index].promotion.value + "%</span>";
                    }
                    html1 += "<a href='/product/detail/" + items[index].productId + "'>";
                    if (items[index].productOption && items[index].productOption.length > 0) {
                        var isSoldOutAll = true;
                        for (var i = 0; i < items[index].productOption.length; i++) {
                            if (items[index].productOption[i].isSoldOut == false || items[index].productOption[i].quantity != 0) {
                                isSoldOutAll = false;
                                break;
                            }
                        }
                        if (isSoldOutAll == false) {
                            html1 += "<img style='max-height: 300px' class='primary-img' src='" + items[index].productOption[0].img_url + "' alt='single-product'>";
                        }
                        else {
                            html1 += "<img style='max-height: 300px' class='primary-img img_sold_out ' src='" + items[index].productOption[0].img_url + "' alt='single-product'>";
                            html1 += "<div class='overlay'>Hết hàng</div>";
                        }
                    }
                    html1 += "</a>";
                    html1 += "<div class='item_quick_link'>";
                    html1 += "<a href='#' title='Xem chi tiết' onclick='quickView(" + items[index].productId + ")'><i class='icofont-search'></i></a>";
                    html1 += "</div>";
                    html1 += "<div class='product-count-wrap'>";
                    html1 += "</div>";
                    html1 += "</div>";
                    html1 += "<!-- Product Image End -->";
                    html1 += "<!-- Product Content Start -->";
                    html1 += "<div class='product_content_wrap'>";
                    html1 += "<div class='product_content'>";
                    html1 += "<h4 style='height: 33px;'><a href='/product/detail/" + items[index].productId + "'>" + items[index].name + "</a></h4>";
                    html1 += "<div class='scrollable-description'>";
                    html1 += "<p class='list_des' " + items[index].description + "</p>";
                    html1 += "</div>";
                    html1 += "<div class='grid_price'>";
                    if (items[index].productOption && items[index].productOption.length > 0) {
                        const amount = items[index].productOption[0].price;
                        const formattedAmount = formatVND(amount);
                        if (items[index].promotion != null && items[index].promotion.promotionId != 0) {
                            const amountAfterDiscount = amount * (1 - items[index].promotion.value / 100);
                            html1 += "<span class='regular-price'>" + formatVND(amountAfterDiscount) + " VND</span>";
                            html1 += "<del class='discount_price'>" + formatVND(amount) + "VND</del>";
                        } else {
                            html1 += "<span class='regular-price'>" + formatVND(amount) + " VND</span>";
                        }

                    }
                    html1 += "</div>";
                    html1 += "</div>";
                    html1 += "<div class='item_add_cart'>";
                    //html1 += "<a class='grid_compare' href='compare.html1' title='Compare'><i class='icofont-random'></i></a>";
                    html1 += '<a class="grid_cart details_cart link " href="#" data-bs-toggle="modal" data-bs-target="#myModal" title="Add to Cart" onclick="quickView(' + items[index].productId + ')">Thêm Vào Giỏ Hàng</a>';
                    html1 += "<a class='grid_wishlist' title='Wishlist'><i class='icofont-heart-alt " + favoriteClass + "' data-id='" + items[index].productId + "' style='color: " + favoriteColor + "; cursor: pointer;' onclick='ToggleFavorite(" + items[index].productId + ", this)'></i></a>";

                    html1 += "</div>";
                    html1 += "</div>";
                    html1 += "<!-- Product Content End -->";
                    html1 += "</div>";
                }
                console.log("pageSize:" + response.pageSize);
                $('#grid-view').html(htmlGridView);
                $('#data-grid-view').html(html);
                $('#list-view').html(html1);
                pageIndex = response.currentPage;
                paging(response.currentPage, response.numberPage, response.pageSize);
            } else {
                $('#grid-view').empty();
                var gridView = document.getElementById('grid-view');
                var htmlAnnounce = "<div id='no-products' style='text-align: center; margin-left: 20px;'><h4>Sản phẩm khách hàng lựa chọn hiện không có!</h4></div>";
                $('#grid-view').html(htmlAnnounce);
                $('#list-view').html(htmlAnnounce);
                $('#pagination').html("");
                console.error("Unexpected response structure:", response);

            }
        },
        error: function (xhr, status, error) {
            console.error("Error:", xhr.responseText);
        }

    });
    window.scrollTo(0, 0);
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

    loadData(url, pageSize, page, selectedBrands, selectSort, rangeInput[0].value, rangeInput[1].value, selectedColors, selectedSizes, selectedStatus, selectedPromotion);
}
function changePageSize() {
    console.log("changePageSize:");
    console.log("PriceM: " + rangeInput[0].value);
    console.log("PriceMax: " + rangeInput[1].value);
    pageSize = parseInt(document.getElementById("pageSizeSelect").value);
    console.log(pageSize);
    loadData(url, pageSize, 1, selectedBrands, selectSort, rangeInput[0].value, rangeInput[1].value, selectedColors, selectedSizes, selectedStatus, selectedPromotion);
}

function formatVND(amount) {
    return amount.toLocaleString('en-US', '#.###');
}

function ToggleFavorite(productId, element) {
    var $this = $(element); // The <i> element that was clicked
    var isFavorite = $this.hasClass('favorite');

    // Toggle the favorite class
    $this.toggleClass('favorite not-favorite');
    // Update the color based on the new state
    if (isFavorite) {
        $this.css('color', 'black');
    } else {
        $this.css('color', 'red');
    }

    $.ajax({
        url: '/Product/ToggleFavorite',
        type: 'POST',
        data: { productId: productId },
        success: function (response) {
            console.log('AJAX success:', response);
        },
        error: function (xhr, status, error) {
            var errorMessage;
            console.log(xhr);
            console.log(status);
            console.log(error);
            // Kiểm tra mã trạng thái lỗi và đặt thông báo lỗi tương ứng
            switch (xhr.status) {
                case 400:
                    errorMessage = 'Yêu cầu không hợp lệ. Vui lòng kiểm tra lại thông tin.';
                    break;
                case 401:
                    errorMessage = 'Bạn chưa đăng nhập. Vui lòng đăng nhập để thực hiện thao tác này.';
                    break;
                case 403:
                    errorMessage = 'Bạn không có quyền thực hiện thao tác này.';
                    break;
                case 404:
                    errorMessage = 'Không tìm thấy sản phẩm. Vui lòng thử lại.';
                    break;
                case 500:
                    errorMessage = 'Lỗi máy chủ. Vui lòng thử lại sau.';
                    break;
                default:
                    errorMessage = 'Đã xảy ra lỗi không xác định. Vui lòng thử lại.';
            }

            alert(errorMessage);

            // Revert the favorite status on error
            $this.toggleClass('favorite not-favorite');
            if (isFavorite) {
                $this.css('color', 'red');
            } else {
                $this.css('color', 'black');
            }
        }
    });
}

function selectedSort() {
    selectSort = document.getElementById("selected_sort").value;
    console.log("selected_sort: change" + selectSort);
    loadData(url, pageSize, pageIndex, selectedBrands, selectSort, rangeInputMin, rangeInputMax, selectedColors, selectedSizes, selectedStatus);
}
