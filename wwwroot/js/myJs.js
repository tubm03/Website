window.onload = function () {
    getCartBoxItems();
    amountCart();
    loadMenu();
};

let discount = 0;

function loadMenu() {
    $.ajax({
        type: "POST",
        url: "/Home/GetCategoryMenu",
        success: function (response) {
            let dogFoodContent = '';
            response.dogFoodCategory.forEach(function (cate) {
                dogFoodContent += `<li><a href="/Product/DogFood/${cate.productCateId}">${cate.name}</a></li>`;
            });

            let dogAccessoryContent = '';
            response.dogAccessoryCategory.forEach(function (cate) {
                dogAccessoryContent += `<li><a href="/Product/DogAccessory/${cate.productCateId}">${cate.name}</a></li>`;
            });

            let catFoodContent = '';
            response.catFoodCategory.forEach(function (cate) {
                catFoodContent += `<li><a href="/Product/CatFood/${cate.productCateId}">${cate.name}</a></li>`;
            });

            let catAccessoryContent = '';
            response.catAccessoryCategory.forEach(function (cate) {
                catAccessoryContent += `<li><a href="/Product/CatAccessory/${cate.productCateId}">${cate.name}</a></li>`;
            });

            let serviceContent = '';
            response.services.forEach(function (service) {
                serviceContent += `<li><a href="/service/detail/${service.serviceId}">${service.name}</a></li>`;
            });

            $("#DogFood").append(dogFoodContent);
            $("#DogAccessory").append(dogAccessoryContent);
            $("#CatFood").append(catFoodContent);
            $("#CatAccessory").append(catAccessoryContent);
            $("#serviceMenu").append(serviceContent);
        },
        error: function (xhr, status, error) {
            console.error('The request failed!', status, error);
        }
    });
}

function addToCart(productOptionId, quantity) {
    $.ajax({
        type: "POST",
        url: "/cart/AddToCart",
        data: { productOptionId: productOptionId, quantity: quantity },
        success: function (response) {
            if (response.message != 'success') {
                alert(response.message)
            }
            else {
                getCartBoxItems();
                showNotification('Thêm sản phẩm thành công', 'green');
            }
        },
        error: function (xhr, status, error) {
            console.error('The request failed!', status, error);
        }
    });
    $('#myModal').modal('hide');
    
}

function getCartBoxItems() {
    $.ajax({
        type: "POST",
        url: "/cart/GetCartBoxItems",
        success: function (response) {
            $('#list_item').empty();
            $('#total_item').html(response.length)
            let total_price = 0;
            if (response.length == 0) {
                $('#list_item').html('<h5>Không có sản phẩm nào trong giỏ hàng</h5>')
            }
            else {
                for (const element of response) {
                    if (element.promotion != null) {
                        element.price = element.price * (1 - element.promotion.value/100)
                    }
                    let divSingleCart = $('<div>', {
                        class: "single-cart-box"
                    })

                    let divCartImg = $('<div>', {
                        class: "cart-img"
                    })

                    let divCartContent = $('<div>', {
                        class: "cart-content"
                    })

                    let img = $('<img>', {
                        src: element.imgUrl,
                        alt: "cart-image"
                    })

                    let h6 = $('<h6>', {
                        style: "margin: 0px"
                    })
                    let aTitle = $('<a>', {
                        href: 'http://localhost:5206/product/detail/' + element.productId
                    }).text(element.name)
                    let option = "";
                    if (element.size.name != null) {
                            option += element.size.name
                    }
                    let spanOption = $('<div>', {
                        style: "font-size: 13px; margin-top: 3px; margin-bottom: 3px"
                    }).text(option)
                    let spanPrice = $('<span>', {
                        class: 'cart-price'
                    }).text(element.price.toLocaleString('en-US') + ' x ' + element.quantity)
                    h6.append(aTitle)
                    divCartImg.append(img)
                    divCartContent.append(h6)
                    divCartContent.append(spanOption)
                    divCartContent.append(spanPrice)

                    divSingleCart.append(divCartImg)
                    divSingleCart.append(divCartContent)
                    $('#list_item').append(divSingleCart)

                    total_price += parseFloat(element.price) * parseFloat(element.quantity)
                }
            }
            let x = total_price.toLocaleString('en-US') + " VND"
            $('#total_price').html(x);
            
        },
        error: function (xhr, status, error) {
            console.error('Error: ' + error);
        }
    });
}

