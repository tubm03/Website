function confirmServiceId(serviceIds, url) {
    let serviceId = Number($("#serviceId-search").val());

    if (!isNaN(serviceId)) {
        if (serviceIds.includes(serviceId)) {
            window.location.href = url + serviceId;
            return;
        }
    }
    $("#error-input").text("Giá trị ID không tồn tại. Vui lòng nhập lại.");
}

function pet_type_selected(serviceId, petType) {
    let pet_type_list = document.getElementById("pet_type_list").querySelectorAll("li");
    pet_type_list.forEach(function (item) {
        item.classList.remove("select");
    });
    console.log(petType);
    let pet_type_selected = document.getElementById(petType);
    pet_type_selected.classList.add("select");

    $.ajax({
        type: "POST",
        url: "/Admin/Service/GetOptionViewModel",
        data: { serviceId: serviceId, petType: petType },
        success: function (response) {
            $('#price').text(response.price.toLocaleString('en-US'));

            let weightList = '';
            response.weights.forEach(function (weight) {
                var isSelected = (response.weight === weight);

                weightList += '<li id="' + weight + '"';
                weightList += (isSelected ? ' class="select"' : '');
                weightList += ' onclick="weight_selected(\'' + response.serviceId + '\',\''
                    + response.petType + '\',\'' + weight + '\')">' + weight + '</li>';
            });

            let status = `<div class="block-warning w-full mt-5">
                            <i class="icon-alert-octagon"></i>
                            <div class="body-title-2" id="status">Tùy chọn dịch vụ đã ngừng kinh doanh</div>
                        </div>`;

            if (response.isDelete) {
                $("#error-status").empty();
                $("#error-status").html(status);
            } else {
                $("#error-status").empty();
            }

            $('#weight_list').html(weightList);
            $('#book_service').attr('data-service-option-id', response.serviceOptionId);
        },

        error: function (xhr, status, error) {
            console.error('The request failed!', status, error);
        }
    });
};

function weight_selected(serviceId, petType, weight) {
    let weight_list = document.getElementById("weight_list").querySelectorAll("li");
    weight_list.forEach(function (item) {
        item.classList.remove("select");
    });
    let weight_selected = document.getElementById(weight);
    weight_selected.classList.add("select");

    $.ajax({
        type: "POST",
        url: "/Admin/Service/GetServiceOptionPrice",
        data: { serviceId: serviceId, petType: petType, weight: weight },
        success: function (response) {
            let status = `<div class="block-warning w-full mt-5">
                            <i class="icon-alert-octagon"></i>
                            <div class="body-title-2" id="status">Tùy chọn dịch vụ đã ngừng kinh doanh</div>
                        </div>`;

            if (response.isDelete) {
                $("#error-status").empty();
                $("#error-status").html(status);
            } else {
                $("#error-status").empty();
            }

            $('#price').text(response.price.toLocaleString('en-US'));
            $('#book_service').attr("data-service-option-id", response.serviceOptionId);
        },

        error: function (xhr, status, error) {
            console.error('The request failed!', status, error);
        }
    });
};