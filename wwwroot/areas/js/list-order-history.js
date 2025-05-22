
function OrderDetail(element) {
    let orderData = element.getAttribute('data-order');
    let order = JSON.parse(decodeURIComponent(orderData));

    $.ajax({
        url: '/admin/order/Detail',
        type: 'post',
        data: order,
        success: function (response) {

            $('#modal_title_detail').empty();
            $('#modal_content_detail').html(response);
            $('#orderDetail').modal("show");

        },
        error: function (xhr, status, error) {
            console.error("Error:", error);
        }
    });
}