function deleteCartItem(productOptionId) {
    $('#' + productOptionId).remove()
    let list = document.getElementsByClassName('item');
    if (list.length == 0) {
        let x = `
                <div class="row justify-content-center">
                    <div class="col-md-6 col-sm-10 text-center">
                        <div class="text-center">
                        <img style="width: 70%" src="/img/notfound/cart-empty.jpg" alt="Giỏ hàng trống" />
                        </div>
                        <div class="buttons-cart d-inline-block ">
                            <a style="margin:0" href="/">Tiếp tục mua sắm</a>
                        </div>
                    </div>
                </div>
                `;
        $('#cart_content').html(x);
    }
    $.ajax({
        url: "http://localhost:5206/cart/delete",
        type: "DELETE",
        data: { productOptionId: productOptionId },
        success: function (response) {
            getCartBoxItems();
        }
    });
    showNotification('Sản phẩm đã được xóa', 'red')
    amountCart();
}

function amountCart() {
    let total_amount = 0.0;
    let totalCheckboxId = $('#totalCheckboxId').val();
    for (let i = 0; i < totalCheckboxId; i++) {
        let checkbox = $('#checkbox_' + i);
        if (checkbox.prop('checked')) {
            total_amount += parseFloat($('#product_totalPrice_' + i).text().replace(/[^0-9.-]+/g, ""));
        }
    }
    $('#amount').html(total_amount.toLocaleString('en-US'))
}
function quickEditCartItem(oldProductOptionId, productId) {
    $('#quick_add_to_cart').attr('data-old-product-option-id', oldProductOptionId)
    quickView(productId)
    $('#myModal').modal('show');
}
function editCartItem(oldProductOptionId, newProductOptionId, quantity) {
    $.ajax({
        url: "http://localhost:5206/cart/Edit",
        method: "PUT",
        data: { oldProductOptionId: oldProductOptionId, newProductOptionId: newProductOptionId, quantity: quantity },
        success: function (response) {
            if (response.message != null) {
                alert(response.message)
            }
            else {
                updateCartItem(oldProductOptionId, response)
                getCartBoxItems()
                $('#myModal').modal('hide');
                amountCart();
                showNotification('Sản phẩm đã được lưu', 'green');
            }
        }
    });
}

function updateCartItem(oldId, cartItem) {
    let option = "";
    if (cartItem.size != null) {
            option += "( " + cartItem.size.name + " )";
    }
    // Find the row by ProductOptionId
    let row = $('#' + oldId);
    // Update the product name and link
    row.find('.product-name a').attr('href', 'http://localhost:5206/product/detail/' + cartItem.productId)
        .html(cartItem.name + '</br>' + option);

    // Update the product image
    row.find('.product-thumbnail img').attr('src', cartItem.imgUrl);

    let price = cartItem.price;

    if (cartItem.promotion != null) {
        price = price * (1 - (parseFloat(cartItem.promotion.value) / 100));
    }
    console.log("cartItem", cartItem);

    console.log("quantityInstock", cartItem.quantityInStock);
    console.log("quantity", cartItem.quantity);

    console.log("abc");


    // Update the product price
    row.find('.product-price .amount').text(price.toLocaleString('en-US') + ' VND');
    if (cartItem.promotion != null) {
        row.find('.discount_price').text(cartItem.price.toLocaleString('en-US') + ' VND')
    }

    // Update the product quantity
    row.find('.product-quantity input').val(cartItem.quantity);

    // Update the product subtotal
    let sub_total = parseFloat(price) * parseFloat(cartItem.quantity)
    row.find('.product-subtotal').text(sub_total.toLocaleString('en-US') + ' VND')

    // Update the edit button
    row.find('a[title="Chỉnh sửa"]').attr('onclick', `quickEditCartItem(${cartItem.productOptionId}, ${cartItem.productId})`);

    // Update the delete button
    row.find('a[title="Xóa sản phẩm"]').attr('onclick', `deleteCartItem(${cartItem.productOptionId})`);

    if (oldId != cartItem.productOptionId) {
        row.find('intput[type="checkbox"]').addClass('checkbox_' + cartItem.productOptionId).removeClass('checkbox_' + oldId);
    }

    if (cartItem.quantityInStock < cartItem.quantity) {

        row.find('.checkbox_' + cartItem.productOptionId).attr('disabled', true);
    } else {
        row.find('.checkbox_' + oldId).attr('disabled', false);
    }

    row.find('.productOptionSelected').val(cartItem.productOptionId);

    row.attr('id', cartItem.productOptionId)


}

