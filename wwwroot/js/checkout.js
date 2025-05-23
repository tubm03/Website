function ItemsCheckoutViewModel(Name, Option, Price, Quantity, ImgUrl, ProductOptionId, ProductId, PromotionId) {
    this.Name = Name;
    this.Option = Option;
    this.Price = Price;
    this.Quantity = Quantity;
    this.ImgUrl = ImgUrl;
    this.ProductOptionId = ProductOptionId;
    this.ProductId = ProductId;
    this.PromotionId = PromotionId;
}

async function selectedProductCheckout() {
    var selectedProduct = [];
    var totalCheckboxId = $('#totalCheckboxId').val();

    for (var i = 0; i < totalCheckboxId; i++) {
        var checkbox = $('#checkbox_' + i);
        if (checkbox.prop('checked') && !checkbox.prop('disabled')) {
            var productName = $('#product_name_' + i).val();
            var imgUrl = $('#product_img_' + i).attr('src');
            var productOption = $('#product_option_' + i).val();
            var productPrice = $('#product_price_' + i).val();
            var productQuantity = $('#product_quantity_' + i).val();
            var productOptionId = $('#product_productOption_' + i).val();
            var productId = $('#product_id_' + i).val();
            var promotionId = $('#product_promotionId_' + i).val();

            var outOfStock = await checkOutOfStock(productOptionId, productQuantity);
            if (outOfStock) {
                alert("Lựa chọn của sản phẩm này đã hết hàng!");
                window.location.reload();
                return;
            }

            var cart = new ItemsCheckoutViewModel(
                productName,
                productOption,
                parseFloat(productPrice), // Convert price to float
                parseInt(productQuantity), // Convert quantity to int
                imgUrl,
                productOptionId,
                productId,
                promotionId
            );
            selectedProduct.push(cart);
        }
    }

    if (selectedProduct.length == 0) {
        alert("Bạn cần chọn ít nhất 1 sản phẩm trong giỏ hàng để tiến hành thanh toán!");
    } else {
        if (selectedProduct.length >= 10) {
            alert("Bạn không thể mua lớn hơn 10 sản phẩm!");
            return;
        }
        $.ajax({
            url: '/Checkout/Form',
            type: 'POST', // Use POST method
            contentType: 'application/json', // Specify the content type
            data: JSON.stringify(selectedProduct), // Convert data to JSON string
            success: function (response) {
                window.location.href = "/checkout/Form";
            },
            error: function (xhr, status, error) {
                console.log(error);
            }
        });
    }
}

async function checkOutOfStock(productOptionId, quantity) {
    try {
        let response = await $.ajax({
            url: '/cart/CheckOutOfStock',
            type: 'POST',
            data: { productOptionId: productOptionId, quantity: quantity }
        });
        return response; // Return true if out of stock, false otherwise
    } catch (error) {
        console.log(error);
        return true; // Treat as out of stock on error
    }
}



document.addEventListener("DOMContentLoaded", function () {
    var checkboxAll = document.getElementById('checkbox_all');
    var totalCheckboxId = parseInt(document.getElementById('totalCheckboxId').value, 10);
    var checkboxItems = document.querySelectorAll('.checkboxItem');

    checkboxAll.addEventListener("change", function () {
        for (var i = 0; i < totalCheckboxId; i++) {
            var checkboxItem = document.getElementById('checkbox_' + i);
            if (checkboxItem) {
                if (!checkboxItem.disabled) {
                    checkboxItem.checked = checkboxAll.checked;
                }
            }
        }
        amountCart();
    });

    checkboxItems.forEach(item => item.addEventListener("change", function () {
        checkboxAll.checked = Array.from(checkboxItems).every(checkbox => checkbox.checked && !checkbox.disabled);
    }));
});
