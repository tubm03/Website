var numberViewProduct = 2;

//fetch('https://vapi.vnappmob.com/api/province/')
//    .then(response => {
//        if (!response.ok) {
//            throw new Error('Network response was not ok');
//        }
//        return response.json();
//    })
//    .then(data => {
//        let provinces = data.results;
//        provinces.map(value => document.getElementById('consigneeProvince').innerHTML += '<option value="' + value.province_id + '">' + value.province_name + '</option>');
//    })
//    .catch(error => {
//        console.error('There was a problem with the fetch operation:', error);
//    });

//function getProvinces(event) {
//    let province_id = event.target.value;
//    fetchDistricts(province_id);
//}

//function fetchDistricts(province_id) {
//    fetch('https://vapi.vnappmob.com/api/province/district/' + province_id)
//        .then(response => {
//            if (!response.ok) {
//                throw new Error('Network response was not ok');
//            }
//            return response.json();
//        })
//        .then(data => {
//            let districts = data.results;
//            document.getElementById('consigneeDistrict').innerHTML = '<option value="0">- Quận/Huyện/Thị xã -</option>';
//            document.getElementById('consigneeWard').innerHTML = '<option value="0">- Xã/Phường/Thị trấn -</option>';
//            districts.map(value => document.getElementById('consigneeDistrict').innerHTML += '<option value="' + value.district_id + '">' + value.district_name + '</option>');
//        })
//        .catch(error => {
//            console.error('There was a problem with the fetch operation:', error);
//        });
//}

//function getDistricts(event) {
//    let district_id = event.target.value;
//    fetchWards(district_id);
//}

//function fetchWards(district_id) {
//    fetch('https://vapi.vnappmob.com/api/province/ward/' + district_id)
//        .then(response => {
//            if (!response.ok) {
//                throw new Error('Network response was not ok');
//            }
//            return response.json();
//        })
//        .then(data => {
//            let districts = data.results;
//            document.getElementById('consigneeWard').innerHTML = '<option value="0">- Xã/Phường/Thị trấn -</option>';
//            districts.map(value => document.getElementById('consigneeWard').innerHTML += '<option value="' + value.ward_id + '">' + value.ward_name + '</option>');
//        })
//        .catch(error => {
//            console.error('There was a problem with the fetch operation:', error);
//        });
//}



var OrderItems = [];
var checkoutViewModel = "";

function ItemOrders(OrderId, ProductOptionId, Quantity, Price, PromotionId) {
    this.OrderId = OrderId;
    this.ProductOptionId = ProductOptionId;
    this.Quantity = Quantity;
    this.Price = Price;
    this.PromotionId = PromotionId;
}

function CheckOutViewModel(CheckoutId, OrderItems, OrderName, OrderPhone, OrderEmail, ConsigneeName,
    ConsigneePhone, ConsigneeProvince, ConsigneeDistrict, ConsigneeWard, ConsigneeAddressDetail, PaymentMethod, TotalAmount, DiscountId, ShippingFee, OwnDiscountId) {
    this.CheckoutId = 0;
    this.OrderItems = OrderItems;
    this.OrderName = OrderName;
    this.OrderEmail = OrderEmail;
    this.OrderPhone = OrderPhone;
    this.ConsigneeName = ConsigneeName;
    this.ConsigneePhone = ConsigneePhone;
    this.ConsigneeProvince = ConsigneeProvince;
    this.ConsigneeDistrict = ConsigneeDistrict;
    this.ConsigneeWard = ConsigneeWard;
    this.ConsigneeAddressDetail = ConsigneeAddressDetail;
    this.PaymentMethod = PaymentMethod;
    this.TotalAmount = TotalAmount;
    this.DiscountId = DiscountId;
    this.ShippingFee = ShippingFee;
    this.OwnDiscountId = OwnDiscountId;
}

function AddOrderItems() {
    var index = $('#product_index').val();
    for (var i = 0; i < index; i++) {
        var orderId = 0;
        var productOptionId = $('#product_productOptionId_' + i).val();
        var productPrice = $('#product_price_' + i).val();
        var productQuantity = $('#product_quantity_' + i).val();
        var promotionId = $('#product_promotionId_' + i).val();
        var items = new ItemOrders(
            orderId,
            productOptionId,
            productQuantity,
            productPrice,
            promotionId
        );
        OrderItems.push(items);
    }
}