function quickView(productId) {
    console.log('quickView')
    $('#quick_add_to_cart').removeClass('out-of-stock');
    $('#quick_add_to_cart').html('Thêm vào giỏ hàng');
    $('#quick_quantity').attr('readonly', false);
    $('#myModal').modal('show');
    $('#quick_attribute').empty();
    $('#quick_size').empty();
    $.ajax({
        type: "POST",
        url: "http://localhost:5206/Product/quickPreview",
        data: { productId: productId },
        success: function (response) {
            $('#quick_name').html(response.name);
            let price = response.productOption[0].price
            if (response.promotion != null) {
                $('#quick_price_discount').html(price.toLocaleString('en-US') + ' VND');
                price = response.productOption[0].price * (1 - response.promotion.value / 100);
                $('#quick_price').html(price.toLocaleString('en-US'));
                discount = response.promotion.value;
            }
            else {
                $('#quick_price').html(price.toLocaleString('en-US'));
                $('#quick_price_discount').html('');
                discount = 0;
            }
            
            $('#quick_image').empty();
            $('#quick_amount').html(price.toLocaleString('en-US'));
            $('#quick_add_to_cart').attr('data-product-option-id', response.productOption[0].id)

            let imgDiv = $('<div>', {
                class: 'tab-pane large-img-style active'
            });
            let imgElement = $('<img>', {
                src: response.productOption[0].img_url,
                alt: ''
            });
            imgDiv.append(imgElement);
            $('#quick_image').append(imgDiv);

            if (response.promotion != null) {
                let sale = $('<span>', {
                    class: 'sticker-sale',
                    text: '-' + response.promotion.value + '%'
                })
                $('#quick_image').append(sale);
            }

            $('#quick_image').append(imgElement);
/*            $('#quick_brand').html('Thương hiệu: ' + response.brand);*/
            let jsonStr = JSON.stringify(response.productOption);

            /*if (response.attributes[0].name != null) {
                let divContainer = $('<div>', {
                    class: 'color clearfix mb-30'
                });
                let label = $('<label>').text('Loại :');
                let ul = $('<ul>', {
                    class: 'color-list scroll'
                });
                let list = ""
                for (let index = 0; index < response.attributes.length;) {
                    if (response.attributes[index].name != null) {
                        list += "<li id='quick_attribute_" + response.attributes[index].attributeId + "' style = 'cursor:pointer' onclick='quick_attribute_selected(" + index + "," + response.attributes[index].attributeId + "," + jsonStr + ")'" + "data = '" + response.attributes[index].attributeId + "'>" + response.attributes[index].name + "</li>"
                        index += 1
                    }
                    else {
                        index -= 1
                    }
                }
                ul.append(list)
                divContainer.append(label);
                divContainer.append(ul);
                $('#quick_attribute').append(divContainer);
            }*/

            if (response.sizes[0].name != null) {
                let sizeLabel = $('<label>').text('Kích thước :');
                let sizeDiv = $('<div>', {
                    class: 'color-list size_list'
                });
                let sizeUl = $('<ul>', {
                    class: 'color-list size_list scroll'
                });

                let list = ""
                for (let index = 0; index < response.sizes.length;) {
                    if (response.sizes[index].name != null) {
                        list += "<li id='quick_size_" + response.sizes[index].sizeId + "' style = 'cursor:pointer' onclick='quick_size_selected(" + index + "," + response.sizes[index].sizeId + "," + jsonStr + ")'" + "data = '" + response.sizes[index].sizeId + "'>" + response.sizes[index].name + "</li>"
                        index += 1
                    }
                    else {
                        index -= 1
                    }

                }
                sizeUl.append(list)
                sizeDiv.append(sizeUl);
                $('#quick_size').append(sizeLabel);
                $('#quick_size').append(sizeDiv);
            }

            let isStill = false;
            let attributeId, sizeId, quantityInStock;
            for (const element of response.productOption) {
                if (!element.isSoldOut) { // status is boolean
                    sizeId = element.size.sizeId;
                    quantityInStock = element.quantity;
                    isStill = true
                    break;
                }

            }
            // updatePriceAndImage(); // Uncomment this line if you have a function to update price and image
            if (isStill) {
                let list_size = document.getElementById('quick_size').querySelectorAll('li');

                if (list_size.length > 0) {
                    let sizeElement = document.getElementById('quick_size_' + sizeId);
                    if (sizeElement) {
                        sizeElement.classList.add('select');
                    } else {
                        console.warn('Size element not found: size_' + sizeId);
                    }
                }

                if (list_size.length > 0) {
                    quickUpdatePriceAndImage(sizeId, attributeId, response.productOption)
                }

                console.log(quantityInStock)

                $('#quick_quantityInStock').text(quantityInStock)
            }
            else {
                let divSoldOut = $('<div>', {
                    class: 'overlay'
                }).text('Hết hàng')
                console.log("Het hang")
                imgDiv.append(divSoldOut);
                imgElement.addClass('out-of-stock');
                $('#quick_add_to_cart').addClass('out-of-stock');
                $('#quick_add_to_cart').html('Đã bán hết');
                let list_size = $('#quick_size li')
                let list_attribute = $('#quick_attribute li')
                if (list_size.length > 0) {
                    list_size.each(function () {
                        $(this).addClass('out-of-stock')
                    })
                }
                if (list_attribute.length > 0) {
                    list_attribute.each(function () {
                        $(this).addClass('out-of-stock')
                    })
                }
                $('#quick_quantity').attr('readonly', true);
            }
            document.getElementById('quick_quantity').value = 1

        },
        error: function (xhr, status, error) {
            console.error('Error: ' + error);
        }
    });
}

