var url = window.location.pathname;
var queryString = window.location.search;
var sortName = "";
var selectStatus = "";
var objects = [];
var sortTotalOrder = "abc";
var sortTotalOrderService = "abc";
var sortTotalFeedback = "abc";
var sortTotalOrderServiceEmployee = "abc";
window.addEventListener('load', function () {
    load(1, 10, "", "", "");

});

function load(pageIndex, pageSize, searchName, sortName, selectStatus) {
    $.ajax({
        url: url,
        type: 'POST',
        data: {
            pageIndex: pageIndex,
            pageSize: pageSize,
            searchName: searchName,
            sortName: sortName,
            selectStatus: selectStatus
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

function displayAccounts(accounts, userType, totalAccount) {
    var html = "";
    $("#list-users").empty().show();
    $('.table-title').show();
    $('#table-content').addClass('wg-table');
    $('.empty-order-history').empty();
    for (var i = 0; i < accounts.length; i++) {
        if (userType == 2) {
            html += elementHtmlEmployee(accounts[i]);
        } else {
            html += elementHtmlCustomer(accounts[i]);
        }
    }
    $('#total-user').html('<strong>Tổng: ' + totalAccount + '</strong>' );
    $("#list-users").html(html);
}

function displayNoData() {
    $('#total-user').empty();
    $('#pagination').empty();
    $('#list-users').hide();
    $('#list-users').html('<div class="empty-order-history text-center"> <i class="fas fa-box-open" style="font-size:100px; " ></i><h5>Không có dữ liệu</h5></div> ').show();
    //$('#no_data').html('<div class="empty-order-history"> <i class="fas fa-box-open" style="font-size:100px; " ></i><h5>Không có dữ liệu</h5></div> ').show();
}

function elementHtmlEmployee(account) {
    var html = '<li class="user-item gap20 pa1215" >';
    html += '<div class="image" >';
    html += '<img src="/areas/images/logo_user/user-avatar.svg.png" />';
    html += '</div>';
    html += '<div class="flex items-center justify-between gap20 flex-grow" id="list-body">';
    html += '<div class="name" >';
    html += '<a href="#" class="body-title-2" onclick="quickViewAccount(\'' + encodeURIComponent(JSON.stringify(account.accountDetail)) + '\')">' + account.accountDetail.fullName + '</a>';
    html += '</div>';
    if (account.accountDetail.phone != null) {
        html += '<div class="body-text phone" style="width:206px!important">' + account.accountDetail.phone + '</div>';
    } else {
        html += '<div class="body-text phone" style="font-weight:600;  font-style: italic;" > - </div>';
    }
    html += '<div class="body-text email" style="width: 288px !important">' + account.accountDetail.email + '</div>';
    html += '<div class="body-text total_fb text-center" style="width: 180px !important;">' + account.totalResponseFeedback + '</div>';
    html += '<div class="body-text totalOrderService text-center" style="width: 152px !important; ">' + account.totalOrderService + '</div>';
    if (account.accountDetail.isDelete == 1) {
        html += '<div class="body-text status block-not-available" style="width: 200px !important;"><span class="">Không kích hoạt</span></div>';
    } else {
        html += '<div class="body-text status block-available" style="width: 200px !important;" ><span class="">Kích hoạt</span></div>';
    }
    html += '<div class="list-icon-function" >';
    html += '<div class="item eye">';
    html += '<i class="icon-eye" onclick="quickViewAccount(\'' + encodeURIComponent(JSON.stringify(account.accountDetail)) + '\')"></i>';
    html += '</div>';
    html += '<div class="item trash">';
    html += `<i class="icon-trash-2" onclick="deleteAccount(${account.accountDetail.accountId}, '${account.accountDetail.fullName}', '${account.accountDetail.isDelete}')"></i>`;
    html += '</div>';
    html += '</div>';
    html += '</div>';
    html += '</li>';
    return html;
}

function nextPage(pageIndex, pageSize) {
    var searchName = document.getElementById("search-input").value;

    console.log('PageIndex: ' + pageIndex);
    console.log('PageSize: ' + pageSize);
    console.log('SearchName: ' + searchName);

    load(pageIndex, pageSize, searchName, sortName, selectStatus);
}

document.addEventListener('DOMContentLoaded', function () {
    var searchInput = document.querySelector('input[name="name"]');

    searchInput.addEventListener('keypress', function (event) {
        if (event.key === 'Enter') {
            event.preventDefault(); // Ngăn chặn hành động mặc định của form
            search();
        }
    });
});

function search() {
    var searchName = document.getElementById("search-input").value;

    console.log("Search query:", searchName);
    load(1, 10, searchName, sortName, selectStatus);
}

function quickViewAccount(accountJson) {
    var account = JSON.parse(decodeURIComponent(accountJson));

    console.log(account);
    $('#profilePopup').modal('show');
    $('#user-name').empty();
    $('#role').empty();
    $('#gender').val('');
    $('#dob').val('');
    $('#phone').val('');
    $('#address').val('');
    $('#email').val('');

    $('#user-name').append(account.fullName);

    if (account.role != null) {
        switch (account.role.roleId) {
            case 1:
                $('#role').append('<span>Quản trị viên</span>');
                break;
            case 2:
                $('#role').append('<span>Nhân viên</span>');
                break;
            case 3:
                $('#role').append('<span>Khách hàng</span>');
                break;
            default:
                $('#role').append('<span>-</span>');
                break;
        }
    } else {
        $('#role').append('<span>Chưa cập nhật</span>');
    }

    if (account.gender != null) {
        if (account.gender == 1) {
            $('#gender').val("Nam");
        } else {
            $('#gender').val("Nữ");
        }
    } else {
        $('#gender').val("-");
    }

    $('#dob').val(account.doB);
    if (account.phone != null) {
        $('#phone').val(account.phone);

    } else {
        $('#phone').val('-');
    }
    if (account.address != null && !(account.address.empty)) {
        $('#address').val(account.address);
    } else {
        $('#address').val('-');
    }
    $('#email').val(account.email);
}

function closeQuickViewAccount() {
    $('#alert-role').empty();
    $('#profilePopup').modal('hide');
}

function sortByName() {
    var searchName = document.getElementById("search-input").value;

    switch (sortName) {
        case "":
            sortName = "abc";
            break;
        case "abc":
            sortName = "zxy";
            break;
        case "zxy":
            sortName = "";
            break;
        default:
            break;
    }
    load(1, 10, searchName, sortName, selectStatus);
}
function sortByField(field, sortOrder, userType) {
    sortOrder = (sortOrder === "abc") ? "zxy" : "abc";
    objects.sort(function (a, b) {
        if (sortOrder === "abc") {
            return a[field] - b[field];
        } else {
            return b[field] - a[field];
        }
    });

    var html = "";
    if (userType == 2) {
        for (var i = 0; i < objects.length; i++) {
            html += elementHtmlEmployee(objects[i]);
        }
    } else {
        for (var i = 0; i < objects.length; i++) {
            html += elementHtmlCustomer(objects[i]);
        }
    }

    $("#list-users").empty().html(html);

    return sortOrder;
}
function sortByTotalOrder() {
    sortTotalOrder = sortByField('totalOrder', sortTotalOrder, 3);
}

function sortByTotalOrderService() {
    sortTotalOrderService = sortByField('totalOrderService', sortTotalOrderService, 3);
}

function sortByTotalFeedback() {
    sortTotalFeedback = sortByField('totalResponseFeedback', sortTotalFeedback, 2);
}

function sortByTotalOrderServiceEmployee() {
    sortTotalOrderServiceEmployee = sortByField('totalOrderService', sortTotalOrderServiceEmployee, 2);
}

/* Add account */

function addAccount() {
    $('#addAccount').modal('show');
}

$(document).ready(function () {
    $('#addAccount').on('shown.bs.modal', function () {
        // Reset the form
        $('#registrationForm')[0].reset();
        // Clear any validation messages
        $('#registrationForm .text-danger').html('');
    });
});

$(function () {
    $('form').submit(function (e) {
        e.preventDefault();
        $.ajax({
            url: this.action,
            type: this.method,
            data: $(this).serialize(),
            success: function (result) {
                if (result.success) {
                    $('#addAccount').modal('hide');
                    $('#notification').modal('show');
                    $('#title-noti').html('Thành Công!');
                    $('#body-noti').html('Tài khoản nhân viên đã tạo thành công. Tiếp tục các hoạt động!');
                    load(1, 10, "", "", "");
                } else {
                    $('span.text-danger').html('');
                    var errorMessage = `<div class="alert alert-danger alert-dismissible fade show rounded-0">
                                                                <span>${result.message}</span>
                                                                <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
                                                            </div>`;
                    if (result.errors == "email") {


                        $('#error-email-mess').html(errorMessage);
                    } else if (result.errors == "phone") {

                        $('#error-phone-mess').html(errorMessage);
                    } else {
                        $.each(result.errors, function (key, errorMessages) {
                            var errorMessage = errorMessages.join('<br>');
                            $('span[data-valmsg-for="'+key+'"]').html(errorMessage);
                        });
                    }
                }
            }
        });
    });
});

function deleteAccount(accountId, accountName, isDelete) {
    if (isDelete == 'false') {
        $('#error-mess').html("");
        $('#deleteAccount').modal('show');
        $('#nameAccountDelete').html(accountName);
        $('#accountId').val(accountId);
        $('#passwordAdmin').val("");
    } else {
        $('#notification').modal('show');
        $('#title-noti').html('Thất bại!');
        $('#body-noti').html('Tài khoản nhân viên đang trong trạng thái không kích hoạt. Tiếp tục các hoạt động!');
    }

}

$(document).ready(function () {
    $('#confirmDelete').click(function () {
        var accountId = $('#accountId').val();
        console.log("accountiD: " + accountId);
        var passwordAdmin = $('#passwordAdmin').val().trim();

        if (passwordAdmin === "") {
            var html = `<div class="alert alert-danger alert-dismissible fade show rounded-0">
                                                                            <span>Vui lòng nhập mật khẩu</span>
                                                                            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
                                                                        </div>`;
            $('#error-mess').html(html);

            return;
        } else {
            $.ajax({
                url: "DeleteAccount",
                type: "post",
                data: { accountId: accountId, passwordAdmin: passwordAdmin },
                success: function (response) {
                    if (response.success) {
                        $('#deleteAccount').modal('hide');
                        $('#notification').modal('show');
                        $('#title-noti').html('Thành Công!');
                        $('#body-noti').html('Tài khoản nhân viên đã tạo xóa thành công. Tiếp tục các hoạt động!');
                        load(1, 10, "", sortName, selectStatus);
                    } else {
                        if (response.isExistAccount == false) {
                            var html = `<div class="alert alert-danger alert-dismissible fade show rounded-0">
                                                                                <span>Account không tồn tại trong hệ thống!</span>
                                                                                <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
                                                                            </div>`;
                            $('#error-mess').html(html);
                        } else if (response.passwordAdmin == false) {
                            var html = `<div class="alert alert-danger alert-dismissible fade show rounded-0">
                                                                                <span>Mật khẩu sai! Vui lòng nhập lại.</span>
                                                                                <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
                                                                            </div>`;
                            $('#error-mess').html(html);
                        } else {
                            $('#deleteAccount').modal('hide');
                            $('#notification').modal('show');
                            $('#title-noti').html('Thất bại!');
                            $('#body-noti').html('Tài khoản nhân viên đang trong trạng thái không kích hoạt. Tiếp tục các hoạt động!');
                        }
                    }
                }
            });
        }
    })
});

function showPassword() {
    var pass = document.getElementById("passwordAdmin");
    var icon = document.getElementById("eye-icon");
    if (pass.type === "password") {
        pass.type = "text";
        icon.classList.replace("bi-eye", "bi-eye-slash");
    } else {
        pass.type = "password";
        icon.classList.replace("bi-eye-slash", "bi-eye");
    }
}

/* Customer */
function elementHtmlCustomer(account) {
    var html = `<li class="user-item gap14">
                    <div class="image">
                        <img src="/areas/images/logo_user/user-avatar.svg.png" alt="">
                    </div>
                    <div class="flex items-center justify-between gap20 flex-grow" id="list-body">
                        <div class="name">
                            <a href="CustomerDetail?userId=${account.accountDetail.userId}" class="body-title-2">${account.accountDetail.fullName}</a>
                        </div>
                        <div class="body-text phone_customer_style" style="width:125px!important">${account.accountDetail.phone != null ? account.accountDetail.phone : "-"}</div>
                        <div class="body-text email">${account.accountDetail.email}</div>
                        <div class="body-text totalOrder text-center">${account.totalOrder}</div>
                        <div class="body-text totalOrderService text-center" style="width: 150px;">${account.totalOrderService}</div>
                        
                            ${account.accountDetail.isDelete == 1 ?
            '<div class="body-text status text-center block-not-available"><span class="">Không kích hoạt</span></div>' :
            '<div class="body-text status text-center block-available"><span class="">Kích hoạt</span></div>'}

                        <div class="list-icon-function ">
                            <div class="item eye text-center" style="width:90px">
                                <a href="CustomerDetail?userId=${account.accountDetail.userId}"><i class="icon-eye"></i></a>
                            </div>
                        </div>
                    </div>
                </li>`;
    return html;
}

function changeStatus(status) {
    var searchName = document.getElementById("search-input").value;

    switch (status) {
        case 0:
            selectStatus = "0";
            break;
        case 1:
            selectStatus = "1";
            break;
        case -1:
            selectStatus = "";
            break;
    }
    load(1, 10, searchName, sortName, selectStatus);
}