function ProcessPay() {
    var productsByNull = $('#products_null').val();
    if (productsByNull == "0") {
        alert("Danh sách sản phẩm mua hàng trống!");
        return;
    }
    var orderName = $('#orderName').val();
    var checkOrderName = checkName(orderName, "orderName");
    if (checkOrderName == 0) return;

    var orderPhone = $('#orderPhone').val();
    var checkOrderPhone = checkPhone(orderPhone, "orderPhone");
    if (checkOrderPhone == 0) return;

    var consigneeName = $('#consigneeName').val();
    var checkConsigneeName = checkName(consigneeName, "consigneeName");
    if (checkConsigneeName == 0) return;

    var consigneePhone = $('#consigneePhone').val();
    var checkConsigneePhone = checkPhone(consigneePhone, "consigneePhone");
    if (checkConsigneePhone == 0) return;

    var consigneeProvince = checkSelect("consigneeProvince", "Tỉnh/Thành phố");
    if (consigneeProvince == 0) return;
    var selectConsigneeProvince = $('#consigneeProvince option:selected').text();

    var consigneeDistrict = checkSelect("consigneeDistrict", "Quận/Huyện/Thị xã/");
    if (consigneeDistrict == 0) return;
    var selectConsigneeDistrict = $('#consigneeDistrict option:selected').text();

    var consigneeWard = checkSelect("consigneeWard", "Xã/Phường/Thị trấn");
    if (consigneeWard == 0) return;
    var selectConsigneeWard = $('#consigneeWard option:selected').text();

    var orderEmail = "";
    if (validateEmail($('#orderEmail').val())) {
        orderEmail = $('#orderEmail').val();
    }
    var consigneeAddress = $('#consigneeAddress').val();

    var moneyToCheckout = $('#order_value').val();

    var paymentMethod = $('input[name="pay-method"]:checked').val();

    if (paymentMethod == null) {
        alert("Vui lòng chọn phương thức thanh toán!");
        return;
    }

    var shippingFee = $('#moneyToShip').val();

    console.log(orderName + ' ' + orderPhone + ' ' + consigneeName + ' ' + consigneePhone +
        ' ' + selectConsigneeProvince + ' ' + selectConsigneeDistrict + ' ' + selectConsigneeWard +
        ' ' + orderEmail + ' address ' + consigneeAddress + ' amount: ' + moneyToCheckout + ' payment: ' + paymentMethod);

    AddOrderItems();
    console.log('OrderItems: ' + OrderItems);

    checkoutViewModel = new CheckOutViewModel(
        0,
        OrderItems,
        orderName,
        orderPhone, 
        orderEmail,
        consigneeName,
        consigneePhone,
        selectConsigneeProvince,
        selectConsigneeDistrict,
        selectConsigneeWard,
        consigneeAddress,
        paymentMethod,
        moneyToCheckout,
        discountId,
        shippingFee,
        ownDiscountId
    );
    processInfoCheckout(checkoutViewModel);
    //console.log($('#product_index').val());
    //console.log(OrderItems);
    //console.log(checkoutViewModel);
}

function processInfoCheckout(checkoutViewModel) {
    console.log(checkoutViewModel.OwnDiscountId)
    $.ajax({
        url: '/Checkout/ProcessCheckout',
        type: 'post',
        contentType: 'application/json',
        data: JSON.stringify(checkoutViewModel),
        success: function (response) {
            window.location.href = "/checkout/" + response.urlTransfer + '?orderId=' + response.orderId + '&amount=' + response.amount;
        },
        error: function (xhr, status, error) {
            console.error(error);
        }
    });
}

function checkName(name, typeName) {
    if (name.trim() === "") {
        $('#span_' + typeName).text("Vui lòng nhập tên!");
        return 0;
    }
    resetSpan();
    return 1;
}

function checkPhone(phoneNumber, typePhone) {
    const regex = /^(0|\+84)(3|5|7|8|9)\d{8}$/;

    if (phoneNumber.trim() === "" || !regex.test(phoneNumber)) {
        $('#span_' + typePhone).text("Số điện thoại không hợp lệ!");
        return 0;
    }
    resetSpan();
    return 1;
}

function checkSelect(typeSelect, stringSelect) {
    var selectValue = $('#' + typeSelect).val();
    if (selectValue == "0") {
        $('#span_' + typeSelect).text("Vui lòng chọn " + stringSelect + "!");
        return 0;
    }
    resetSpan();
    return 1;
}

function resetSpan() {
    $('.span_danger').text('');
}

function validateEmail(email) {
    const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return regex.test(email);
}