function quickCheckOutOfStock(sizeId, attributeId, productOptions) {
    let list_size = document.getElementById('quick_size').querySelectorAll('li');
    let list_attribute = document.getElementById('quick_attribute').querySelectorAll('li');
    if (list_size.length > 0) {
        for (const element of productOptions) {
            if (element.isSoldOut) {
              $('#quick_size_' + element.size.sizeId).addClass('out-of-stock')
            }
        }
    }
    else if (list_size.length > 0) {
        for (const element of productOptions) {
            if (element.isSoldOut) {
                $('#quick_size_' + element.size.sizeId).addClass('out-of-stock')
            }
        }
    }
}

function quick_size_selected(index, size_id, productOption) {
    console.log(index, size_id, productOption)
    let list_size = document.getElementById('quick_size').querySelectorAll('li');
    list_size.forEach(function (li) {
        li.classList.remove('select');
        li.classList.remove('out-of-stock')
    });
    list_size[index].classList.add('select');
    quickUpdatePriceAndImage(size_id, null, productOption);
}

function quick_attribute_selected(index, attribute_id, productOption) {
    let list_attr = document.getElementById('quick_attribute').querySelectorAll('li');
    let list_size = document.getElementById('quick_size').querySelectorAll('li');
    list_attr.forEach(function (li) {
        li.classList.remove('select');
        li.classList.remove('out-of-stock')
    });
    list_size.forEach(function (li) {
        li.classList.remove('out-of-stock')
    });
    list_attr[index].classList.add('select');

    let size_id
    try {
        size_id = document.getElementById('quick_size').querySelectorAll('li.select')[0].getAttribute('data')
    } catch (error) {
        size_id = 1
    }
    quickUpdatePriceAndImage(size_id, null, productOption);
}

function quickUpdatePriceAndImage(size_id, attribute_id, productOptions_json) {
    let quick_price, quick_img_url;

    for (const element of productOptions_json) {
        if (element['size']['sizeId'] == size_id) {
            quick_price = element.price;
            let quantityInStock = element.quantity;
            if (discount != 0) {
                document.getElementById('quick_price_discount').innerText = quick_price.toLocaleString('en-US') + ' VND';
                quick_price = quick_price * (1 - discount / 100)
                document.getElementById('quick_price').innerText = quick_price.toLocaleString('en-US');
            }
            else {
                document.getElementById('quick_price').innerText = quick_price.toLocaleString('en-US');
            }
            $('#quick_quantityInStock').text(quantityInStock)
            quick_img_url = element.img_url;
            $('#quick_add_to_cart').attr('data-product-option-id', element.id)
            break;
        }
    }
    document.querySelector('#quick_image img').setAttribute('src', quick_img_url);
    
    quick_total_price();
    quickCheckOutOfStock(size_id, attribute_id, productOptions_json);
}

function quick_total_price() {
    let quick_quantity = parseFloat(document.getElementById('quick_quantity').value);
    let quick_price = parseFloat(document.getElementById('quick_price').innerText.replace(/,/g, ''));
    let quick_amount = quick_price * quick_quantity;
    document.getElementById("quick_amount").innerText = quick_amount.toLocaleString('en-US');
}

function validateQuantity(input) {
    let value = parseInt(input.value);

    if (isNaN(value) || value < 1 || value > 10) {
        input.value = 1; // Đặt giá trị về 1 khi nhập sai
    }
}

function showNotification(message, color) {
    let notification = document.getElementById('notification');
    notification.innerText = message;
    notification.style.backgroundColor = color;
    notification.style.display = 'block';
    setTimeout(function () {
        notification.style.display = 'none';
    }, 2000);
}

function generateProductList(products) {
    console.log(products)
    let productList = $('#product-list');
    productList.empty();
    let index = 0;
    products.forEach(function (product) {
        let productItem = `
                    <li class="product-item product-${product.id} gap14 index-${index}">
                        <div class="image no-bg">
                            <img src="${product.imgUrl}" alt="">
                        </div>
                        <div class="flex items-center justify-between gap20 flex-grow">
                            <div class="name">
                                <a href="/admin/product/detail?productId=${product.id}" class="body-title-2 product-name">${product.name}</a>
                            </div>
                            <div class="body-text product-id">#${product.id}</div>
                            <div class="body-text"><span class="price">${product.price.toLocaleString()}</span> VND</div>
                            <div class="body-text soldQuantity">${product.soldQuantity}</div>
                            <div>
                                <div style="width: 100%" class=" isSoldOut ${product.isSoldOut ? 'block-not-available' : 'block-available'}">
                                    ${product.isSoldOut ? 'Hết hàng' : product.quantity}
                                </div>
                            </div>
                            <div>
                                <div style="width: 100%" class="isDelete ${product.isDelete ? 'block-not-available' : 'block-available'}">
                                    ${product.isDelete ? 'Ngừng bán' : 'Còn bán'}
                                </div>
                            </div>
                            <div class="list-icon-function">
                                <div class="item eye" data-bs-toggle="modal" data-bs-target="#myModal" title="Xem chi tiết" onclick="quickViewForAdmin(${product.id})">
                                    <a href="/admin/product/detail?productId=${product.id}"><i class="icon-eye"></i></a>
                                </div>
                                <div class="item edit">
                                    <a href="/admin/product/update?productId=${product.id}"><i class="icon-edit-3"></i></a>
                                </div>
                                <div class="item trash" onclick="deleteProduct(this, ${product.id}, '${product.name}')">
                                    <i class="icon-trash-2 ${!product.isDelete ? '' : 'hide'}"></i>
                                </div>
                            </div>
                        </div>
                    </li>
                `;
                index += 1
        productList.append(productItem);
    });
}