document.addEventListener('DOMContentLoaded', function () {
    var orderName = document.getElementById('orderName');
    var consigneeName = document.getElementById('consigneeName');
    var orderPhone = document.getElementById('orderPhone');
    var consigneePhone = document.getElementById('consigneePhone');

    orderName.addEventListener('input', function () {
        consigneeName.value = orderName.value;
    });

    orderPhone.addEventListener('input', function () {
        consigneePhone.value = orderPhone.value;
    });
});


let title = '';
let reduce = 0;
let ownReduce = 0;
let discountId = 0;
let ownDiscountId = 0;
function showDiscount(event) {
    event.preventDefault()
    $('#staticBackdrop').modal('show')
}

function chooseDiscount() {
    title = $(this).parent().find('input[name="title"]').val();
    reduce = $(this).parent().find('input[name="reduce"]').val();
    discountId = $(this).val();
    let totalAmount = parseFloat($("#order_value").val()) - parseFloat(reduce);
    $.ajax({
        url: "/checkout/getowndiscounts",
        type: "POST",
        data: { TotalAmount: totalAmount },
        success: function (response) {
            if (response != "Faile") {
                console.log(response.ownDiscounts)
                $('#list-own-discounts').empty();
                for (let item of response.ownDiscounts) {
                    console.log(item)
                    let div_discount = `
                            <div class="discount-card ${item.status == false ? 'out-of-stock' : ""}">
                                <div style="width: 89%">
                                    <div class="header">
                                        <img src="../img/coupon.png" />
                                        <span class="title">${item.discountType.id != 2 ? "Giảm " + item.value + "% Giảm tối đa " + item.maxValue?.toLocaleString("en-US") + " VND" : "Giảm " + item.value?.toLocaleString("en-US") + " VND"}</span>
                                    </div>
                                    <div class="content">
                                        <div class="description">Đơn Tối Thiểu ${parseFloat(item.minPurchase).toLocaleString("en-US")} VND</div>
                                    </div>
                                    <div class="text-tiny">Giới hạn sử dụng: ${item.maxUse} lượt, HSD: ${new Date(item.endDate).toLocaleDateString('vi-VN')}</div>
                                    <div class="footer">
                                        <div class="warning">${item.statusString == null ? '' : item.statusString}</div>
                                    </div>
                                </div>
                                <div>
                                    <input type="radio" name="ownDiscount" value="${item.id}" onclick="chooseOwnDiscount.call(this)" ${item.id == ownDiscountId ? "checked" : ""}></input>
                                    <input type="hidden" name="title" value="${item.title}"></input>
                                    <input type="hidden" name="ownReduce" value="${item.reduce}"></input>
                                </div>
                            </div>`
                    if (item.id == ownDiscountId) {
                        ownReduce = item.reduce
                    }
                    $('#list-own-discounts').append(div_discount)
                }
            }
        }
    })
    let total_reduce = parseFloat(reduce) + parseFloat(ownReduce);
    if (ownDiscountId != 0) {
        $('#number_discount').text('2')
    }
    else {
        $('#number_discount').text('1')
    }

    $('#title').text("Giảm -" + parseFloat(total_reduce).toLocaleString('en-US'));
    $('#title').parent().removeClass('hide')
    console.log(title);
}

function sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

function chooseOwnDiscount() {
    title = $(this).parent().find('input[name="title"]').val();
    ownReduce = $(this).parent().find('input[name="ownReduce"]').val();
    console.log(ownReduce)
    console.log(reduce)
    ownDiscountId = $(this).val();
    console.log(ownDiscountId)
    let total_reduce = parseFloat(reduce) + parseFloat(ownReduce);
    if (discountId != 0) {
        $('#number_discount').text('2')
    }
    else {
        $('#number_discount').text('1')
    }
    $('#title').text("Giảm -" + parseFloat(total_reduce).toLocaleString('en-US'));
    $('#title').parent().removeClass('hide')
    console.log(title);
}

function useDiscount() {
    $('#staticBackdrop').modal('hide')
    let moneyToCheckout = $('#order_value').val()
    let x = moneyToCheckout
    let moneyShip = parseFloat($("#moneyToShip").val());
    console.log('use_discount:' + moneyShip);
    let total_reduce = parseFloat(reduce) + parseFloat(ownReduce);
    moneyToCheckout = parseFloat(moneyToCheckout) - total_reduce + moneyShip;
    console.log(x, reduce, ownReduce, total_reduce, moneyShip)
    $('#money').text(moneyToCheckout.toLocaleString('en', 'US') + " VND")
    $('#moneyToCheckout').val(moneyToCheckout)
    $('#save').text('-' + total_reduce.toLocaleString('en-US') + ' VND')
    $('#moneySave').val(total_reduce);
}