function generateFormInput(pageSize, pageNumber, categoryId, productCateId, key) {
    let searchForm = $('#form-search');
    searchForm.empty();
    let input = `<input type="text" placeholder="Tìm tên sản phẩm..." tabindex="2" value="${key}" id="search-input" style="border-radius: 10px;"
                     onkeypress="if(event.key == 'Enter'){ chooseKey(${pageSize}, ${pageNumber}, ${categoryId}, ${productCateId}); }">
                 `;
    let search = `<span class="" type="submit" onclick="chooseKey(${pageSize}, ${pageNumber}, ${categoryId}, ${productCateId})">
                        <i class="icon-search"></i>
                      </span>`;
    searchForm.append(input);
    searchForm.append(search);
}


function fetchProducts(pageSize, pageNumber, categoryId, productCateId, key) {

    if (key == null) {
        key = '';
    }

    $.ajax({
        url: 'http://localhost:5206/admin/product/fetchproduct', // Replace with your API endpoint
        type: 'POST',
        data: {
            pageSize: pageSize,
            pageNumber: pageNumber,
            categoryId: categoryId,
            productCateId: productCateId,
            key: key
        },
        success: function (response) {
            let products = response.products;
            let pageSize = response.pageSize;
            let pageNumber = response.pageNumber;
            let totalPageNumber = response.totalPageNumber;
            let categories = response.categories;
            let productCategories = response.productCategories;

            if (response.products.length == 0) {
                key = '';
                $('#no-option').show();
                $('#table-product').hide();
            }
            else {
                generateProductList(products);
                $('#no-option').hide();
                $('#table-product').show();
            }

            generatePageSize(pageSize, 1, categoryId, productCateId, key);

            generatePagination(totalPageNumber, pageNumber, pageSize, categoryId, productCateId, key)

            generateCategory(pageSize, categoryId, categories, 1, null, key);

            generateProductCategory(productCategories, pageSize, 1, categoryId, productCateId, key);

            generateFormInput(pageSize, pageNumber, categoryId, productCateId, key)

            generateSortPrice();

            generateSortSoldQuantity();

            generateIsInStock();

            generateIsDelete();

            generateIsInStock();
            
        },
        error: function (xhr, status, error) {
            console.error('Error fetching products:', error);
        }
    });
}

function generatePagination(totalPageNumber, currentPage, pageSize, categoryId, productCateId, key) {
    let parentElement = $('#pageNumber');
    parentElement.empty();
    if (totalPageNumber > 1) {
        if (currentPage > 1) {
            let prevPage = currentPage - 1;
            let prevPageLink = $('<a>', {
                class: 'page-link',
                onclick: `ChoosePage(${prevPage}, ${pageSize}, ${categoryId}, ${productCateId}, '${key}')`,
                text: 'Trang trước'
            });
            let prevPageItem = $('<li>', { class: 'page-item' }).append(prevPageLink);
            parentElement.append(prevPageItem);
        } else {
            let prevPageLink = $('<a>', {
                class: 'page-link',
                style: 'pointer-events: none; cursor: default; color: #b7b7b7;',
                text: 'Trang trước'
            });
            let prevPageItem = $('<li>', { class: 'page-item' }).append(prevPageLink);
            parentElement.append(prevPageItem);
        }

        let startPage = Math.max(1, currentPage - 2);
        let endPage = Math.min(totalPageNumber, currentPage + 2);

        if (startPage > 1) {
            let firstPageLink = $('<a>', {
                class: 'page-link',
                onclick: `ChoosePage(1, ${pageSize}, ${categoryId}, ${productCateId}, '${key}')`,
                text: '1'
            });
            let firstPageItem = $('<li>', { class: 'page-item' }).append(firstPageLink);
            parentElement.append(firstPageItem);

            if (startPage > 2) {
                let ellipsisItem = $('<li>', { class: 'page-item' }).html('<a class="page-link">...</a>');
                parentElement.append(ellipsisItem);
            }
        }

        for (let i = startPage; i <= endPage; i++) {
            if (currentPage === i) {
                let activePageItem = $('<li>', { class: 'page-item active' }).html('<a class="page-link">' + i + '</a>');
                parentElement.append(activePageItem);
            } else {
                let pageLink = $('<a>', {
                    class: 'page-link',
                    onclick: `ChoosePage(${i}, ${pageSize}, ${categoryId}, ${productCateId}, '${key}')`,
                    text: i
                });
                let pageItem = $('<li>', { class: 'page-item' }).append(pageLink);
                parentElement.append(pageItem);
            }
        }

        if (totalPageNumber >= endPage + 1) {
            if (totalPageNumber >= endPage + 2) {
                let ellipsisItem = $('<li>', { class: 'page-item' }).html('<a class="page-link">...</a>');
                parentElement.append(ellipsisItem);
            }

            let lastPageLink = $('<a>', {
                class: 'page-link',
                onclick: `ChoosePage(${totalPageNumber}, ${pageSize}, ${categoryId}, ${productCateId}, '${key}')`,
                text: totalPageNumber
            });
            let lastPageItem = $('<li>', { class: 'page-item' }).append(lastPageLink);
            parentElement.append(lastPageItem);
        }

        if (currentPage != totalPageNumber) {
            let nextPage = currentPage + 1;
            let nextPageLink = $('<a>', {
                class: 'page-link',
                onclick: `ChoosePage(${nextPage}, ${pageSize}, ${categoryId}, ${productCateId}, '${key}')`,
                text: 'Trang sau'
            });
            let nextPageItem = $('<li>', { class: 'page-item' }).append(nextPageLink);
            parentElement.append(nextPageItem);
        } else {
            let nextPageLink = $('<a>', {
                class: 'page-link',
                style: 'pointer-events: none; cursor: default; color: #b7b7b7;',
                text: 'Trang sau'
            });
            let nextPageItem = $('<li>', { class: 'page-item' }).append(nextPageLink);
            parentElement.append(nextPageItem);
        }
    }
}

function generateCategory(pageSize, categoryId, categories, pageNumber, productCateId, key) {
    console.log(categories);
    let categoryList = $('#category-list');
    categoryList.empty();

    let li = $('<li>');
    let href = $('<a>', {
        href: '#',
        class: ((categoryId == null || categoryId == 0) ? 'choose' : ''),
        style: "width: 250px",
        onclick: `ChooseCategory(${0}, ${pageSize}, ${pageNumber}, ${productCateId}, '${key}')`
    }).text('- Tất cả');
    li.append(href);
    categoryList.append(li);

    categories.forEach(function (category) {
        let li = $('<li>');
        let href = $('<a>', {
            href: '#',
            class: (categoryId == category.id ? 'choose' : ''),
            style: "width: 250px",
            onclick: `ChooseCategory(${category.id}, ${pageSize}, ${pageNumber}, ${productCateId}, '${key}')`
        }).text('- ' + category.name);
        li.append(href);
        categoryList.append(li);
    });
}

function generateProductCategory(productCategories, pageSize, pageNumber, categoryId, productCateId, key) {
    let productCategoryList = $('#product-category-list');
    productCategoryList.empty();
    productCategories.forEach(function (productCategory) {
        let li = $('<li>');
        let href = $('<a>', {
            href: '#',
            class: (productCateId == productCategory.id ? 'choose' : ''),
            style: 'width: 250px',
            onclick: `chooseProductCategory(${productCategory.id}, ${pageSize}, ${pageNumber}, ${categoryId}, '${key}')`
        }).text('- ' + productCategory.name);
        li.append(href);
        productCategoryList.append(li);
    });
}

function generatePageSize(pageSize, pageNumber, categoryId, productCateId, key) {
    let p = $('#pageSize');
    let pS = $('<select>', {
        id: 'size',
        onchange: `ChoosePageSize(${pageSize}, ${pageNumber}, ${categoryId}, 
            ${productCateId}, '${key}')`
    });
    p.empty();
    let pageSizeList = [10, 20, 30];
    pageSizeList.forEach(function (size) {
        let option = `<option value="${size}" ${pageSize == size ? 'selected' : ''}>${size}</option>`;
        pS.append(option);
    });
    p.append(pS);
}

function generateSortPrice() {
    let sort_price = $('#sort-price');
    sort_price.empty();
    let sortPriceList = [true, false];
    let sortPriceName = ['Giá: Thấp đến cao', 'Giá: Cao đến thấp'];
    sortPriceList.forEach(function (price) {
        li = $('<li>');
        href = $('<a>', {
            style: 'width: 200px',
            href: '#',
            onclick: `ChooseSortPrice(${price})`
        }).text('- ' + sortPriceName[sortPriceList.indexOf(price)]);
        li.append(href);
        sort_price.append(li);
    });
}

function generateSortSoldQuantity() {
    let sort_sold_quantity = $('#sort-sold-quantity');
    sort_sold_quantity.empty();
    let sortSoldQuantityList = [true, false];
    let sortSoldQuantityName = ['Số lượng bán: Thấp đến cao', 'Số lượng bán: Cao đến thấp'];
    sortSoldQuantityList.forEach(function (soldQuantity) {
        li = $('<li>');
        href = $('<a>', {
            style: 'width: 240px',
            href: '#',
            onclick: `ChooseSortSoldQuantity(${soldQuantity})`
        }).text('- ' + sortSoldQuantityName[sortSoldQuantityList.indexOf(soldQuantity)]);
        li.append(href);
        sort_sold_quantity.append(li);
    });
}

function generateIsInStock() {
    let in_stock = $('#is-in-stock');
    in_stock.empty();
    let li = $('<li>');
    let href = $('<a>', {
        href: '#',
        class: 'choose',
        onclick: `ChooseIsInStock(this, ${null})`,
    }).text('- Tất cả');
    li.append(href);
    in_stock.append(li);
    let isSoldOutList = [true, false];
    let isSoldOutName = ['Tăng dần', 'Giảm dần'];
    isSoldOutList.forEach(function (inSoldOut) {
        li = $('<li>');
        href = $('<a>', {
            href: '#',
            onclick: `ChooseIsInStock(this, ${inSoldOut})`,
        }).text('- ' + isSoldOutName[isSoldOutList.indexOf(inSoldOut)]);
        li.append(href);
        in_stock.append(li);
    }
    );
}

function generateIsDelete() {
    let is_delete = $('#is-delete');
    is_delete.empty();
    let li = $('<li>');
    let href = $('<a>', {
        style: 'width: 130px',
        href: '#',
        class: 'choose',
        onclick: `ChooseIsDelete(this, ${null})`,
    }).text('- Tất cả');
    li.append(href);
    is_delete.append(li);
    let isDeleteList = [true, false];
    let isDeleteName = ['Ngừng bán', 'Còn bán'];
    isDeleteList.forEach(function (isdelete) {
        li = $('<li>');
        href = $('<a>', {
            href: '#',
            style: 'width: 130px',
            onclick: `ChooseIsDelete(this, ${isdelete})`,
        }).text('- ' + isDeleteName[isDeleteList.indexOf(isdelete)]);
        li.append(href);
        is_delete.append(li);
    });
}
